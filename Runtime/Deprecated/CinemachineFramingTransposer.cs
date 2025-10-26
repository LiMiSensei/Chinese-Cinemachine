#if !CINEMACHINE_NO_CM2_SUPPORT
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
    /// <summary>
    /// This is a deprecated component.  Use CinemachinePositionComposer instead.
    /// </summary>
    [AddComponentMenu("")] // Don't display in add component menu
    [Obsolete("CinemachineFramingTransposer has been deprecated. Use CinemachinePositionComposer instead")]
    [CameraPipeline(CinemachineCore.Stage.Body)]
    [SaveDuringPlay]
    public class CinemachineFramingTransposer : CinemachineComponentBase
    {
        /// <summary>
        /// Offset from the Follow Target object (in target-local co-ordinates).  The camera will attempt to
        /// frame the point which is the target's position plus this offset.  Use it to correct for
        /// cases when the target's origin is not the point of interest for the camera.
        /// </summary>
        [Tooltip("相对于跟随目标对象的偏移量（位于目标局部坐标系中）。相机会尝试对“目标位置加上此偏移量”所得到的点进行取景。当目标的原点并非相机的关注点时，可使用此参数进行修正。")]
        public Vector3 m_TrackedObjectOffset;

        /// <summary>This setting will instruct the composer to adjust its target offset based
        /// on the motion of the target.  The composer will look at a point where it estimates
        /// the target will be this many seconds into the future.  Note that this setting is sensitive
        /// to noisy animation, and can amplify the noise, resulting in undesirable camera jitter.
        /// If the camera jitters unacceptably when the target is in motion, turn down this setting,
        /// or animate the target more smoothly.</summary>
        [Tooltip("此设置将指示构图器根据目标的运动调整其目标偏移量。构图器会看向一个它预估目标在未来若干秒后将到达的点。"
            + "请注意，此设置对嘈杂的动画较为敏感，可能会放大噪声，导致不必要的相机抖动。"
            + "如果目标运动时相机抖动得令人无法接受，请调低此设置，或让目标的动画更平滑一些。")]
        [Range(0f, 1f)]
        [Space]
        public float m_LookaheadTime = 0;

        /// <summary>Controls the smoothness of the lookahead algorithm.  Larger values smooth out
        /// jittery predictions and also increase prediction lag</summary>
        [Tooltip("控制前瞻算法的平滑度。数值越大，越能消除抖动性预测，但同时也会增加预测延迟。")]
        [Range(0, 30)]
        public float m_LookaheadSmoothing = 0;

        /// <summary>If checked, movement along the Y axis will be ignored for lookahead calculations</summary>
        [Tooltip("如果勾选，前瞻计算将忽略沿Y轴的移动。")]
        public bool m_LookaheadIgnoreY;

        /// <summary>How aggressively the camera tries to maintain the offset in the X-axis.
        /// Small numbers are more responsive, rapidly translating the camera to keep the target's
        /// x-axis offset.  Larger numbers give a more heavy slowly responding camera.
        /// Using different settings per axis can yield a wide range of camera behaviors</summary>
        [Space]
        [Range(0f, 20f)]
        [Tooltip("相机在X轴方向上维持偏移量的积极程度。"
        +"数值越小，响应越灵敏，相机会快速平移以保持目标的X轴偏移；数值越大，相机响应越迟缓、显得更“沉重”。"
        +"针对不同轴使用不同的设置，可实现多种不同的相机行为效果。")]
        public float m_XDamping = 1f;

        /// <summary>How aggressively the camera tries to maintain the offset in the Y-axis.
        /// Small numbers are more responsive, rapidly translating the camera to keep the target's
        /// y-axis offset.  Larger numbers give a more heavy slowly responding camera.
        /// Using different settings per axis can yield a wide range of camera behaviors</summary>
        [Range(0f, 20f)]
        [Tooltip("相机在 Y 轴方向上维持偏移量的积极程度。"
        + "数值越小，响应越灵敏，相机会快速平移以保持目标的 Y 轴偏移；数值越大，相机响应越迟缓、显得更 “沉重”。"
        +"针对不同轴使用不同的设置，可实现多种不同的相机行为效果。")]
        public float m_YDamping = 1f;

        /// <summary>How aggressively the camera tries to maintain the offset in the Z-axis.
        /// Small numbers are more responsive, rapidly translating the camera to keep the
        /// target's z-axis offset.  Larger numbers give a more heavy slowly responding camera.
        /// Using different settings per axis can yield a wide range of camera behaviors</summary>
        [Range(0f, 20f)]
        [Tooltip("相机在 Z 轴方向上维持偏移量的积极程度。"
        + "数值越小，响应越灵敏，相机会快速平移以保持目标的 Z 轴偏移；数值越大，相机响应越迟缓、显得更 “沉重”。"
        + "针对不同轴使用不同的设置，可实现多种不同的相机行为效果。")]
        public float m_ZDamping = 1f;

        /// <summary>如果启用，阻尼效果将仅适用于目标运动，而不适用于相机旋转变化。开启此选项可在旋转发生变化时获得即时响应。</summary>
        [Tooltip("如果启用，阻尼效果将仅适用于目标运动，而不适用于相机旋转变化。开启此选项可在旋转发生变化时获得即时响应。")]
        public bool m_TargetMovementOnly = true;

        /// <summary>目标的水平屏幕位置。相机会移动以将被跟踪对象定位到此位置</summary>
        [Space]
        [Range(-0.5f, 1.5f)]
        [Tooltip("目标的水平屏幕位置。相机会移动以将被跟踪对象定位到此位置。")]
        public float m_ScreenX = 0.5f;

        /// <summary>目标的垂直屏幕位置。相机会移动以将被跟踪对象定位到此位置</summary>
        [Range(-0.5f, 1.5f)]
        [Tooltip("目标的垂直屏幕位置。相机会移动以将被跟踪对象定位到此位置。")]
        public float m_ScreenY = 0.5f;

        /// <summary>将保持的相机轴方向上与跟随目标的距离</summary>
        [Tooltip("将保持的相机轴方向上与跟随目标的距离")]
        public float m_CameraDistance = 10f;

        /// <summary>如果目标位于此位置范围内，相机将不会水平移动</summary>
        [Space]
        [Range(0f, 2f)]
        [Tooltip("如果目标位于此位置范围内，相机将不会水平移动。")]
        public float m_DeadZoneWidth = 0f;

        /// <summary>如果目标位于此位置范围内，相机将不会垂直移动</summary>
        [Range(0f, 2f)]
        [Tooltip("如果目标位于此位置范围内，相机将不会垂直移动。")]
        public float m_DeadZoneHeight = 0f;

        /// <summary>如果跟随目标在指定相机距离的此范围内，相机将不会沿其z轴移动</summary>
        [Tooltip("如果跟随目标在指定相机距离的此范围内，相机将不会沿其z轴移动")]
        [FormerlySerializedAs("m_DistanceDeadZoneSize")]
        public float m_DeadZoneDepth = 0;

        /// <summary>如果勾选，则软区域将无大小限制</summary>
        [Space]
        [Tooltip("如果勾选，则软区域将无大小限制。")]
        public bool m_UnlimitedSoftZone = false;

        /// <summary>当目标位于此区域内时，相机将根据阻尼速度逐渐移动以重新对准期望位置</summary>
        [Range(0f, 2f)]
        [Tooltip("当目标位于此区域内时，相机将根据阻尼速度逐渐水平移动以重新对准期望位置。")]
        public float m_SoftZoneWidth = 0.8f;

        /// <summary>当目标位于此区域内时，相机将根据阻尼速度逐渐移动以重新对准期望位置</summary>
        [Range(0f, 2f)]
        [Tooltip("当目标位于此区域内时，相机将根据阻尼速度逐渐垂直移动以重新对准期望位置。")]
        public float m_SoftZoneHeight = 0.8f;

        /// <summary>非零的偏移量将使目标位置远离软区域的中心</summary>
        [Range(-0.5f, 0.5f)]
        [Tooltip("非零的偏移量将使目标位置水平远离软区域的中心。")]
        public float m_BiasX = 0f;

        /// <summary>非零的偏移量将使目标位置远离软区域的中心</summary>
        [Range(-0.5f, 0.5f)]
        [Tooltip("非零的偏移量将使目标位置垂直远离软区域的中心。")]
        public float m_BiasY = 0f;

        /// <summary>当此相机激活时，强制目标居中于屏幕。
        /// 如果为false，则将目标限制在死区边缘</summary>
        [Tooltip("当此相机激活时，强制目标居中于屏幕。"
            + "如果为false，则将目标限制在死区边缘")]
        public bool m_CenterOnActivate = true;

        /// <summary>构图时要考虑哪些屏幕维度</summary>
        public enum FramingMode
        {
            /// <summary>仅考虑水平维度。垂直构图将被忽略。</summary>
            Horizontal,
            /// <summary>仅考虑垂直维度。水平构图将被忽略。</summary>
            Vertical,
            /// <summary>水平和垂直维度中较大的将主导，以获得最佳适配。</summary>
            HorizontalAndVertical,
            /// <summary>不进行任何构图调整</summary>
            None
        };

        /// <summary>构图时要考虑哪些屏幕维度</summary>
        [Space]
        [Tooltip("构图时要考虑哪些屏幕维度。可以是水平、垂直或两者兼有")]
        [FormerlySerializedAs("m_FramingMode")]
        public FramingMode m_GroupFramingMode = FramingMode.HorizontalAndVertical;

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

        /// <summary>目标的边界框应填充多少屏幕空间。</summary>
        [Tooltip("目标的边界框应占据此比例的屏幕空间。"
            + "1表示填充整个屏幕。0.5表示填充一半屏幕，以此类推。")]
        public float m_GroupFramingSize = 0.8f;

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


        /// <summary>Internal API for the inspector editor</summary>
        internal Rect SoftGuideRect
        {
            get
            {
                return new Rect(
                    m_ScreenX - m_DeadZoneWidth / 2, m_ScreenY - m_DeadZoneHeight / 2,
                    m_DeadZoneWidth, m_DeadZoneHeight);
            }
            set
            {
                m_DeadZoneWidth = Mathf.Clamp(value.width, 0, 2);
                m_DeadZoneHeight = Mathf.Clamp(value.height, 0, 2);
                m_ScreenX = Mathf.Clamp(value.x + m_DeadZoneWidth / 2, -0.5f,  1.5f);
                m_ScreenY = Mathf.Clamp(value.y + m_DeadZoneHeight / 2, -0.5f,  1.5f);
                m_SoftZoneWidth = Mathf.Max(m_SoftZoneWidth, m_DeadZoneWidth);
                m_SoftZoneHeight = Mathf.Max(m_SoftZoneHeight, m_DeadZoneHeight);
            }
        }

        /// <summary>Internal API for the inspector editor</summary>
        internal Rect HardGuideRect
        {
            get
            {
                Rect r = new Rect(
                        m_ScreenX - m_SoftZoneWidth / 2, m_ScreenY - m_SoftZoneHeight / 2,
                        m_SoftZoneWidth, m_SoftZoneHeight);
                r.position += new Vector2(
                        m_BiasX * (m_SoftZoneWidth - m_DeadZoneWidth),
                        m_BiasY * (m_SoftZoneHeight - m_DeadZoneHeight));
                return r;
            }
            set
            {
                m_SoftZoneWidth = Mathf.Clamp(value.width, 0, 2f);
                m_SoftZoneHeight = Mathf.Clamp(value.height, 0, 2f);
                m_DeadZoneWidth = Mathf.Min(m_DeadZoneWidth, m_SoftZoneWidth);
                m_DeadZoneHeight = Mathf.Min(m_DeadZoneHeight, m_SoftZoneHeight);
            }
        }

        void OnValidate()
        {
            m_CameraDistance = Mathf.Max(m_CameraDistance, kMinimumCameraDistance);
            m_DeadZoneDepth = Mathf.Max(m_DeadZoneDepth, 0);

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

        /// <summary>True if component is enabled and has a valid Follow target</summary>
        public override bool IsValid { get { return enabled && FollowTarget != null; } }

        /// <summary>Get the Cinemachine Pipeline stage that this component implements.
        /// Always returns the Body stage</summary>
        public override CinemachineCore.Stage Stage { get { return CinemachineCore.Stage.Body; } }

        /// <summary>FramingTransposer's algorithm tahes camera orientation as input,
        /// so even though it is a Body component, it must apply after Aim</summary>
        public override bool BodyAppliesAfterAim { get { return true; } }

        const float kMinimumCameraDistance = 0.01f;
        const float kMinimumGroupSize = 0.01f;

        /// <summary>State information for damping</summary>
        Vector3 m_PreviousCameraPosition = Vector3.zero;
        internal PositionPredictor m_Predictor = new PositionPredictor();

        /// <summary>Internal API for inspector</summary>
        public Vector3 TrackedPoint { get; private set; }

        /// <summary>This is called to notify the user that a target got warped,
        /// so that we can update its internal state to make the camera
        /// also warp seamlessy.</summary>
        /// <param name="target">The object that was warped</param>
        /// <param name="positionDelta">The amount the target's position changed</param>
        public override void OnTargetObjectWarped(Transform target, Vector3 positionDelta)
        {
            base.OnTargetObjectWarped(target, positionDelta);
            if (target == FollowTarget)
            {
                m_PreviousCameraPosition += positionDelta;
                m_Predictor.ApplyTransformDelta(positionDelta);
            }
        }

        /// <summary>
        /// Force the virtual camera to assume a given position and orientation
        /// </summary>
        /// <param name="pos">Worldspace pposition to take</param>
        /// <param name="rot">Worldspace orientation to take</param>
        public override void ForceCameraPosition(Vector3 pos, Quaternion rot)
        {
            base.ForceCameraPosition(pos, rot);
            m_PreviousCameraPosition = pos;
            m_prevRotation = rot;
        }

        /// <summary>
        /// Report maximum damping time needed for this component.
        /// </summary>
        /// <returns>Highest damping setting in this component</returns>
        public override float GetMaxDampTime()
        {
            return Mathf.Max(m_XDamping, Mathf.Max(m_YDamping, m_ZDamping));
        }

        /// <summary>Notification that this virtual camera is going live.
        /// Base class implementation does nothing.</summary>
        /// <param name="fromCam">The camera being deactivated.  May be null.</param>
        /// <param name="worldUp">Default world Up, set by the CinemachineBrain</param>
        /// <param name="deltaTime">Delta time for time-based effects (ignore if less than or equal to 0)</param>
        /// <returns>True if the vcam should do an internal update as a result of this call</returns>
        public override bool OnTransitionFromCamera(
            ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime)
        {
            if (fromCam != null
                && (VirtualCamera.State.BlendHint & CameraState.BlendHints.InheritPosition) != 0
                && !CinemachineCore.IsLiveInBlend(VirtualCamera))
            {
                m_PreviousCameraPosition = fromCam.State.RawPosition;
                m_prevRotation = fromCam.State.RawOrientation;
                m_InheritingPosition = true;
                return true;
            }
            return false;
        }

        bool m_InheritingPosition;

        // Convert from screen coords to normalized orthographic distance coords
        private Rect ScreenToOrtho(Rect rScreen, float orthoSize, float aspect)
        {
            Rect r = new Rect();
            r.yMax = 2 * orthoSize * ((1f-rScreen.yMin) - 0.5f);
            r.yMin = 2 * orthoSize * ((1f-rScreen.yMax) - 0.5f);
            r.xMin = 2 * orthoSize * aspect * (rScreen.xMin - 0.5f);
            r.xMax = 2 * orthoSize * aspect * (rScreen.xMax - 0.5f);
            return r;
        }

        private Vector3 OrthoOffsetToScreenBounds(Vector3 targetPos2D, Rect screenRect)
        {
            // Bring it to the edge of screenRect, if outside.  Leave it alone if inside.
            Vector3 delta = Vector3.zero;
            if (targetPos2D.x < screenRect.xMin)
                delta.x += targetPos2D.x - screenRect.xMin;
            if (targetPos2D.x > screenRect.xMax)
                delta.x += targetPos2D.x - screenRect.xMax;
            if (targetPos2D.y < screenRect.yMin)
                delta.y += targetPos2D.y - screenRect.yMin;
            if (targetPos2D.y > screenRect.yMax)
                delta.y += targetPos2D.y - screenRect.yMax;
            return delta;
        }

        float m_prevFOV; // State for frame damping
        Quaternion m_prevRotation;

        /// <summary>For editor visulaization of the calculated bounding box of the group</summary>
        public Bounds LastBounds { get; private set; }

        /// <summary>For editor visualization of the calculated bounding box of the group</summary>
        public Matrix4x4 LastBoundsMatrix { get; private set; }

        /// <summary>Positions the virtual camera according to the transposer rules.</summary>
        /// <param name="curState">The current camera state</param>
        /// <param name="deltaTime">Used for damping.  If less than 0, no damping is done.</param>
        public override void MutateCameraState(ref CameraState curState, float deltaTime)
        {
            LensSettings lens = curState.Lens;
            Vector3 followTargetPosition = FollowTargetPosition + (FollowTargetRotation * m_TrackedObjectOffset);
            bool previousStateIsValid = deltaTime >= 0 && VirtualCamera.PreviousStateIsValid;
            if (!previousStateIsValid || VirtualCamera.FollowTargetChanged)
                m_Predictor.Reset();
            if (!previousStateIsValid)
            {
                m_PreviousCameraPosition = curState.RawPosition;
                m_prevFOV = lens.Orthographic ? lens.OrthographicSize : lens.FieldOfView;
                m_prevRotation = curState.RawOrientation;
                if (!m_InheritingPosition && m_CenterOnActivate)
                {
                    m_PreviousCameraPosition = FollowTargetPosition
                        + (curState.RawOrientation * Vector3.back) * m_CameraDistance;
                }
            }
            if (!IsValid)
            {
                m_InheritingPosition = false;
                return;
            }

            var verticalFOV = lens.FieldOfView;

            // Compute group bounds and adjust follow target for group framing
            ICinemachineTargetGroup group = FollowTargetAsGroup;
            bool isGroupFraming = group != null && group.IsValid && m_GroupFramingMode != FramingMode.None && !group.IsEmpty;
            if (isGroupFraming)
                followTargetPosition = ComputeGroupBounds(group, ref curState);

            TrackedPoint = followTargetPosition;
            if (m_LookaheadTime > Epsilon)
            {
                m_Predictor.Smoothing = m_LookaheadSmoothing;
                m_Predictor.AddPosition(followTargetPosition, deltaTime);
                var delta = m_Predictor.PredictPositionDelta(m_LookaheadTime);
                if (m_LookaheadIgnoreY)
                    delta = delta.ProjectOntoPlane(curState.ReferenceUp);
                var p = followTargetPosition + delta;
                if (isGroupFraming)
                {
                    var b = LastBounds;
                    b.center += LastBoundsMatrix.MultiplyPoint3x4(delta);
                    LastBounds = b;
                }
                TrackedPoint = p;
            }

            if (!curState.HasLookAt())
                curState.ReferenceLookAt = followTargetPosition;

            // Adjust the desired depth for group framing
            float targetDistance = m_CameraDistance;
            bool isOrthographic = lens.Orthographic;
            float targetHeight = isGroupFraming ? GetTargetHeight(LastBounds.size / m_GroupFramingSize) : 0;
            targetHeight = Mathf.Max(targetHeight, kMinimumGroupSize);
            if (!isOrthographic && isGroupFraming)
            {
                // Adjust height for perspective - we want the height at the near surface
                float boundsDepth = LastBounds.extents.z;
                float z = LastBounds.center.z;
                if (z > boundsDepth)
                    targetHeight = Mathf.Lerp(0, targetHeight, (z - boundsDepth) / z);

                if (m_AdjustmentMode != AdjustmentMode.ZoomOnly)
                {
                    // What distance from near edge would be needed to get the adjusted
                    // target height, at the current FOV
                    targetDistance = targetHeight / (2f * Mathf.Tan(verticalFOV * Mathf.Deg2Rad / 2f));

                    // Clamp to respect min/max distance settings to the near surface of the bounds
                    targetDistance = Mathf.Clamp(targetDistance, m_MinimumDistance, m_MaximumDistance);

                    // Clamp to respect min/max camera movement
                    float targetDelta = targetDistance - m_CameraDistance;
                    targetDelta = Mathf.Clamp(targetDelta, -m_MaxDollyIn, m_MaxDollyOut);
                    targetDistance = m_CameraDistance + targetDelta;
                }
            }

            // Optionally allow undamped camera orientation change
            Quaternion localToWorld = curState.RawOrientation;
            if (previousStateIsValid && m_TargetMovementOnly)
            {
                var q = localToWorld * Quaternion.Inverse(m_prevRotation);
                m_PreviousCameraPosition = TrackedPoint + q * (m_PreviousCameraPosition - TrackedPoint);
            }
            m_prevRotation = localToWorld;

            // Work in camera-local space
            Vector3 camPosWorld = m_PreviousCameraPosition;
            Quaternion worldToLocal = Quaternion.Inverse(localToWorld);
            Vector3 cameraPos = worldToLocal * camPosWorld;
            Vector3 targetPos = (worldToLocal * TrackedPoint) - cameraPos;
            Vector3 lookAtPos = targetPos;

            // Move along camera z
            Vector3 cameraOffset = Vector3.zero;
            float cameraMin = Mathf.Max(kMinimumCameraDistance, targetDistance - m_DeadZoneDepth/2);
            float cameraMax = Mathf.Max(cameraMin, targetDistance + m_DeadZoneDepth/2);
            float targetZ = Mathf.Min(targetPos.z, lookAtPos.z);
            if (targetZ < cameraMin)
                cameraOffset.z = targetZ - cameraMin;
            if (targetZ > cameraMax)
                cameraOffset.z = targetZ - cameraMax;

            // Move along the XY plane
            float screenSize = lens.Orthographic
                ? lens.OrthographicSize
                : Mathf.Tan(0.5f * verticalFOV * Mathf.Deg2Rad) * (targetZ - cameraOffset.z);
            Rect softGuideOrtho = ScreenToOrtho(SoftGuideRect, screenSize, lens.Aspect);
            if (!previousStateIsValid)
            {
                // No damping or hard bounds, just snap to central bounds, skipping the soft zone
                Rect rect = softGuideOrtho;
                if (m_CenterOnActivate && !m_InheritingPosition)
                    rect = new Rect(rect.center, Vector2.zero); // Force to center
                cameraOffset += OrthoOffsetToScreenBounds(targetPos, rect);
            }
            else
            {
                // Move it through the soft zone, with damping
                cameraOffset += OrthoOffsetToScreenBounds(targetPos, softGuideOrtho);
                cameraOffset = VirtualCamera.DetachedFollowTargetDamp(
                    cameraOffset, new Vector3(m_XDamping, m_YDamping, m_ZDamping), deltaTime);

                // Make sure the real target (not the lookahead one) is still in the frame
                if (!m_UnlimitedSoftZone
                    && (deltaTime < 0 || VirtualCamera.FollowTargetAttachment > 1 - Epsilon))
                {
                    Rect hardGuideOrtho = ScreenToOrtho(HardGuideRect, screenSize, lens.Aspect);
                    var realTargetPos = (worldToLocal * followTargetPosition) - cameraPos;
                    cameraOffset += OrthoOffsetToScreenBounds(
                        realTargetPos - cameraOffset, hardGuideOrtho);
                }
            }
            curState.RawPosition = camPosWorld + localToWorld * cameraOffset;
            m_PreviousCameraPosition = curState.RawPosition;

            // Adjust lens for group framing
            if (isGroupFraming)
            {
                if (isOrthographic)
                {
                    targetHeight = Mathf.Clamp(targetHeight / 2, m_MinimumOrthoSize, m_MaximumOrthoSize);

                    // Apply Damping
                    if (previousStateIsValid)
                        targetHeight = m_prevFOV + VirtualCamera.DetachedFollowTargetDamp(
                            targetHeight - m_prevFOV, m_ZDamping, deltaTime);
                    m_prevFOV = targetHeight;

                    lens.OrthographicSize = Mathf.Clamp(targetHeight, m_MinimumOrthoSize, m_MaximumOrthoSize);
                    curState.Lens = lens;
                }
                else if (m_AdjustmentMode != AdjustmentMode.DollyOnly)
                {
                    var localTarget = Quaternion.Inverse(curState.RawOrientation)
                        * (followTargetPosition - curState.RawPosition);
                    float nearBoundsDistance = localTarget.z;
                    float targetFOV = 179;
                    if (nearBoundsDistance > Epsilon)
                        targetFOV = 2f * Mathf.Atan(targetHeight / (2 * nearBoundsDistance)) * Mathf.Rad2Deg;
                    targetFOV = Mathf.Clamp(targetFOV, m_MinimumFOV, m_MaximumFOV);

                    // ApplyDamping
                    if (previousStateIsValid)
                        targetFOV = m_prevFOV + VirtualCamera.DetachedFollowTargetDamp(
                            targetFOV - m_prevFOV, m_ZDamping, deltaTime);
                    m_prevFOV = targetFOV;

                    lens.FieldOfView = targetFOV;
                    curState.Lens = lens;
                }
            }
            m_InheritingPosition = false;
        }

        float GetTargetHeight(Vector2 boundsSize)
        {
            switch (m_GroupFramingMode)
            {
                case FramingMode.Horizontal:
                    return boundsSize.x / VcamState.Lens.Aspect;
                case FramingMode.Vertical:
                    return boundsSize.y;
                default:
                case FramingMode.HorizontalAndVertical:
                    return Mathf.Max(boundsSize.x / VcamState.Lens.Aspect, boundsSize.y);
            }
        }

        Vector3 ComputeGroupBounds(ICinemachineTargetGroup group, ref CameraState curState)
        {
            Vector3 cameraPos = curState.RawPosition;
            Vector3 fwd = curState.RawOrientation * Vector3.forward;

            // Get the bounding box from camera's direction in view space
            LastBoundsMatrix = Matrix4x4.TRS(cameraPos, curState.RawOrientation, Vector3.one);
            Bounds b = group.GetViewSpaceBoundingBox(LastBoundsMatrix, true);
            Vector3 groupCenter = LastBoundsMatrix.MultiplyPoint3x4(b.center);
            float boundsDepth = b.extents.z;
            if (!curState.Lens.Orthographic)
            {
                // Parallax might change bounds - refine
                float d = (Quaternion.Inverse(curState.RawOrientation) * (groupCenter - cameraPos)).z;
                cameraPos = groupCenter - fwd * (Mathf.Max(d, boundsDepth) + boundsDepth);

                // Will adjust cameraPos
                b = GetScreenSpaceGroupBoundingBox(group, ref cameraPos, curState.RawOrientation);
                LastBoundsMatrix = Matrix4x4.TRS(cameraPos, curState.RawOrientation, Vector3.one);
                groupCenter = LastBoundsMatrix.MultiplyPoint3x4(b.center);
            }
            LastBounds = b;
            return groupCenter - fwd * boundsDepth;
        }

        static Bounds GetScreenSpaceGroupBoundingBox(
            ICinemachineTargetGroup group, ref Vector3 pos, Quaternion orientation)
        {
            var observer = Matrix4x4.TRS(pos, orientation, Vector3.one);
            group.GetViewSpaceAngularBounds(observer, out var minAngles, out var maxAngles, out var zRange);
            var shift = (minAngles + maxAngles) / 2;

            var q = Quaternion.identity.ApplyCameraRotation(shift, Vector3.up);
            pos = q * new Vector3(0, 0, (zRange.y + zRange.x)/2);
            pos.z = 0;
            pos = observer.MultiplyPoint3x4(pos);
            observer = Matrix4x4.TRS(pos, orientation, Vector3.one);
            group.GetViewSpaceAngularBounds(observer, out minAngles, out maxAngles, out zRange);

            // For width and height (in camera space) of the bounding box, we use the values at the center of the box.
            // This is an arbitrary choice.  The gizmo drawer will take this into account when displaying
            // the frustum bounds of the group
            var d = zRange.y + zRange.x;
            Vector2 angles = new Vector2(89.5f, 89.5f);
            if (zRange.x > 0)
            {
                angles = Vector2.Max(maxAngles, UnityVectorExtensions.Abs(minAngles));
                angles = Vector2.Min(angles, new Vector2(89.5f, 89.5f));
            }
            angles *= Mathf.Deg2Rad;
            return new Bounds(
                new Vector3(0, 0, d/2),
                new Vector3(Mathf.Tan(angles.y) * d, Mathf.Tan(angles.x) * d, zRange.y - zRange.x));
        }

        // Helper to upgrade to CM3
        internal ScreenComposerSettings Composition
        {
            get => new ()
            {
                ScreenPosition = new Vector2(m_ScreenX, m_ScreenY) - new Vector2(0.5f, 0.5f),
                DeadZone = new () { Enabled = true, Size = new Vector2(m_DeadZoneWidth, m_DeadZoneHeight) },
                HardLimits = new ()
                {
                    Enabled = !m_UnlimitedSoftZone,
                    Size = new Vector2(m_SoftZoneWidth, m_SoftZoneHeight),
                    Offset = new Vector2(m_BiasX, m_BiasY) * 2
                }
            };
            set
            {
                m_ScreenX = value.ScreenPosition.x + 0.5f;
                m_ScreenY = value.ScreenPosition.y + 0.5f;
                m_DeadZoneWidth = value.DeadZone.Size.x;
                m_DeadZoneHeight = value.DeadZone.Size.y;
                m_SoftZoneWidth = value.HardLimits.Size.x;
                m_SoftZoneHeight = value.HardLimits.Size.y;
                m_BiasX = value.HardLimits.Offset.x / 2;
                m_BiasY = value.HardLimits.Offset.y / 2;
            }
        }

        // Helper to upgrade to CM3
        internal void UpgradeToCm3(CinemachinePositionComposer c)
        {
            c.TargetOffset = m_TrackedObjectOffset;
            c.Lookahead = new LookaheadSettings
            {
                Enabled = m_LookaheadTime > 0,
                Time = m_LookaheadTime,
                Smoothing = m_LookaheadSmoothing,
                IgnoreY = m_LookaheadIgnoreY
            };
            c.CameraDistance = m_CameraDistance;
            c.DeadZoneDepth = m_DeadZoneDepth;
            c.Damping = new Vector3(m_XDamping, m_YDamping, m_ZDamping);
            c.Composition = Composition;
            c.CenterOnActivate = m_CenterOnActivate;
        }

        // Helper to upgrade to CM3
        internal void UpgradeToCm3(CinemachineGroupFraming c)
        {
            c.FramingMode = (CinemachineGroupFraming.FramingModes)m_GroupFramingMode; // values are the same
            c.FramingSize = m_GroupFramingSize;
            c.Damping = m_ZDamping;
            c.SizeAdjustment = (CinemachineGroupFraming.SizeAdjustmentModes)m_AdjustmentMode; // values are the same
            c.LateralAdjustment = CinemachineGroupFraming.LateralAdjustmentModes.ChangePosition;
            c.DollyRange = new Vector2(-m_MaxDollyIn, m_MaxDollyOut);
            c.FovRange = new Vector2(m_MinimumFOV, m_MaximumFOV);
            c.OrthoSizeRange = new Vector2(m_MinimumOrthoSize, m_MaximumOrthoSize);
        }
    }
}
#endif
