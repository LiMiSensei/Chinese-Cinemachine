using UnityEngine;
using System;

namespace Unity.Cinemachine
{
    /// <summary>
    /// 此行为旨在附加到空的 GameObject 上，
    /// 它在 Unity 场景中代表一个 Cinemachine 摄像机。
    ///
    /// CinemachineCamera 将根据其 CinemachineComponent 管线（瞄准、身体和噪声）
    /// 中包含的规则来动画化其 Transform。当 CM 摄像机处于活动状态时，
    /// Unity 摄像机将采用 CinemachineCamera 的位置和方向。
    ///
    /// CinemachineCamera 不是摄像机。相反，它可以被视为摄像机控制器，
    /// 类似于摄影师。它可以驱动 Unity 摄像机并控制其位置、
    /// 旋转、镜头设置和后处理效果。每个 CM 摄像机都拥有
    /// 自己的 Cinemachine 组件管线，通过该管线您可以提供
    /// 程序化跟踪特定游戏对象的指令。空的程序化管线
    /// 将导致被动的 CinemachineCamera，可以像普通 GameObject 一样控制。
    ///
    /// CinemachineCamera 非常轻量，不进行自己的渲染。它只是
    /// 跟踪有趣的 GameObject，并相应地定位自己。一个典型的游戏
    /// 可以有几十个 CinemachineCamera，每个都设置为跟随特定角色
    /// 或捕捉特定事件。
    ///
    /// CinemachineCamera 可以处于以下三种状态之一：
    ///
    /// * **活动状态**：CinemachineCamera 正在主动控制 Unity 摄像机。
    /// CinemachineCamera 正在跟踪其目标并每帧更新。
    /// * **待命状态**：CinemachineCamera 正在跟踪其目标并每帧更新，
    /// 但没有 Unity 摄像机被它主动控制。这是
    /// 场景中启用但优先级可能低于活动 CinemachineCamera 的
    /// CinemachineCamera 的状态。
    /// * **禁用状态**：CinemachineCamera 存在但在场景中被禁用。它
    /// 不主动跟踪其目标，因此不消耗处理能力。但是，
    /// CinemachineCamera 可以从时间轴变为活动状态。
    ///
    /// Unity 摄像机可以由场景中的任何 CinemachineCamera 驱动。游戏
    /// 逻辑可以通过操纵 CM 摄像机的启用标志和/或其优先级
    /// 来选择要激活的 CinemachineCamera，基于游戏逻辑。
    ///
    /// 为了由 CinemachineCamera 驱动，Unity 摄像机必须具有 CinemachineBrain
    /// 行为，该行为将根据其优先级或其他条件选择最符合条件的
    /// CinemachineCamera，并管理混合。
    /// </summary>
    /// 
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [SaveDuringPlay]
    [AddComponentMenu("Cinemachine/Cinemachine Camera")]
    [HelpURL(Documentation.BaseURL + "manual/CinemachineCamera.html")]
    public sealed class CinemachineCamera : CinemachineVirtualCameraBase
    {
        [RedHeader("Help：摄像机控制器-主动控制 Unity 摄像机,优先级活动，必须具有 CinemachineBrain")]
        [Space(30)]
        /// <summary>The Tracking and LookAt targets for this camera.</summary>
        [NoSaveDuringPlay]
        [Tooltip("指定此相机的跟踪目标和注视目标。")]
        public CameraTarget Target;

        /// <summary>指定此相机的镜头设置。
        /// 当Cinemachine相机激活时，这些设置将传递给Unity相机。</summary>
        [Tooltip("指定此虚拟相机的镜头属性。这些属性通常与"
            + "Unity相机的镜头设置相对应，并在虚拟相机激活时用于驱动Unity相机。")]
        public LensSettings Lens = LensSettings.Default;

        /// <summary>从此Cinemachine相机过渡和过渡到此相机的提示。提示可以组合使用，尽管
        /// 并非所有组合都有意义。在提示冲突的情况下，Cinemachine将
        /// 做出任意选择。</summary>
        [Tooltip("从此Cinemachine相机过渡和过渡到此相机的提示。提示可以组合使用，尽管"
            + "并非所有组合都有意义。在提示冲突的情况下，Cinemachine将"
            + "做出任意选择。")]
        public CinemachineCore.BlendHints BlendHint;

        CameraState m_State = CameraState.Default;
        CinemachineComponentBase[] m_Pipeline;

        void Reset()
        {
            Priority = new();
            OutputChannel = OutputChannels.Default;
            Target = default;
            Lens = LensSettings.Default;
        }

        /// <summary>Validates the settings avter inspector edit</summary>
        void OnValidate()
        {
            Lens.Validate();
        }

        /// <summary>The current camera state, which will applied to the Unity Camera</summary>
        public override CameraState State { get => m_State; }

        /// <summary>Get the current LookAt target.  Returns parent's LookAt if parent
        /// is non-null and no specific LookAt defined for this camera</summary>
        public override Transform LookAt
        {
            get { return ResolveLookAt(Target.CustomLookAtTarget ? Target.LookAtTarget : Target.TrackingTarget); }
            set { Target.CustomLookAtTarget = true; Target.LookAtTarget = value; }
        }

        /// <summary>Get the current Follow target.  Returns parent's Follow if parent
        /// is non-null and no specific Follow defined for this camera</summary>
        public override Transform Follow
        {
            get { return ResolveFollow(Target.TrackingTarget); }
            set { Target.TrackingTarget = value; }
        }

        /// <summary>This is called to notify the CinemachineCamera that a target got warped,
        /// so that the CinemachineCamera can update its internal state to make the camera
        /// also warp seamlessly.</summary>
        /// <param name="target">The object that was warped</param>
        /// <param name="positionDelta">The amount the target's position changed</param>
        public override void OnTargetObjectWarped(Transform target, Vector3 positionDelta)
        {
            if (target == Follow)
            {
                transform.position += positionDelta;
                m_State.RawPosition += positionDelta;
            }

            UpdatePipelineCache();
            for (int i = 0; i < m_Pipeline.Length; ++i)
            {
                if (m_Pipeline[i] != null)
                    m_Pipeline[i].OnTargetObjectWarped(target, positionDelta);
            }
            base.OnTargetObjectWarped(target, positionDelta);
        }

        /// <summary>
        /// Force the CinemachineCamera to assume a given position and orientation
        /// </summary>
        /// <param name="pos">World-space position to take</param>
        /// <param name="rot">World-space orientation to take</param>
        public override void ForceCameraPosition(Vector3 pos, Quaternion rot)
        {
            UpdatePipelineCache();
            for (int i = 0; i < m_Pipeline.Length; ++i)
                if (m_Pipeline[i] != null)
                    m_Pipeline[i].ForceCameraPosition(pos, rot);

            m_State.RawPosition = pos;
            m_State.RawOrientation = rot;

            // Push the raw position back to the game object's transform, so it
            // moves along with the camera.
            transform.ConservativeSetPositionAndRotation(pos, rot);

            base.ForceCameraPosition(pos, rot);
        }

        /// <summary>
        /// Query components and extensions for the maximum damping time.
        /// </summary>
        /// <returns>Highest damping setting in this CinemachineCamera</returns>
        public override float GetMaxDampTime()
        {
            float maxDamp = base.GetMaxDampTime();
            UpdatePipelineCache();
            for (int i = 0; i < m_Pipeline.Length; ++i)
                if (m_Pipeline[i] != null)
                    maxDamp = Mathf.Max(maxDamp, m_Pipeline[i].GetMaxDampTime());
            return maxDamp;
        }

        /// <summary>Handle transition from another CinemachineCamera.  InheritPosition is implemented here.</summary>
        /// <param name="fromCam">The camera being deactivated.  May be null.</param>
        /// <param name="worldUp">Default world Up, set by the CinemachineBrain</param>
        /// <param name="deltaTime">Delta time for time-based effects (ignore if less than or equal to 0)</param>
        public override void OnTransitionFromCamera(
            ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime)
        {
            base.OnTransitionFromCamera(fromCam, worldUp, deltaTime);
            InvokeOnTransitionInExtensions(fromCam, worldUp, deltaTime);
            bool forceUpdate = false;

            // Can't inherit position if already live, because there will be a pop
            if ((State.BlendHint & CameraState.BlendHints.InheritPosition) != 0
                && fromCam != null && !CinemachineCore.IsLiveInBlend(this))
            {
                var state = fromCam.State;
                ForceCameraPosition(state.GetFinalPosition(), state.GetFinalOrientation());
            }
            UpdatePipelineCache();
            for (int i = 0; i < m_Pipeline.Length; ++i)
                if (m_Pipeline[i] != null && m_Pipeline[i].OnTransitionFromCamera(fromCam, worldUp, deltaTime))
                    forceUpdate = true;

            if (!forceUpdate)
                UpdateCameraState(worldUp, deltaTime);
            else
            {
                // GML todo: why twice?  Isn't once enough?  Check this.
                InternalUpdateCameraState(worldUp, deltaTime);
                InternalUpdateCameraState(worldUp, deltaTime);
            }
        }

        /// <summary>Internal use only.  Called by CinemachineCore at designated update time
        /// so the vcam can position itself and track its targets.</summary>
        /// <param name="worldUp">Default world Up, set by the CinemachineBrain</param>
        /// <param name="deltaTime">Delta time for time-based effects (ignore if less than 0)</param>
        public override void InternalUpdateCameraState(Vector3 worldUp, float deltaTime)
        {
            UpdateTargetCache();

            FollowTargetAttachment = 1;
            LookAtTargetAttachment = 1;
            if (deltaTime < 0)
                PreviousStateIsValid = false;

            // Initialize the camera state, in case the game object got moved in the editor
            m_State = PullStateFromVirtualCamera(worldUp, ref Lens);

            // Do our stuff
            var lookAt = LookAt;
            if (lookAt != null)
                m_State.ReferenceLookAt = (LookAtTargetAsVcam != null)
                    ? LookAtTargetAsVcam.State.GetFinalPosition() : TargetPositionCache.GetTargetPosition(lookAt);
            m_State.BlendHint = (CameraState.BlendHints)BlendHint;
            InvokeComponentPipeline(ref m_State, deltaTime);

            // Push the raw position back to the game object's transform, so it
            // moves along with the camera.
            transform.ConservativeSetPositionAndRotation(m_State.RawPosition, m_State.RawOrientation);

            // Signal that it's all done
            PreviousStateIsValid = true;
        }

        CameraState InvokeComponentPipeline(ref CameraState state, float deltaTime)
        {
            // Extensions first
            InvokePrePipelineMutateCameraStateCallback(this, ref state, deltaTime);

            // Apply the component pipeline
            UpdatePipelineCache();
            for (int i = 0; i < m_Pipeline.Length; ++i)
            {
                var c = m_Pipeline[i];
                if (c != null && c.IsValid)
                    c.PrePipelineMutateCameraState(ref state, deltaTime);
            }
            CinemachineComponentBase postAimBody = null;
            for (int i = 0; i < m_Pipeline.Length; ++i)
            {
                var stage = (CinemachineCore.Stage)i;
                var c = m_Pipeline[i];
                if (c != null && c.IsValid)
                {
                    if (stage == CinemachineCore.Stage.Body && c.BodyAppliesAfterAim)
                    {
                        postAimBody = c;
                        continue; // do the body stage of the pipeline after Aim
                    }
                    c.MutateCameraState(ref state, deltaTime);
                }
                InvokePostPipelineStageCallback(this, stage, ref state, deltaTime);
                if (stage == CinemachineCore.Stage.Aim)
                {
                    // If we have saved a Body for after Aim, do it now
                    if (postAimBody != null)
                    {
                        postAimBody.MutateCameraState(ref state, deltaTime);
                        InvokePostPipelineStageCallback(this, CinemachineCore.Stage.Body, ref state, deltaTime);
                    }
                }
            }

            return state;
        }

        /// <summary>A CinemachineComponentBase has just been added or removed.  Pipeline cache will be rebuilt</summary>
        internal void InvalidatePipelineCache() => m_Pipeline = null;

        /// For unit tests
        internal bool PipelineCacheInvalidated => m_Pipeline == null;

        /// For unit tests
        internal Type PeekPipelineCacheType(CinemachineCore.Stage stage)
            => m_Pipeline[(int)stage] == null ? null : m_Pipeline[(int)stage].GetType();

        void UpdatePipelineCache()
        {
            const int pipelineLength = (int)CinemachineCore.Stage.Finalize + 1;
            if (m_Pipeline == null || m_Pipeline.Length != pipelineLength)
            {
                m_Pipeline = new CinemachineComponentBase[pipelineLength];
                var components = GetComponents<CinemachineComponentBase>();
                for (int i = 0; i < components.Length; ++i)
                {
                    if (m_Pipeline[(int)components[i].Stage] == null)
                        m_Pipeline[(int)components[i].Stage] = components[i];
//#if UNITY_EDITOR
//                    else
//                        Debug.LogWarning("Multiple " + components[i].Stage + " components on " + name);
//#endif
                }
            }
        }

        /// <summary>Get the component set for a specific stage in the pipeline.</summary>
        /// <param name="stage">The stage for which we want the component</param>
        /// <returns>The Cinemachine component for that stage, or null if not present.</returns>
        public override CinemachineComponentBase GetCinemachineComponent(CinemachineCore.Stage stage)
        {
            UpdatePipelineCache();
            var i = (int)stage;
            return i >= 0 && i < m_Pipeline.Length ? m_Pipeline[i] : null;
        }
    }
}
