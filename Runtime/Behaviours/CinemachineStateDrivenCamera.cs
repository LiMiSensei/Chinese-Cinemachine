#if CINEMACHINE_UNITY_ANIMATION
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
    /// <summary>
    /// This is a virtual camera "manager" that owns and manages a collection
    /// of child Virtual Cameras.  These child vcams are mapped to individual states in
    /// an animation state machine, allowing you to associate specific vcams to specific
    /// animation states.  When that state is active in the state machine, then the
    /// associated camera will be activated.
    ///
    /// You can define custom blends and transitions between child cameras.
    ///
    /// In order to use this behaviour, you must have an animated target (i.e. an object
    /// animated with a state machine) to drive the behaviour.
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [ExcludeFromPreset]
    [SaveDuringPlay]
    [AddComponentMenu("Cinemachine/Cinemachine State Driven Camera")]
    [HelpURL(Documentation.BaseURL + "manual/CinemachineStateDrivenCamera.html")]
    public class CinemachineStateDrivenCamera : CinemachineCameraManagerBase
    {
        /// <summary>状态机，其状态变化将驱动此相机选择活动子相机</summary>
[Space]
[Tooltip("状态机，其状态变化将驱动此相机选择活动子相机")]
[NoSaveDuringPlay]
[FormerlySerializedAs("m_AnimatedTarget")]
public Animator AnimatedTarget;

/// <summary>要观察的目标状态机中的哪个层</summary>
[Tooltip("要观察的目标状态机中的哪个层")]
[NoSaveDuringPlay]
[FormerlySerializedAs("m_LayerIndex")]
public int LayerIndex;

/// <summary>这表示对状态驱动相机的单条指令。它将状态机中的一个状态与子虚拟相机关联起来，
/// 并包含激活调优参数。</summary>
[Serializable]
public struct Instruction
{
    /// <summary>动画状态的完整哈希值</summary>
    [Tooltip("动画状态的完整哈希值")]
    [FormerlySerializedAs("m_FullHash")]
    public int FullHash;
    
    /// <summary>当动画状态激活时要激活的虚拟相机</summary>
    [Tooltip("当动画状态激活时要激活的虚拟相机")]
    [FormerlySerializedAs("m_VirtualCamera")]
    [ChildCameraProperty]
    public CinemachineVirtualCameraBase Camera;
    
    /// <summary>激活相机前要等待的时间（以秒为单位）。
    /// 这可以过滤掉非常短的状态持续时间</summary>
    [Tooltip("激活相机前要等待的时间（以秒为单位）。"
        + "这可以过滤掉非常短的状态持续时间")]
    [FormerlySerializedAs("m_ActivateAfter")]
    public float ActivateAfter;
    
    /// <summary>保持相机活动的最小时间长度（以秒为单位）</summary>
    [Tooltip("保持相机活动的最小时间长度（以秒为单位）")]
    [FormerlySerializedAs("m_MinDuration")]
    public float MinDuration;
    };

    /// <summary>将虚拟相机与状态关联的指令集。
    /// 这些指令用于在任何给定时刻选择活动子相机</summary>
    [Tooltip("将相机与状态关联的指令集。"
        + "这些指令用于在任何给定时刻选择活动子相机")]
    [FormerlySerializedAs("m_Instructions")]
    public Instruction[] Instructions;


        /// <summary>Internal API for the Inspector editor.  This implements nested states.</summary>
        [Serializable]
        internal struct ParentHash
        {
            /// <summary>Internal API for the Inspector editor</summary>
            public int Hash;
            /// <summary>Internal API for the Inspector editor</summary>
            public int HashOfParent;
        }
        /// <summary>Internal API for the Inspector editor</summary>
        [HideInInspector, SerializeField, NoSaveDuringPlay] private List<ParentHash> HashOfParent = new();

        /// <summary>Internal API for the Inspector editor</summary>
        internal void SetParentHash(List<ParentHash> list)
        {
            HashOfParent.Clear();
            HashOfParent.AddRange(list);
        }

        [SerializeField, HideInInspector, NoSaveDuringPlay, FormerlySerializedAs("m_LookAt")] Transform m_LegacyLookAt;
        [SerializeField, HideInInspector, NoSaveDuringPlay, FormerlySerializedAs("m_Follow")] Transform m_LegacyFollow;

        float m_ActivationTime = 0;
        int m_ActiveInstructionIndex;
        float m_PendingActivationTime = 0;
        int m_PendingInstructionIndex;
        Dictionary<int, List<int>> m_InstructionDictionary;
        Dictionary<int, int> m_StateParentLookup;
        readonly List<AnimatorClipInfo> m_ClipInfoList = new ();

        /// <inheritdoc />
        protected override void Reset()
        {
            base.Reset();
            Instructions = null;
            AnimatedTarget = null;
            LayerIndex = 0;
            Instructions = null;
            DefaultBlend = new (CinemachineBlendDefinition.Styles.EaseInOut, 0.5f);
            CustomBlends = null;
        }

        /// <inheritdoc/>
        protected internal override void PerformLegacyUpgrade(int streamedVersion)
        {
            base.PerformLegacyUpgrade(streamedVersion);
            if (streamedVersion < 20220721)
            {
                if (m_LegacyLookAt != null || m_LegacyFollow != null)
                {
                    DefaultTarget = new DefaultTargetSettings
                    {
                        Enabled = true,
                        Target = new CameraTarget
                        {
                            LookAtTarget = m_LegacyLookAt,
                            TrackingTarget = m_LegacyFollow,
                            CustomLookAtTarget = m_LegacyLookAt != m_LegacyFollow
                        }
                    };
                    m_LegacyLookAt = m_LegacyFollow = null;
                }
            }
        }

        /// <summary>API for the inspector editor.  Animation module does not have hashes
        /// for state parents, so we have to invent them in order to implement nested state
        /// handling</summary>
        /// <param name="parentHash">Parent state's hash</param>
        /// <param name="clip">The clip to create the fake hash for</param>
        /// <returns>The fake hash</returns>
        internal static int CreateFakeHash(int parentHash, AnimationClip clip)
        {
            return Animator.StringToHash(parentHash.ToString() + "_" + clip.name);
        }

        // Avoid garbage string manipulations at runtime
        struct HashPair { public int parentHash; public int hash; }
        Dictionary<AnimationClip, List<HashPair>> m_HashCache;
        int LookupFakeHash(int parentHash, AnimationClip clip)
        {
            m_HashCache ??= new Dictionary<AnimationClip, List<HashPair>>();
            if (!m_HashCache.TryGetValue(clip, out var list))
            {
                list = new List<HashPair>();
                m_HashCache[clip] = list;
            }
            for (int i = 0; i < list.Count; ++i)
                if (list[i].parentHash == parentHash)
                    return list[i].hash;
            int newHash = CreateFakeHash(parentHash, clip);
            list.Add(new HashPair() { parentHash = parentHash, hash = newHash });
            m_StateParentLookup[newHash] = parentHash;
            return newHash;
        }

        /// <summary>Internal API for the inspector editor.</summary>
        internal void ValidateInstructions()
        {
            Instructions ??= Array.Empty<Instruction>();
            m_InstructionDictionary = new Dictionary<int, List<int>>();
            for (int i = 0; i < Instructions.Length; ++i)
            {
                if (!m_InstructionDictionary.TryGetValue(Instructions[i].FullHash, out var list))
                {
                    list = new List<int>();
                    m_InstructionDictionary[Instructions[i].FullHash] = list;
                }
                list.Add(i);
            }

            // Create the parent lookup
            m_StateParentLookup = new Dictionary<int, int>();
            for (int i = 0; HashOfParent != null && i < HashOfParent.Count; ++i)
                m_StateParentLookup[HashOfParent[i].Hash] = HashOfParent[i].HashOfParent;

            // Zap the cached current instructions
            m_HashCache = null;
            m_ActivationTime = m_PendingActivationTime = 0;
            ResetLiveChild();
        }

        /// <inheritdoc />
        protected override CinemachineVirtualCameraBase ChooseCurrentCamera(Vector3 worldUp, float deltaTime)
        {
            if (!PreviousStateIsValid)
                ValidateInstructions();

            var children = ChildCameras;
            if (children == null || children.Count == 0)
            {
                m_ActivationTime = 0;
                return null;
            }
            var fallbackCam = children[0];
            if (AnimatedTarget == null || !AnimatedTarget.gameObject.activeSelf
                || AnimatedTarget.runtimeAnimatorController == null
                || LayerIndex < 0 || !AnimatedTarget.hasBoundPlayables
                || LayerIndex >= AnimatedTarget.layerCount)
            {
                m_ActivationTime = 0;
                return fallbackCam;
            }

            // quick sanity check, in case number of instructions changed
            if (m_ActiveInstructionIndex < 0 || m_ActiveInstructionIndex >= Instructions.Length)
            {
                m_ActiveInstructionIndex = 0;
                m_ActivationTime = 0;
            }
            if (!PreviousStateIsValid || m_PendingInstructionIndex < 0 || m_PendingInstructionIndex >= Instructions.Length)
            {
                m_PendingInstructionIndex = 0;
                m_PendingActivationTime = 0;
            }

            // Get the current animation state
            int hash;
            if (AnimatedTarget.IsInTransition(LayerIndex))
            {
                // Force "current" state to be the state we're transitioning to
                var info = AnimatedTarget.GetNextAnimatorStateInfo(LayerIndex);
                AnimatedTarget.GetNextAnimatorClipInfo(LayerIndex, m_ClipInfoList);
                hash = GetClipHash(info.fullPathHash, m_ClipInfoList);
            }
            else
            {
                var info = AnimatedTarget.GetCurrentAnimatorStateInfo(LayerIndex);
                AnimatedTarget.GetCurrentAnimatorClipInfo(LayerIndex, m_ClipInfoList);
                hash = GetClipHash(info.fullPathHash, m_ClipInfoList);
            }

            // If we don't have an instruction for this state, find a suitable default state
            while (hash != 0 && !m_InstructionDictionary.ContainsKey(hash))
                hash = m_StateParentLookup.ContainsKey(hash) ? m_StateParentLookup[hash] : 0;

            // Get the highest-priority active camera from the instruction list
            int newInstrIndex = -1;
            if (m_InstructionDictionary.ContainsKey(hash))
            {
                var instrList = m_InstructionDictionary[hash];

                // Find the instruction whose camera is active and has the highest priority
                int bestPriority = int.MinValue;
                for (int i = 0; i < instrList.Count; ++i)
                {
                    var index = instrList[i];
                    var cam = index < Instructions.Length ? Instructions[index].Camera : null;
                    if (cam != null && cam.isActiveAndEnabled && cam.Priority.Value > bestPriority)
                    {
                        newInstrIndex = index;
                        bestPriority = cam.Priority.Value;
                    }
                }
            }

            // Process it.  If no new camera is desired, we just ignore this state
            var now = CinemachineCore.CurrentTime;
            if (newInstrIndex >= 0)
            {
                // If it's neither active nor pending, we must take action
                if (m_ActivationTime == 0)
                {
                    // No current camera, actibvate immediately
                    m_ActiveInstructionIndex = newInstrIndex;
                    m_ActivationTime = now;
                    m_PendingActivationTime = 0;
                }
                else if (m_ActiveInstructionIndex != newInstrIndex
                    && (m_PendingActivationTime == 0 || m_PendingInstructionIndex != newInstrIndex))
                {
                    // Make it pending
                    m_PendingInstructionIndex = newInstrIndex;
                    m_PendingActivationTime = now;
                }
            }

            // Process the pending instruction
            if (m_PendingActivationTime != 0)
            {
                // Has it been pending long enough, and are we allowed to switch away
                // from the active action?
                if ((now - m_PendingActivationTime) > Instructions[m_PendingInstructionIndex].ActivateAfter
                    && (now - m_ActivationTime) > Instructions[m_ActiveInstructionIndex].MinDuration)
                {
                    // Yes, activate it now
                    m_ActiveInstructionIndex = m_PendingInstructionIndex;
                    m_ActivationTime = now;
                    m_PendingActivationTime = 0;
                }
            }

            if (m_ActivationTime != 0)
                return Instructions[m_ActiveInstructionIndex].Camera;
            return fallbackCam;
        }

        int GetClipHash(int hash, List<AnimatorClipInfo> clips)
        {
            // Find the strongest-weighted animation clip sub-state
            var bestClip = -1;
            for (var i = 0; i < clips.Count; ++i)
                if (bestClip < 0 || clips[i].weight > clips[bestClip].weight)
                    bestClip = i;

            // Use its hash
            if (bestClip >= 0 && clips[bestClip].weight > 0)
                hash = LookupFakeHash(hash, clips[bestClip].clip);

            return hash;
        }

        /// <summary>
        /// Call this to cancel the current wait time for the pending instruction and activate
        /// the pending instruction immediately.
        /// </summary>
        public void CancelWait()
        {
            if (m_PendingActivationTime != 0 && m_PendingInstructionIndex >= 0 && m_PendingInstructionIndex < Instructions.Length)
            {
                m_ActiveInstructionIndex = m_PendingInstructionIndex;
                m_ActivationTime = CinemachineCore.CurrentTime;
                m_PendingActivationTime = 0;
            }
        }
    }
}
#endif
