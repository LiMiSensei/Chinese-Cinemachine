using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
    /// <summary>
    /// This is a virtual camera "manager" that owns and manages a collection
    /// of child Virtual Cameras.  When the camera goes live, these child vcams
    /// are enabled, one after another, holding each camera for a designated time.
    /// Blends between cameras are specified.
    /// The last camera is held indefinitely, unless the Loop flag is enabled.
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [ExcludeFromPreset]
    [SaveDuringPlay]
    [AddComponentMenu("Cinemachine/Cinemachine Sequencer Camera")]
    [HelpURL(Documentation.BaseURL + "manual/CinemachineSequencerCamera.html")]
    public class CinemachineSequencerCamera : CinemachineCameraManagerBase
    {
        /// <summary>启用后，子虚拟相机会无限循环，而不会在最后一个停止</summary>
        [Tooltip("启用后，子虚拟相机会无限循环，而不会在最后一个停止")]
        [FormerlySerializedAs("m_Loop")]
        public bool Loop;

        /// <summary>表示 BlendListCamera 指令列表中的单个条目</summary>
        [Serializable]
        public struct Instruction
        {
            /// <summary>当此指令激活时要启用的相机</summary>
            [Tooltip("当此指令激活时要启用的相机")]
            [FormerlySerializedAs("m_VirtualCamera")]
            [ChildCameraProperty]
            public CinemachineVirtualCameraBase Camera;

            /// <summary>如何混合到列表中的下一个相机（如果有）</summary>
            [Tooltip("如何混合到列表中的下一个相机（如果有）")]
            [FormerlySerializedAs("m_Blend")]
            public CinemachineBlendDefinition Blend;

            /// <summary>激活列表中的下一个相机之前要等待的时间（秒）</summary>
            [Tooltip("激活列表中的下一个相机之前要等待的时间（秒）")]
            [FormerlySerializedAs("m_Hold")]
            public float Hold;

            /// <summary>将设置限制在合理范围内</summary>
            public void Validate() => Hold = Mathf.Max(Hold, 0);
        };

        /// <summary>用于启用子相机的指令集</summary>
        [Tooltip("用于启用子相机的指令集")]
        [FormerlySerializedAs("m_Instructions")]

        public List<Instruction> Instructions = new ();

        [SerializeField, HideInInspector, NoSaveDuringPlay, FormerlySerializedAs("m_LookAt")] Transform m_LegacyLookAt;
        [SerializeField, HideInInspector, NoSaveDuringPlay, FormerlySerializedAs("m_Follow")] Transform m_LegacyFollow;

        float m_ActivationTime = -1; // The time at which the current instruction went live
        int m_CurrentInstruction = 0;

        /// <inheritdoc />
        protected override void Reset()
        {
            base.Reset();
            Loop = false;
            Instructions = null;
        }

        void OnValidate()
        {
            if (Instructions != null)
            {
                for (int i = 0; i < Instructions.Count; ++i)
                {
                    var e = Instructions[i];
                    e.Validate();
                    Instructions[i] = e;
                }
            }
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

        /// <inheritdoc />
        public override void OnTransitionFromCamera(
            ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime)
        {
            m_ActivationTime = CinemachineCore.CurrentTime;
            m_CurrentInstruction = 0;
        }

        /// <inheritdoc />
        protected override CinemachineVirtualCameraBase ChooseCurrentCamera(Vector3 worldUp, float deltaTime)
        {
            if (!PreviousStateIsValid)
                m_CurrentInstruction = -1;

            AdvanceCurrentInstruction(deltaTime);
            return (m_CurrentInstruction >= 0 && m_CurrentInstruction < Instructions.Count)
                ? Instructions[m_CurrentInstruction].Camera : null;
        }

        /// <summary>Returns the current blend of the current instruction.</summary>
        /// <param name="outgoing">The camera we're blending from (ignored).</param>
        /// <param name="incoming">The camera we're blending to (ignored).</param>
        /// <returns>The blend to use for this camera transition.</returns>
        protected override CinemachineBlendDefinition LookupBlend(
            ICinemachineCamera outgoing, ICinemachineCamera incoming) => Instructions[m_CurrentInstruction].Blend;

        /// <inheritdoc />
        protected override bool UpdateCameraCache()
        {
            Instructions ??= new ();
            return base.UpdateCameraCache();
        }

        void AdvanceCurrentInstruction(float deltaTime)
        {
            if (ChildCameras == null || ChildCameras.Count == 0
                || m_ActivationTime < 0 || Instructions.Count == 0)
            {
                m_ActivationTime = -1;
                m_CurrentInstruction = -1;
                return;
            }

            var now = CinemachineCore.CurrentTime;
            if (m_CurrentInstruction < 0 || deltaTime < 0)
            {
                m_ActivationTime = now;
                m_CurrentInstruction = 0;
            }
            if (m_CurrentInstruction > Instructions.Count - 1)
            {
                m_ActivationTime = now;
                m_CurrentInstruction = Instructions.Count - 1;
            }

            var holdTime = Instructions[m_CurrentInstruction].Hold
                + Instructions[m_CurrentInstruction].Blend.BlendTime;
            var minHold = m_CurrentInstruction < Instructions.Count - 1 || Loop ? 0 : float.MaxValue;
            if (now - m_ActivationTime > Mathf.Max(minHold, holdTime))
            {
                m_ActivationTime = now;
                ++m_CurrentInstruction;
                if (Loop && m_CurrentInstruction == Instructions.Count)
                    m_CurrentInstruction = 0;
            }
        }
    }
}
