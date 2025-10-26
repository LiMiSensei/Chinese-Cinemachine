#if !CINEMACHINE_NO_CM2_SUPPORT
using System;
using UnityEngine;

namespace Unity.Cinemachine
{
    /// <summary>
    /// Third-person follower, with complex pivoting: horizontal about the origin,
    /// vertical about the shoulder.
    /// </summary>
    [Obsolete("Cinemachine3rdPersonFollow has been deprecated. Use CinemachineThirdPersonFollow instead")]
    [CameraPipeline(CinemachineCore.Stage.Body)]
    [RequiredTarget(RequiredTargetAttribute.RequiredTargets.Tracking)]
    [AddComponentMenu("")]
    [SaveDuringPlay]
    [HelpURL(Documentation.BaseURL + "manual/Cinemachine3rdPersonFollow.html")]
    public class Cinemachine3rdPersonFollow : CinemachineComponentBase
        , CinemachineFreeLookModifier.IModifierValueSource
        , CinemachineFreeLookModifier.IModifiablePositionDamping
        , CinemachineFreeLookModifier.IModifiableDistance
    {
        /// <summary>摄像机跟踪目标的响应程度。每个轴（摄像机局部空间）
        /// 都可以有自己的设置。数值是摄像机跟上目标新位置
        /// 所需的大致时间。较小的值会产生更刚性的
        /// 效果，较大的值会产生更柔软的效果。</summary>
        [Tooltip("摄像机跟踪目标的响应程度。每个轴（摄像机局部空间）"
        + "都可以有自己的设置。数值是摄像机跟上目标新位置"
        + "所需的大致时间。较小的值会产生更"
        + "刚性的效果，较大的值会产生更柔软的效果")]
        public Vector3 Damping;

        /// <summary>肩部枢轴相对于跟随目标原点的位置。
        /// 这个偏移量在目标局部空间中。</summary>
        [Header("支架设置")]
        [Tooltip("肩部枢轴相对于跟随目标原点的位置。"
            + "这个偏移量在目标局部空间中")]
        public Vector3 ShoulderOffset;

        /// <summary>手部相对于肩部的垂直偏移量。
        /// 手臂长度会影响摄像机垂直旋转时
        /// 跟随目标的屏幕位置。</summary>
        [Tooltip("手部相对于肩部的垂直偏移量。"
            + "手臂长度会影响摄像机垂直旋转时"
            + "跟随目标的屏幕位置")]
        public float VerticalArmLength;

        /// <summary>指定摄像机位于哪个肩部（左、右或中间）。</summary>
        [Tooltip("指定摄像机位于哪个肩部（左、右或中间）")]
        [Range(0, 1)]
        public float CameraSide;

        /// <summary>摄像机会被放置在手臂后方多远的距离。</summary>
        [Tooltip("摄像机会被放置在手臂后方多远的距离")]
        public float CameraDistance;

        #if CINEMACHINE_PHYSICS
        /// <summary>摄像机会避开这些层上的障碍物。</summary>
        [Header("障碍物设置")]
        [Tooltip("摄像机会避开这些层上的障碍物")]
        public LayerMask CameraCollisionFilter;

        /// <summary>
        /// 具有此标签的障碍物将被忽略。将此字段
        /// 设置为目标的标签是个好主意
        /// </summary>
        [TagField]
        [Tooltip("具有此标签的障碍物将被忽略。"
            + "将此字段设置为目标的标签是个好主意")]
        public string IgnoreTag = string.Empty;

        /// <summary>
        /// 指定摄像机可以靠近障碍物的距离
        /// </summary>
        [Tooltip("指定摄像机可以靠近障碍物的距离")]
        [Range(0, 1)]
        public float CameraRadius;

        /// <summary>
        /// 摄像机移动以校正遮挡的渐变程度。
        /// 数值越高，摄像机移动越平缓。
        /// </summary>
        [Range(0, 10)]
        [Tooltip("摄像机移动以校正遮挡的渐变程度。" +
            "数值越高，摄像机移动越平缓。")]
        public float DampingIntoCollision;

        /// <summary>
        /// 在被内置碰撞解决系统校正后，摄像机返回正常位置的渐变程度。
        /// 数值越高，摄像机返回正常位置越平缓。
        /// </summary>
        [Range(0, 10)]
        [Tooltip("在被内置碰撞解决系统校正后，摄像机返回正常位置的渐变程度。" +
            "数值越高，摄像机返回正常位置越平缓。")]
        public float DampingFromCollision;
#endif

        // State info
        Vector3 m_PreviousFollowTargetPosition;
        Vector3 m_DampingCorrection; // this is in local rig space
#if CINEMACHINE_PHYSICS
        float m_CamPosCollisionCorrection;
#endif

        void OnValidate()
        {
            CameraSide = Mathf.Clamp(CameraSide, -1.0f, 1.0f);
            Damping.x = Mathf.Max(0, Damping.x);
            Damping.y = Mathf.Max(0, Damping.y);
            Damping.z = Mathf.Max(0, Damping.z);
#if CINEMACHINE_PHYSICS
            CameraRadius = Mathf.Max(0.001f, CameraRadius);
            DampingIntoCollision = Mathf.Max(0, DampingIntoCollision);
            DampingFromCollision = Mathf.Max(0, DampingFromCollision);
#endif
        }

        void Reset()
        {
            ShoulderOffset = new Vector3(0.5f, -0.4f, 0.0f);
            VerticalArmLength = 0.4f;
            CameraSide = 1.0f;
            CameraDistance = 2.0f;
            Damping = new Vector3(0.1f, 0.5f, 0.3f);
#if CINEMACHINE_PHYSICS
            CameraCollisionFilter = 0;
            CameraRadius = 0.2f;
            DampingIntoCollision = 0;
            DampingFromCollision = 2f;
#endif
        }

        float CinemachineFreeLookModifier.IModifierValueSource.NormalizedModifierValue
        {
            get
            {
                var up = VirtualCamera.State.ReferenceUp;
                var rot = FollowTargetRotation;
                var a = Vector3.SignedAngle(rot * Vector3.up, up, rot * Vector3.right);
                return Mathf.Clamp(a, -90, 90) / -90;
            }
        }

        Vector3 CinemachineFreeLookModifier.IModifiablePositionDamping.PositionDamping
        {
            get => Damping;
            set => Damping = value;
        }

        float CinemachineFreeLookModifier.IModifiableDistance.Distance
        {
            get => CameraDistance;
            set => CameraDistance = value;
        }

        /// <summary>True if component is enabled and has a Follow target defined</summary>
        public override bool IsValid => enabled && FollowTarget != null;

        /// <summary>Get the Cinemachine Pipeline stage that this component implements.
        /// Always returns the Aim stage</summary>
        public override CinemachineCore.Stage Stage { get { return CinemachineCore.Stage.Body; } }

#if CINEMACHINE_PHYSICS
        /// <summary>
        /// Report maximum damping time needed for this component.
        /// </summary>
        /// <returns>Highest damping setting in this component</returns>
        public override float GetMaxDampTime()
        {
            return Mathf.Max(
                Mathf.Max(DampingIntoCollision, DampingFromCollision),
                Mathf.Max(Damping.x, Mathf.Max(Damping.y, Damping.z)));
        }
#endif

        /// <summary>Orients the camera to match the Follow target's orientation</summary>
        /// <param name="curState">The current camera state</param>
        /// <param name="deltaTime">Elapsed time since last frame, for damping calculations.
        /// If negative, previous state is reset.</param>
        public override void MutateCameraState(ref CameraState curState, float deltaTime)
        {
            if (IsValid)
            {
                if (!VirtualCamera.PreviousStateIsValid)
                    deltaTime = -1;
                PositionCamera(ref curState, deltaTime);
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
                m_PreviousFollowTargetPosition += positionDelta;
        }

        void PositionCamera(ref CameraState curState, float deltaTime)
        {
            var up = curState.ReferenceUp;
            var targetPos = FollowTargetPosition;
            var targetRot = FollowTargetRotation;
            var targetForward = targetRot * Vector3.forward;
            var heading = GetHeading(targetRot, up);

            if (deltaTime < 0)
            {
                // No damping - reset damping state info
                m_DampingCorrection = Vector3.zero;
#if CINEMACHINE_PHYSICS
                m_CamPosCollisionCorrection = 0;
#endif
            }
            else
            {
                // Damping correction is applied to the shoulder offset - stretching the rig
                m_DampingCorrection += Quaternion.Inverse(heading) * (m_PreviousFollowTargetPosition - targetPos);
                m_DampingCorrection -= VirtualCamera.DetachedFollowTargetDamp(m_DampingCorrection, Damping, deltaTime);
            }

            m_PreviousFollowTargetPosition = targetPos;
            var root = targetPos;
            GetRawRigPositions(root, targetRot, heading, out _, out Vector3 hand);

            // Place the camera at the correct distance from the hand
            var camPos = hand - (targetForward * (CameraDistance - m_DampingCorrection.z));

#if CINEMACHINE_PHYSICS
            // Check if hand is colliding with something, if yes, then move the hand
            // closer to the player. The radius is slightly enlarged, to avoid problems
            // next to walls
            float dummy = 0;
            var collidedHand = ResolveCollisions(root, hand, -1, CameraRadius * 1.05f, ref dummy);
            camPos = ResolveCollisions(
                collidedHand, camPos, deltaTime, CameraRadius, ref m_CamPosCollisionCorrection);
#endif
            // Set state
            curState.RawPosition = camPos;
            curState.RawOrientation = targetRot; // not necessary, but left in to avoid breaking scenes that depend on this
        }

        /// <summary>
        /// Internal use only.  Public for the inspector gizmo
        /// </summary>
        /// <param name="root">Root of the rig.</param>
        /// <param name="shoulder">Shoulder of the rig.</param>
        /// <param name="hand">Hand of the rig.</param>
        public void GetRigPositions(out Vector3 root, out Vector3 shoulder, out Vector3 hand)
        {
            var up = VirtualCamera.State.ReferenceUp;
            var targetRot = FollowTargetRotation;
            var heading = GetHeading(targetRot, up);
            root = m_PreviousFollowTargetPosition;
            GetRawRigPositions(root, targetRot, heading, out shoulder, out hand);
#if CINEMACHINE_PHYSICS
            float dummy = 0;
            hand = ResolveCollisions(root, hand, -1, CameraRadius * 1.05f, ref dummy);
#endif
        }

        internal static Quaternion GetHeading(Quaternion targetRot, Vector3 up)
        {
            var targetForward = targetRot * Vector3.forward;
            var planeForward = Vector3.Cross(up, Vector3.Cross(targetForward.ProjectOntoPlane(up), up));
            if (planeForward.AlmostZero())
                planeForward = Vector3.Cross(targetRot * Vector3.right, up);
            return Quaternion.LookRotation(planeForward, up);
        }

        void GetRawRigPositions(
            Vector3 root, Quaternion targetRot, Quaternion heading,
            out Vector3 shoulder, out Vector3 hand)
        {
            var shoulderOffset = ShoulderOffset;
            shoulderOffset.x = Mathf.Lerp(-shoulderOffset.x, shoulderOffset.x, CameraSide);
            shoulderOffset.x += m_DampingCorrection.x;
            shoulderOffset.y += m_DampingCorrection.y;
            shoulder = root + heading * shoulderOffset;
            hand = shoulder + targetRot * new Vector3(0, VerticalArmLength, 0);
        }

#if CINEMACHINE_PHYSICS
        Vector3 ResolveCollisions(
            Vector3 root, Vector3 tip, float deltaTime,
            float cameraRadius, ref float collisionCorrection)
        {
            if (CameraCollisionFilter.value == 0)
            {
                return tip;
            }

            var dir = tip - root;
            var len = dir.magnitude;
            dir /= len;

            var result = tip;
            float desiredCorrection = 0;

            if (RuntimeUtility.SphereCastIgnoreTag(
                new Ray(root, dir), cameraRadius, out RaycastHit hitInfo,
                len, CameraCollisionFilter, IgnoreTag))
            {
                var desiredResult = hitInfo.point + hitInfo.normal * cameraRadius;
                desiredCorrection = (desiredResult - tip).magnitude;
            }

            collisionCorrection += deltaTime < 0 ? desiredCorrection - collisionCorrection : Damper.Damp(
                desiredCorrection - collisionCorrection,
                desiredCorrection > collisionCorrection ? DampingIntoCollision : DampingFromCollision,
                deltaTime);

            // Apply the correction
            if (collisionCorrection > Epsilon)
                result -= dir * collisionCorrection;

            return result;
        }
#endif


        // Helper to upgrade to CM3
        internal void UpgradeToCm3(CinemachineThirdPersonFollow c)
        {
            c.Damping = Damping;
            c.ShoulderOffset = ShoulderOffset;
            c.VerticalArmLength = VerticalArmLength;
            c.CameraDistance = CameraDistance;
            c.CameraSide = CameraSide;
#if CINEMACHINE_PHYSICS
            c.AvoidObstacles = new CinemachineThirdPersonFollow.ObstacleSettings
            {
                Enabled = true,
                CollisionFilter = CameraCollisionFilter,
                IgnoreTag = IgnoreTag,
                CameraRadius = CameraRadius,
                DampingFromCollision = DampingFromCollision,
                DampingIntoCollision = DampingIntoCollision,
            };
 #endif
        }
    }
}
#endif
