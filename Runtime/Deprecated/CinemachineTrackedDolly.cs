#if !CINEMACHINE_NO_CM2_SUPPORT
using UnityEngine;
using System;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
    /// <summary>
    /// This is a deprecated component.  Use CinemachineSplineDolly instead.
    /// </summary>
    [Obsolete("CinemachineTrackedDolly has been deprecated. Use CinemachineSplineDolly instead.")]
    [AddComponentMenu("")] // Don't display in add component menu
    [SaveDuringPlay]
    [CameraPipeline(CinemachineCore.Stage.Body)]
    public class CinemachineTrackedDolly : CinemachineComponentBase
    {
        /// <summary>摄像机将被约束的路径。此项必须为非空。</summary>
        [Tooltip("摄像机将被约束的路径。此项必须为非空。")]
        public CinemachinePathBase m_Path;

        /// <summary>摄像机将被放置在路径上的位置。
        /// 可以直接动画化此值，或通过自动轨道车功能自动设置，
        /// 以尽可能接近跟随目标。</summary>
        [Tooltip("摄像机将被放置在路径上的位置。可以直接动画化此值，或通过自动轨道车功能自动设置，以尽可能接近跟随目标。该值将根据位置单位设置进行解释。")]
        public float m_PathPosition;

        /// <summary>如何解释路径位置</summary>
        [Tooltip("如何解释路径位置。如果设置为路径单位，值如下：0代表路径上的第一个路径点，1代表第二个，依此类推。中间的值表示路径点之间的路径位置。如果设置为距离，则路径位置表示沿路径的距离。")]
        public CinemachinePathBase.PositionUnits m_PositionUnits = CinemachinePathBase.PositionUnits.PathUnits;

        /// <summary>相对于路径位置放置摄像机的位置。X轴垂直于路径，Y轴向上，Z轴平行于路径。</summary>
        [Tooltip("相对于路径位置放置摄像机的位置。X轴垂直于路径，Y轴向上，Z轴平行于路径。这允许摄像机从路径本身偏移（例如，如同在三脚架上）。")]
        public Vector3 m_PathOffset = Vector3.zero;

        /// <summary>摄像机尝试维持垂直于路径的偏移的积极程度。
        /// 较小的数值响应更快，能快速移动摄像机以保持目标的X轴偏移。
        /// 较大的数值会使摄像机响应更缓慢沉重。
        /// 在不同轴上使用不同设置可以产生广泛的摄像机行为</summary>
        [Range(0f, 20f)]
        [Tooltip("摄像机尝试维持垂直于路径方向的偏移的积极程度。较小的数值响应更快，能快速移动摄像机以保持目标的X轴偏移。较大的数值会使摄像机响应更缓慢沉重。在不同轴上使用不同设置可以产生广泛的摄像机行为。")]
        public float m_XDamping = 0f;

        /// <summary>摄像机尝试维持在路径局部向上方向的偏移的积极程度。
        /// 较小的数值响应更快，能快速移动摄像机以保持目标的Y轴偏移。
        /// 较大的数值会使摄像机响应更缓慢沉重。
        /// 在不同轴上使用不同设置可以产生广泛的摄像机行为</summary>
        [Range(0f, 20f)]
        [Tooltip("摄像机尝试维持在路径局部向上方向的偏移的积极程度。较小的数值响应更快，能快速移动摄像机以保持目标的Y轴偏移。较大的数值会使摄像机响应更缓慢沉重。在不同轴上使用不同设置可以产生广泛的摄像机行为。")]
        public float m_YDamping = 0f;

        /// <summary>摄像机尝试维持平行于路径的偏移的积极程度。
        /// 较小的数值响应更快，能快速移动摄像机以保持目标的Z轴偏移。
        /// 较大的数值会使摄像机响应更缓慢沉重。
        /// 在不同轴上使用不同设置可以产生广泛的摄像机行为</summary>
        [Range(0f, 20f)]
        [Tooltip("摄像机尝试维持平行于路径方向的偏移的积极程度。较小的数值响应更快，能快速移动摄像机以保持目标的Z轴偏移。较大的数值会使摄像机响应更缓慢沉重。在不同轴上使用不同设置可以产生广泛的摄像机行为。")]
        public float m_ZDamping = 1f;

        /// <summary>设置摄像机向上向量的不同方式</summary>
        public enum CameraUpMode
        {
            /// <summary>保持摄像机的向上向量不变。它将根据Brain的WorldUp进行设置。</summary>
            Default,
            /// <summary>从当前点的路径向上向量获取向上向量</summary>
            Path,
            /// <summary>从当前点的路径向上向量获取向上向量，但将滚转角归零</summary>
            PathNoRoll,
            /// <summary>从跟随目标的向上向量获取向上向量</summary>
            FollowTarget,
            /// <summary>从跟随目标的向上向量获取向上向量，但将滚转角归零</summary>
            FollowTargetNoRoll,
        };

        /// <summary>如何设置虚拟摄像机的向上向量。这将影响屏幕构图。</summary>
        [Tooltip("如何设置虚拟摄像机的向上向量。这将影响屏幕构图，因为摄像机瞄准行为总是会尝试尊重向上方向。")]
        public CameraUpMode m_CameraUp = CameraUpMode.Default;

        /// <summary>摄像机尝试跟踪目标旋转的X角度的积极程度。
        /// 较小的数值响应更快。较大的数值会使摄像机响应更缓慢沉重。</summary>
        [Range(0f, 20f)]
        [Tooltip("摄像机尝试跟踪目标旋转的X角度的积极程度。较小的数值响应更快。较大的数值会使摄像机响应更缓慢沉重。")]
        public float m_PitchDamping = 0;

        /// <summary>摄像机尝试跟踪目标旋转的Y角度的积极程度。
        /// 较小的数值响应更快。较大的数值会使摄像机响应更缓慢沉重。</summary>
        [Range(0f, 20f)]
        [Tooltip("摄像机尝试跟踪目标旋转的Y角度的积极程度。较小的数值响应更快。较大的数值会使摄像机响应更缓慢沉重。")]
        public float m_YawDamping = 0;

        /// <summary>摄像机尝试跟踪目标旋转的Z角度的积极程度。
        /// 较小的数值响应更快。较大的数值会使摄像机响应更缓慢沉重。</summary>
        [Range(0f, 20f)]
        [Tooltip("摄像机尝试跟踪目标旋转的Z角度的积极程度。较小的数值响应更快。较大的数值会使摄像机响应更缓慢沉重。")]
        public float m_RollDamping = 0f;

        /// <summary>控制自动轨道车的运行方式</summary>
        [Serializable]
        public struct AutoDolly
        {
            /// <summary>如果勾选，将启用自动轨道车功能，它会选择尽可能接近跟随目标的路径位置。</summary>
            [Tooltip("如果勾选，将启用自动轨道车功能，它会选择尽可能接近跟随目标的路径位置。注意：这可能对性能有显著影响")]
            public bool m_Enabled;

            /// <summary>以当前位置单位表示的偏移量，从路径上最接近点到跟随目标。</summary>
            [Tooltip("以当前位置单位表示的偏移量，从路径上最接近点到跟随目标")]
            public float m_PositionOffset;

            /// <summary>在当前位置两侧搜索的路径点数量。使用0表示整个路径。</summary>
            [Tooltip("在当前位置两侧搜索的路径点数量。使用0表示整个路径。")]
            public int m_SearchRadius;

            /// <summary>我们通过将路径段分成这么多直线片段来进行路径点之间的搜索。
            /// 数值越高，结果越精确，但性能会相应变慢。</summary>
            [FormerlySerializedAs("m_StepsPerSegment")]
            [Tooltip("我们通过将路径段分成这么多直线片段来进行路径点之间的搜索。数值越高，结果越精确，但性能会相应变慢。")]
            public int m_SearchResolution;

            /// <summary>使用特定字段值的构造函数</summary>
            /// <param name="enabled">是否启用自动轨道车</param>
            /// <param name="positionOffset">以当前位置单位表示的偏移量，从路径上最接近点到跟随目标</param>
            /// <param name="searchRadius">在当前位置两侧搜索的路径点数量</param>
            /// <param name="stepsPerSegment">我们通过将路径段分成这么多直线片段来进行路径点之间的搜索</param>
            public AutoDolly(bool enabled, float positionOffset, int searchRadius, int stepsPerSegment)
            {
                m_Enabled = enabled;
                m_PositionOffset = positionOffset;
                m_SearchRadius = searchRadius;
                m_SearchResolution = stepsPerSegment;
            }
        }

        /// <summary>控制自动轨道车的运行方式</summary>
        [Tooltip("控制自动轨道车的运行方式。使用此功能需要有一个跟随目标。")]
        public AutoDolly m_AutoDolly = new AutoDolly(false, 0, 2, 5);

        /// <summary>True if component is enabled and has a path</summary>
        public override bool IsValid { get { return enabled && m_Path != null; } }

        /// <summary>Get the Cinemachine Pipeline stage that this component implements.
        /// Always returns the Body stage</summary>
        public override CinemachineCore.Stage Stage { get { return CinemachineCore.Stage.Body; } }

        /// <summary>
        /// Report maximum damping time needed for this component.
        /// </summary>
        /// <returns>Highest damping setting in this component</returns>
        public override float GetMaxDampTime()
        {
            var d2 = AngularDamping;
            var a = Mathf.Max(m_XDamping, Mathf.Max(m_YDamping, m_ZDamping));
            var b = Mathf.Max(d2.x, Mathf.Max(d2.y, d2.z));
            return Mathf.Max(a, b);
        }

        /// <summary>Positions the virtual camera according to the transposer rules.</summary>
        /// <param name="curState">The current camera state</param>
        /// <param name="deltaTime">Used for damping.  If less that 0, no damping is done.</param>
        public override void MutateCameraState(ref CameraState curState, float deltaTime)
        {
            // Init previous frame state info
            if (deltaTime < 0 || !VirtualCamera.PreviousStateIsValid)
            {
                m_PreviousPathPosition = m_PathPosition;
                m_PreviousCameraPosition = curState.RawPosition;
                m_PreviousOrientation = curState.RawOrientation;
            }

            if (!IsValid)
                return;

            // Get the new ideal path base position
            if (m_AutoDolly.m_Enabled && FollowTarget != null)
            {
                float prevPos = m_Path.ToNativePathUnits(m_PreviousPathPosition, m_PositionUnits);
                // This works in path units
                m_PathPosition = m_Path.FindClosestPoint(
                    FollowTargetPosition,
                    Mathf.FloorToInt(prevPos),
                    (deltaTime < 0 || m_AutoDolly.m_SearchRadius <= 0)
                        ? -1 : m_AutoDolly.m_SearchRadius,
                    m_AutoDolly.m_SearchResolution);
                m_PathPosition = m_Path.FromPathNativeUnits(m_PathPosition, m_PositionUnits);

                // Apply the path position offset
                m_PathPosition += m_AutoDolly.m_PositionOffset;
            }
            float newPathPosition = m_PathPosition;

            if (deltaTime >= 0 && VirtualCamera.PreviousStateIsValid)
            {
                // Normalize previous position to find the shortest path
                float maxUnit = m_Path.MaxUnit(m_PositionUnits);
                if (maxUnit > 0)
                {
                    float prev = m_Path.StandardizeUnit(m_PreviousPathPosition, m_PositionUnits);
                    float next = m_Path.StandardizeUnit(newPathPosition, m_PositionUnits);
                    if (m_Path.Looped && Mathf.Abs(next - prev) > maxUnit / 2)
                    {
                        if (next > prev)
                            prev += maxUnit;
                        else
                            prev -= maxUnit;
                    }
                    m_PreviousPathPosition = prev;
                    newPathPosition = next;
                }

                // Apply damping along the path direction
                float offset = m_PreviousPathPosition - newPathPosition;
                offset = Damper.Damp(offset, m_ZDamping, deltaTime);
                newPathPosition = m_PreviousPathPosition - offset;
            }
            m_PreviousPathPosition = newPathPosition;
            Quaternion newPathOrientation = m_Path.EvaluateOrientationAtUnit(newPathPosition, m_PositionUnits);

            // Apply the offset to get the new camera position
            Vector3 newCameraPos = m_Path.EvaluatePositionAtUnit(newPathPosition, m_PositionUnits);
            Vector3 offsetX = newPathOrientation * Vector3.right;
            Vector3 offsetY = newPathOrientation * Vector3.up;
            Vector3 offsetZ = newPathOrientation * Vector3.forward;
            newCameraPos += m_PathOffset.x * offsetX;
            newCameraPos += m_PathOffset.y * offsetY;
            newCameraPos += m_PathOffset.z * offsetZ;

            // Apply damping to the remaining directions
            if (deltaTime >= 0 && VirtualCamera.PreviousStateIsValid)
            {
                Vector3 currentCameraPos = m_PreviousCameraPosition;
                Vector3 delta = (currentCameraPos - newCameraPos);
                Vector3 delta1 = Vector3.Dot(delta, offsetY) * offsetY;
                Vector3 delta0 = delta - delta1;
                delta0 = Damper.Damp(delta0, m_XDamping, deltaTime);
                delta1 = Damper.Damp(delta1, m_YDamping, deltaTime);
                newCameraPos = currentCameraPos - (delta0 + delta1);
            }
            curState.RawPosition = m_PreviousCameraPosition = newCameraPos;

            // Set the orientation and up
            Quaternion newOrientation
                = GetCameraOrientationAtPathPoint(newPathOrientation, curState.ReferenceUp);
            if (deltaTime >= 0 && VirtualCamera.PreviousStateIsValid)
            {
                Vector3 relative = (Quaternion.Inverse(m_PreviousOrientation)
                    * newOrientation).eulerAngles;
                for (int i = 0; i < 3; ++i)
                    if (relative[i] > 180)
                        relative[i] -= 360;
                relative = Damper.Damp(relative, AngularDamping, deltaTime);
                newOrientation = m_PreviousOrientation * Quaternion.Euler(relative);
            }
            m_PreviousOrientation = newOrientation;

            curState.RawOrientation = newOrientation;
            if (m_CameraUp != CameraUpMode.Default)
                curState.ReferenceUp = curState.RawOrientation * Vector3.up;
        }

        private Quaternion GetCameraOrientationAtPathPoint(Quaternion pathOrientation, Vector3 up)
        {
            switch (m_CameraUp)
            {
                default:
                case CameraUpMode.Default: break;
                case CameraUpMode.Path: return pathOrientation;
                case CameraUpMode.PathNoRoll:
                    return Quaternion.LookRotation(pathOrientation * Vector3.forward, up);
                case CameraUpMode.FollowTarget:
                    if (FollowTarget != null)
                        return FollowTargetRotation;
                    break;
                case CameraUpMode.FollowTargetNoRoll:
                    if (FollowTarget != null)
                        return Quaternion.LookRotation(FollowTargetRotation * Vector3.forward, up);
                    break;
            }
            return Quaternion.LookRotation(VirtualCamera.transform.rotation * Vector3.forward, up);
        }

        private Vector3 AngularDamping
        {
            get
            {
                switch (m_CameraUp)
                {
                    case CameraUpMode.PathNoRoll:
                    case CameraUpMode.FollowTargetNoRoll:
                        return new Vector3(m_PitchDamping, m_YawDamping, 0);
                    case CameraUpMode.Default:
                        return Vector3.zero;
                    default:
                        return new Vector3(m_PitchDamping, m_YawDamping, m_RollDamping);
                }
            }
        }

        private float m_PreviousPathPosition = 0;
        Quaternion m_PreviousOrientation = Quaternion.identity;
        private Vector3 m_PreviousCameraPosition = Vector3.zero;

        // Helper to upgrade to CM3
        internal void UpgradeToCm3(CinemachineSplineDolly c)
        {
            c.Damping.Enabled = true;
            c.Damping.Position = new Vector3(m_XDamping, m_YDamping, m_ZDamping);
            c.Damping.Angular = Mathf.Max(m_YawDamping, Mathf.Max(m_RollDamping, m_PitchDamping));
            c.CameraRotation = (CinemachineSplineDolly.RotationMode)m_CameraUp; // enum values match
            c.AutomaticDolly.Enabled = m_AutoDolly.m_Enabled;
            if (m_AutoDolly.m_Enabled)
            {
                c.AutomaticDolly.Method = new SplineAutoDolly.NearestPointToTarget
                {
                    PositionOffset = m_AutoDolly.m_PositionOffset,
                    SearchResolution = m_AutoDolly.m_SearchResolution,
                    SearchIteration = 2
                };
            }
            // set splineDolly spline reference
            if (m_Path != null)
                c.Spline = m_Path.GetComponent<UnityEngine.Splines.SplineContainer>();

            c.CameraPosition = m_PathPosition;
            switch (m_PositionUnits)
            {
                case CinemachinePathBase.PositionUnits.PathUnits:  c.PositionUnits = UnityEngine.Splines.PathIndexUnit.Knot; break;
                case CinemachinePathBase.PositionUnits.Distance:   c.PositionUnits = UnityEngine.Splines.PathIndexUnit.Distance; break;
                case CinemachinePathBase.PositionUnits.Normalized: c.PositionUnits = UnityEngine.Splines.PathIndexUnit.Normalized; break;
            }
            c.SplineOffset = m_PathOffset;
        }
    }
}
#endif
