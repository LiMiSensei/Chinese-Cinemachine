#if !CINEMACHINE_NO_CM2_SUPPORT
using UnityEngine;
using System;
using Unity.Cinemachine.TargetTracking;

namespace Unity.Cinemachine
{
    /// <summary>
    /// This is a deprecated component.  Use CinemachineOrbitalFollow instead.
    /// </summary>
    [Obsolete("CinemachineTransposer has been deprecated. Use CinemachineFollow instead")]
    [AddComponentMenu("")] // Don't display in add component menu
    [SaveDuringPlay]
    [CameraPipeline(CinemachineCore.Stage.Body)]
    public class CinemachineTransposer : CinemachineComponentBase
    {
        /// <summary>解释目标偏移时使用的坐标系</summary>
        [Tooltip("解释目标偏移时使用的坐标系。这也用于设置摄像机的向上向量，该向量在瞄准摄像机时将保持不变。")]
        public BindingMode m_BindingMode = BindingMode.LockToTargetWithWorldUp;

        /// <summary>摄像机位置器将尝试保持与跟随目标的距离向量</summary>
        [Tooltip("摄像机位置器将尝试保持与跟随目标的距离向量")]
        public Vector3 m_FollowOffset = Vector3.back * 10f;

        /// <summary>摄像机尝试维持X轴偏移的积极程度。
        /// 较小的数值响应更快，能快速移动摄像机以保持目标的X轴偏移。
        /// 较大的数值会使摄像机响应更缓慢沉重。
        /// 在不同轴上使用不同设置可以产生广泛的摄像机行为</summary>
        [Range(0f, 20f)]
        [Tooltip("摄像机尝试维持X轴偏移的积极程度。较小的数值响应更快，能快速移动摄像机以保持目标的X轴偏移。较大的数值会使摄像机响应更缓慢沉重。在不同轴上使用不同设置可以产生广泛的摄像机行为。")]
        public float m_XDamping = 1f;

        /// <summary>摄像机尝试维持Y轴偏移的积极程度。
        /// 较小的数值响应更快，能快速移动摄像机以保持目标的Y轴偏移。
        /// 较大的数值会使摄像机响应更缓慢沉重。
        /// 在不同轴上使用不同设置可以产生广泛的摄像机行为</summary>
        [Range(0f, 20f)]
        [Tooltip("摄像机尝试维持Y轴偏移的积极程度。较小的数值响应更快，能快速移动摄像机以保持目标的Y轴偏移。较大的数值会使摄像机响应更缓慢沉重。在不同轴上使用不同设置可以产生广泛的摄像机行为。")]
        public float m_YDamping = 1f;

        /// <summary>摄像机尝试维持Z轴偏移的积极程度。
        /// 较小的数值响应更快，能快速移动摄像机以保持目标的Z轴偏移。
        /// 较大的数值会使摄像机响应更缓慢沉重。
        /// 在不同轴上使用不同设置可以产生广泛的摄像机行为</summary>
        [Range(0f, 20f)]
        [Tooltip("摄像机尝试维持Z轴偏移的积极程度。较小的数值响应更快，能快速移动摄像机以保持目标的Z轴偏移。较大的数值会使摄像机响应更缓慢沉重。在不同轴上使用不同设置可以产生广泛的摄像机行为。")]
        public float m_ZDamping = 1f;

        /// <summary>如何计算目标方向的角阻尼。
        /// 如果预期目标会有非常陡峭的俯仰角度（使用欧拉角可能导致万向节锁问题），请使用四元数。</summary>
        public AngularDampingMode m_AngularDampingMode = AngularDampingMode.Euler;

        /// <summary>摄像机尝试跟踪目标旋转的X角度（俯仰）的积极程度。
        /// 较小的数值响应更快。较大的数值会使摄像机响应更缓慢沉重。</summary>
        [Range(0f, 20f)]
        [Tooltip("摄像机尝试跟踪目标旋转的X角度（俯仰）的积极程度。较小的数值响应更快。较大的数值会使摄像机响应更缓慢沉重。")]
        public float m_PitchDamping = 0;

        /// <summary>摄像机尝试跟踪目标旋转的Y角度（偏航）的积极程度。
        /// 较小的数值响应更快。较大的数值会使摄像机响应更缓慢沉重。</summary>
        [Range(0f, 20f)]
        [Tooltip("摄像机尝试跟踪目标旋转的Y角度（偏航）的积极程度。较小的数值响应更快。较大的数值会使摄像机响应更缓慢沉重。")]
        public float m_YawDamping = 0;

        /// <summary>摄像机尝试跟踪目标旋转的Z角度（滚转）的积极程度。
        /// 较小的数值响应更快。较大的数值会使摄像机响应更缓慢沉重。</summary>
        [Range(0f, 20f)]
        [Tooltip("摄像机尝试跟踪目标旋转的Z角度（滚转）的积极程度。较小的数值响应更快。较大的数值会使摄像机响应更缓慢沉重。")]
        public float m_RollDamping = 0f;

        /// <summary>摄像机尝试跟踪目标方向的积极程度。
        /// 较小的数值响应更快。较大的数值会使摄像机响应更缓慢沉重。</summary>
        [Range(0f, 20f)]
        [Tooltip("摄像机尝试跟踪目标方向的积极程度。较小的数值响应更快。较大的数值会使摄像机响应更缓慢沉重。")]
        public float m_AngularDamping = 0f;

        /// <summary>
        /// Helper object that tracks the Follow target, with damping
        /// </summary>
        Tracker m_TargetTracker;

        /// <summary>Get the damping settings</summary>
        protected TrackerSettings TrackerSettings => new TrackerSettings
        {
            BindingMode = m_BindingMode,
            PositionDamping = new Vector3(m_XDamping, m_YDamping, m_ZDamping),
            RotationDamping = new Vector3(m_PitchDamping, m_YawDamping, m_RollDamping),
            AngularDampingMode = m_AngularDampingMode,
            QuaternionDamping = m_AngularDamping
        };

        /// <summary>Derived classes should call this from their OnValidate() implementation</summary>
        protected virtual void OnValidate()
        {
            m_FollowOffset = EffectiveOffset;
        }

        /// <summary>Hide the offset in int inspector.  Used by FreeLook.</summary>
        internal bool HideOffsetInInspector { get; set; }

        /// <summary>Get the target offset, with sanitization</summary>
        public Vector3 EffectiveOffset
        {
            get
            {
                Vector3 offset = m_FollowOffset;
                if (m_BindingMode == BindingMode.LazyFollow)
                {
                    offset.x = 0;
                    offset.z = -Mathf.Abs(offset.z);
                }
                return offset;
            }
        }

        /// <summary>True if component is enabled and has a valid Follow target</summary>
        public override bool IsValid { get { return enabled && FollowTarget != null; } }

        /// <summary>Get the Cinemachine Pipeline stage that this component implements.
        /// Always returns the Body stage</summary>
        public override CinemachineCore.Stage Stage { get { return CinemachineCore.Stage.Body; } }

        /// <summary>
        /// Report maximum damping time needed for this component.
        /// </summary>
        /// <returns>Highest damping setting in this component</returns>
        public override float GetMaxDampTime() => TrackerSettings.GetMaxDampTime();

        /// <summary>Positions the virtual camera according to the transposer rules.</summary>
        /// <param name="curState">The current camera state</param>
        /// <param name="deltaTime">Used for damping.  If less than 0, no damping is done.</param>
        public override void MutateCameraState(ref CameraState curState, float deltaTime)
        {
            m_TargetTracker.InitStateInfo(this, deltaTime, m_BindingMode, Vector3.zero, curState.ReferenceUp);
            if (IsValid)
            {
                Vector3 offset = EffectiveOffset;
                m_TargetTracker.TrackTarget(
                    this, deltaTime, curState.ReferenceUp, offset, TrackerSettings, Vector3.zero, ref curState,
                    out Vector3 pos, out Quaternion orient);
                offset = orient * offset;

                curState.ReferenceUp = orient * Vector3.up;

                // Respect minimum target distance on XZ plane
                var targetPosition = FollowTargetPosition;
                pos += m_TargetTracker.GetOffsetForMinimumTargetDistance(
                    this, pos, offset, curState.RawOrientation * Vector3.forward,
                    curState.ReferenceUp, targetPosition);

                curState.RawPosition = pos + offset;
            }
        }

        /// <summary>This is called to notify the user that a target got warped,
        /// so that we can update its internal state to make the camera
        /// also warp seamlessly.</summary>
        /// <param name="target">The object that was warped</param>
        /// <param name="positionDelta">The amount the target's position changed</param>
        public override void OnTargetObjectWarped(Transform target, Vector3 positionDelta)
        {
            base.OnTargetObjectWarped(target, positionDelta);
            if (target == FollowTarget)
                m_TargetTracker.OnTargetObjectWarped(positionDelta);
        }

        /// <summary>
        /// Force the virtual camera to assume a given position and orientation
        /// </summary>
        /// <param name="pos">Worldspace position to take</param>
        /// <param name="rot">Worldspace orientation to take</param>
        public override void ForceCameraPosition(Vector3 pos, Quaternion rot)
        {
            base.ForceCameraPosition(pos, rot);
            var state = VcamState;
            state.RawPosition = pos;
            state.RawOrientation = rot;
            state.PositionCorrection = Vector3.zero;
            state.OrientationCorrection = Quaternion.identity;
            m_TargetTracker.OnForceCameraPosition(this, m_BindingMode, Vector3.zero, ref state);
        }

        internal Quaternion GetReferenceOrientation(Vector3 up)
        {
            var state = VcamState;
            return m_TargetTracker.GetReferenceOrientation(this, m_BindingMode, Vector3.zero, up, ref state);
        }

        /// <summary>Internal API for the Inspector Editor, so it can draw a marker at the target</summary>
        /// <param name="worldUp">Current effective world up</param>
        /// <returns>The position of the Follow target</returns>
        internal virtual Vector3 GetTargetCameraPosition(Vector3 worldUp)
        {
            if (!IsValid)
                return Vector3.zero;
            var state = VcamState;
            return FollowTargetPosition + m_TargetTracker.GetReferenceOrientation(
                this, m_BindingMode, Vector3.zero, worldUp, ref state) * EffectiveOffset;
        }

        // Helper to upgrade to CM3
        internal void UpgradeToCm3(CinemachineFollow c)
        {
            c.FollowOffset = m_FollowOffset;
            c.TrackerSettings = TrackerSettings;
        }
    }
}
#endif
