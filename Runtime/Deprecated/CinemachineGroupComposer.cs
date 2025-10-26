#if !CINEMACHINE_NO_CM2_SUPPORT
using UnityEngine;

namespace Unity.Cinemachine
{
    /// <summary>
    /// This is a deprecated component.  Use CinemachineRotationComposer and CinemachineGroupFraming instead.
    /// </summary>
    [System.Obsolete("CinemachineGroupTransposer has been deprecated. Use CinemachineRotationComposer and CinemachineGroupFraming instead")]
    [AddComponentMenu("")] // Don't display in add component menu
    [SaveDuringPlay]
    [CameraPipeline(CinemachineCore.Stage.Aim)]
    public class CinemachineGroupComposer : CinemachineComposer
    {
        /// <summary>目标的边界框应填充多少屏幕空间。</summary>
        [Tooltip("目标的边界框应占据此比例的屏幕空间。"
            + "1表示填充整个屏幕。0.5表示填充一半屏幕，以此类推。")]
        public float m_GroupFramingSize = 0.8f;

        /// <summary>构图时要考虑哪些屏幕维度</summary>
        public enum FramingMode
        {
            /// <summary>仅考虑水平维度。垂直构图将被忽略。</summary>
            Horizontal,
            /// <summary>仅考虑垂直维度。水平构图将被忽略。</summary>
            Vertical,
            /// <summary>水平和垂直维度中较大的将主导，以获得最佳适配。</summary>
            HorizontalAndVertical
        };

        /// <summary>构图时要考虑哪些屏幕维度</summary>
        [Tooltip("构图时要考虑哪些屏幕维度。可以是水平、垂直或两者兼有")]
        public FramingMode m_FramingMode = FramingMode.HorizontalAndVertical;

        /// <summary>相机尝试构图的积极程度。
        /// 较小的数值响应更灵敏</summary>
        [Range(0, 20)]
        [Tooltip("相机尝试构图的积极程度。较小的数值响应更灵敏，"
            + "能快速调整相机以保持目标在画面中。较大的数值会使相机响应更缓慢、更沉重。")]
        public float m_FrameDamping = 2f;

        /// <summary>如何调整相机以获得期望的构图</summary>
        public enum AdjustmentMode
        {
            /// <summary>不移动相机，仅调整FOV。</summary>
            ZoomOnly,
            /// <summary>仅移动相机，不改变FOV。</summary>
            DollyOnly,
            /// <summary>在允许的范围内尽可能移动相机，然后
            /// 如有必要调整FOV以完成拍摄。</summary>
            DollyThenZoom
        };

        /// <summary>如何调整相机以获得期望的构图</summary>
        [Tooltip("如何调整相机以获得期望的构图。您可以缩放、推拉或两者同时进行。")]
        public AdjustmentMode m_AdjustmentMode = AdjustmentMode.ZoomOnly;

        /// <summary>相机可以离目标多近？</summary>
        [Tooltip("此行为允许相机向目标移动的最大距离。")]
        public float m_MaxDollyIn = 5000f;

        /// <summary>相机可以离目标多远？</summary>
        [Tooltip("此行为允许相机远离目标的最大距离。")]
        public float m_MaxDollyOut = 5000f;

        /// <summary>设置此项以限制相机可以离目标多近</summary>
        [Tooltip("设置此项以限制相机可以离目标多近。")]
        public float m_MinimumDistance = 1;

        /// <summary>设置此项以限制相机可以离目标多远</summary>
        [Tooltip("设置此项以限制相机可以离目标多远。")]
        public float m_MaximumDistance = 5000f;

        /// <summary>如果调整FOV，将不会将FOV设置低于此值</summary>
        [Range(1, 179)]
        [Tooltip("如果调整FOV，将不会将FOV设置低于此值。")]
        public float m_MinimumFOV = 3;

        /// <summary>如果调整FOV，将不会将FOV设置高于此值</summary>
        [Range(1, 179)]
        [Tooltip("如果调整FOV，将不会将FOV设置高于此值。")]
        public float m_MaximumFOV = 60;

        /// <summary>如果调整正交尺寸，将不会将其设置低于此值</summary>
        [Tooltip("如果调整正交尺寸，将不会将其设置低于此值。")]
        public float m_MinimumOrthoSize = 1;

        /// <summary>如果调整正交尺寸，将不会将其设置高于此值</summary>
        [Tooltip("如果调整正交尺寸，将不会将其设置高于此值。")]
        public float m_MaximumOrthoSize = 5000;


        private void OnValidate()
        {
            m_GroupFramingSize = Mathf.Max(0.001f, m_GroupFramingSize);
            m_MaxDollyIn = Mathf.Max(0, m_MaxDollyIn);
            m_MaxDollyOut = Mathf.Max(0, m_MaxDollyOut);
            m_MinimumDistance = Mathf.Max(0, m_MinimumDistance);
            m_MaximumDistance = Mathf.Max(m_MinimumDistance, m_MaximumDistance);
            m_MinimumFOV = Mathf.Max(1, m_MinimumFOV);
            m_MaximumFOV = Mathf.Clamp(m_MaximumFOV, m_MinimumFOV, 179);
            m_MinimumOrthoSize = Mathf.Max(0.01f, m_MinimumOrthoSize);
            m_MaximumOrthoSize = Mathf.Max(m_MinimumOrthoSize, m_MaximumOrthoSize);
        }

        private void Reset()
        {
            m_GroupFramingSize = 0.8f;
            m_FramingMode = FramingMode.HorizontalAndVertical;
            m_FrameDamping = 2f;
            m_AdjustmentMode = AdjustmentMode.ZoomOnly;
            m_MaxDollyIn = 5000f;
            m_MaxDollyOut = 5000f;
            m_MinimumDistance = 1;
            m_MaximumDistance = 5000f;
            m_MinimumFOV = 3;
            m_MaximumFOV = 60;
            m_MinimumOrthoSize = 1;
            m_MaximumOrthoSize = 5000;
        }

        // State for damping
        float m_prevFramingDistance;
        float m_prevFOV;

        /// <summary>For editor visulaization of the calculated bounding box of the group</summary>
        public Bounds LastBounds { get; private set; }

        /// <summary>For editor visualization of the calculated bounding box of the group</summary>
        public Matrix4x4 LastBoundsMatrix { get; private set; }

        /// <summary>
        /// Report maximum damping time needed for this component.
        /// </summary>
        /// <returns>Highest damping setting in this component</returns>
        public override float GetMaxDampTime()
        {
            return Mathf.Max(base.GetMaxDampTime(), m_FrameDamping);
        }

        /// <summary>Applies the composer rules and orients the camera accordingly</summary>
        /// <param name="curState">The current camera state</param>
        /// <param name="deltaTime">Used for calculating damping.  If less than
        /// zero, then target will snap to the center of the dead zone.</param>
        public override void MutateCameraState(ref CameraState curState, float deltaTime)
        {
            // Can't do anything without a group to look at
            ICinemachineTargetGroup group = LookAtTargetAsGroup;
            if (group == null || !group.IsValid)
            {
                base.MutateCameraState(ref curState, deltaTime);
                return;
            }

            if (!IsValid || !curState.HasLookAt())
            {
                m_prevFramingDistance = 0;
                m_prevFOV = 0;
                return;
            }

            bool isOrthographic = curState.Lens.Orthographic;
            bool canMoveCamera = !isOrthographic && m_AdjustmentMode != AdjustmentMode.ZoomOnly;

            // Get the bounding box from camera's POV in view space
            Vector3 up = curState.ReferenceUp;
            var cameraPos = curState.RawPosition;
            BoundingSphere s = group.Sphere;
            Vector3 groupCenter = s.position;
            Vector3 fwd = groupCenter - cameraPos;
            float d = fwd.magnitude;
            if (d < Epsilon)
                return;  // navel-gazing, get outa here

            // Approximate looking at the group center
            fwd /= d;
            LastBoundsMatrix = Matrix4x4.TRS(
                cameraPos, Quaternion.LookRotation(fwd, up), Vector3.one);

            // Correction for the actual center
            Bounds b;
            if (isOrthographic)
            {
                b = group.GetViewSpaceBoundingBox(LastBoundsMatrix, true);
                groupCenter = LastBoundsMatrix.MultiplyPoint3x4(b.center);
                fwd = (groupCenter - cameraPos).normalized;
                LastBoundsMatrix = Matrix4x4.TRS(cameraPos, Quaternion.LookRotation(fwd, up), Vector3.one);
                b = group.GetViewSpaceBoundingBox(LastBoundsMatrix, true);
                LastBounds = b;
            }
            else
            {
                b = GetScreenSpaceGroupBoundingBox(group, LastBoundsMatrix, out fwd);
                LastBoundsMatrix = Matrix4x4.TRS(cameraPos, Quaternion.LookRotation(fwd, up), Vector3.one);
                LastBounds = b;
                groupCenter = cameraPos + fwd * b.center.z;
            }

            // Adjust bounds for framing size, and get target height
            float boundsDepth = b.extents.z;
            float targetHeight = GetTargetHeight(b.size / m_GroupFramingSize);

            if (isOrthographic)
            {
                targetHeight = Mathf.Clamp(targetHeight / 2, m_MinimumOrthoSize, m_MaximumOrthoSize);

                // ApplyDamping
                if (deltaTime >= 0 && VirtualCamera.PreviousStateIsValid)
                    targetHeight = m_prevFOV + VirtualCamera.DetachedLookAtTargetDamp(
                        targetHeight - m_prevFOV, m_FrameDamping, deltaTime);
                m_prevFOV = targetHeight;

                LensSettings lens = curState.Lens;
                lens.OrthographicSize = Mathf.Clamp(targetHeight, m_MinimumOrthoSize, m_MaximumOrthoSize);
                curState.Lens = lens;
            }
            else
            {
                // Adjust height for perspective - we want the height at the near surface
                float z = b.center.z;
                if (z > boundsDepth)
                    targetHeight = Mathf.Lerp(0, targetHeight, (z - boundsDepth) / z);

                // Move the camera
                if (canMoveCamera)
                {
                    // What distance from near edge would be needed to get the adjusted
                    // target height, at the current FOV
                    float targetDistance = boundsDepth
                        + targetHeight / (2f * Mathf.Tan(curState.Lens.FieldOfView * Mathf.Deg2Rad / 2f));

                    // Clamp to respect min/max distance settings to the near surface of the bounds
                    targetDistance = Mathf.Clamp(
                        targetDistance, boundsDepth + m_MinimumDistance, boundsDepth + m_MaximumDistance);

                    // Clamp to respect min/max camera movement
                    float targetDelta = targetDistance - Vector3.Distance(curState.RawPosition, groupCenter);
                    targetDelta = Mathf.Clamp(targetDelta, -m_MaxDollyIn, m_MaxDollyOut);

                    // ApplyDamping
                    if (deltaTime >= 0 && VirtualCamera.PreviousStateIsValid)
                    {
                        float delta = targetDelta - m_prevFramingDistance;
                        delta = VirtualCamera.DetachedLookAtTargetDamp(delta, m_FrameDamping, deltaTime);
                        targetDelta = m_prevFramingDistance + delta;
                    }
                    m_prevFramingDistance = targetDelta;
                    curState.PositionCorrection -= fwd * targetDelta;
                    cameraPos -= fwd * targetDelta;
                }

                // Apply zoom
                if (m_AdjustmentMode != AdjustmentMode.DollyOnly)
                {
                    float nearBoundsDistance = (groupCenter - cameraPos).magnitude - boundsDepth;
                    float targetFOV = 179;
                    if (nearBoundsDistance > Epsilon)
                        targetFOV = 2f * Mathf.Atan(targetHeight / (2 * nearBoundsDistance)) * Mathf.Rad2Deg;
                    targetFOV = Mathf.Clamp(targetFOV, m_MinimumFOV, m_MaximumFOV);

                    // ApplyDamping
                    if (deltaTime >= 0 && m_prevFOV != 0 && VirtualCamera.PreviousStateIsValid)
                        targetFOV = m_prevFOV + VirtualCamera.DetachedLookAtTargetDamp(
                            targetFOV - m_prevFOV, m_FrameDamping, deltaTime);
                    m_prevFOV = targetFOV;

                    LensSettings lens = curState.Lens;
                    lens.FieldOfView = targetFOV;
                    curState.Lens = lens;
                }
            }
            // Now compose normally
            curState.ReferenceLookAt = GetLookAtPointAndSetTrackedPoint(
                groupCenter, curState.ReferenceUp, deltaTime);
            base.MutateCameraState(ref curState, deltaTime);
        }

        float GetTargetHeight(Vector2 boundsSize)
        {
            switch (m_FramingMode)
            {
                case FramingMode.Horizontal:
                    return Mathf.Max(Epsilon, boundsSize.x ) / VcamState.Lens.Aspect;
                case FramingMode.Vertical:
                    return Mathf.Max(Epsilon, boundsSize.y);
                default:
                case FramingMode.HorizontalAndVertical:
                    return Mathf.Max(
                        Mathf.Max(Epsilon, boundsSize.x) / VcamState.Lens.Aspect,
                        Mathf.Max(Epsilon, boundsSize.y));
            }
        }

        /// <param name="observer">Point of view</param>
        /// <param name="newFwd">New forward direction to use when interpreting the return value</param>
        /// <returns>Bounding box in a slightly rotated version of observer, as specified by newFwd</returns>
        static Bounds GetScreenSpaceGroupBoundingBox(
            ICinemachineTargetGroup group, Matrix4x4 observer, out Vector3 newFwd)
        {
            group.GetViewSpaceAngularBounds(observer, out var minAngles, out var maxAngles, out var zRange);
            var shift = (minAngles + maxAngles) / 2;

            newFwd = Quaternion.identity.ApplyCameraRotation(shift, Vector3.up) * Vector3.forward;
            newFwd = observer.MultiplyVector(newFwd);

            // For width and height (in camera space) of the bounding box, we use the values at the center of the box.
            // This is an arbitrary choice.  The gizmo drawer will take this into account when displaying
            // the frustum bounds of the group
            var d = zRange.y + zRange.x;
            var angles = Vector2.Min(maxAngles - shift, new Vector2(89.5f, 89.5f)) * Mathf.Deg2Rad;
            return new Bounds(
                new Vector3(0, 0, d/2),
                new Vector3(Mathf.Tan(angles.y) * d, Mathf.Tan(angles.x) * d, zRange.y - zRange.x));
        }

        // Helper to upgrade to CM3
        internal void UpgradeToCm3(CinemachineGroupFraming c)
        {
            c.FramingMode = (CinemachineGroupFraming.FramingModes)m_FramingMode; // values are the same
            c.FramingSize = m_GroupFramingSize;
            c.Damping = m_FrameDamping;
            c.SizeAdjustment = (CinemachineGroupFraming.SizeAdjustmentModes)m_AdjustmentMode; // values are the same
            c.LateralAdjustment = CinemachineGroupFraming.LateralAdjustmentModes.ChangeRotation;
            c.DollyRange = new Vector2(-m_MaxDollyIn, m_MaxDollyOut);
            c.FovRange = new Vector2(m_MinimumFOV, m_MaximumFOV);
            c.OrthoSizeRange = new Vector2(m_MinimumOrthoSize, m_MaximumOrthoSize);
        }
    }
}
#endif
