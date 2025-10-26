using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
    /// <summary>
    /// An add-on module for Cm Camera that adds a final offset to the camera
    /// </summary>
    [AddComponentMenu("Cinemachine/Procedural/Extensions/Cinemachine Camera Offset")]
    [ExecuteAlways]
    [SaveDuringPlay]
    [HelpURL(Documentation.BaseURL + "manual/CinemachineCameraOffset.html")]
    public class CinemachineCameraOffset : CinemachineExtension
    {
        /// <summary>
        /// 相机位置的偏移量（相机空间）
        /// </summary>
        [Tooltip("相机位置的偏移量（相机空间）")]
        [FormerlySerializedAs("m_Offset")]
        public Vector3 Offset = Vector3.zero;

        /// <summary>
        /// 何时应用偏移量
        /// </summary>
        [Tooltip("何时应用偏移量")]
        [FormerlySerializedAs("m_ApplyAfter")]
        public CinemachineCore.Stage ApplyAfter = CinemachineCore.Stage.Aim;

        /// <summary>
        /// 如果在瞄准后应用偏移，重新调整瞄准以尽可能保持
        /// 注视目标的屏幕位置
        /// </summary>
        [Tooltip("如果在瞄准后应用偏移，重新调整瞄准以尽可能保持"
            + "注视目标的屏幕位置")]
        [FormerlySerializedAs("m_PreserveComposition")]

        public bool PreserveComposition;

        private void Reset()
        {
            Offset = Vector3.zero;
            ApplyAfter = CinemachineCore.Stage.Aim;
            PreserveComposition = false;
        }

        /// <summary>
        /// Applies the specified offset to the camera state
        /// </summary>
        /// <param name="vcam">The virtual camera being processed</param>
        /// <param name="stage">The current pipeline stage</param>
        /// <param name="state">The current virtual camera state</param>
        /// <param name="deltaTime">The current applicable deltaTime</param>
        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage == ApplyAfter)
            {
                bool preserveAim = PreserveComposition
                    && state.HasLookAt() && stage > CinemachineCore.Stage.Body;

                Vector3 screenOffset = Vector2.zero;
                if (preserveAim)
                {
                    screenOffset = state.RawOrientation.GetCameraRotationToTarget(
                        state.ReferenceLookAt - state.GetCorrectedPosition(), state.ReferenceUp);
                }

                Vector3 offset = state.RawOrientation * Offset;
                state.PositionCorrection += offset;
                if (!preserveAim)
                    state.ReferenceLookAt += offset;
                else
                {
                    var q = Quaternion.LookRotation(
                        state.ReferenceLookAt - state.GetCorrectedPosition(), state.ReferenceUp);
                    q = q.ApplyCameraRotation(-screenOffset, state.ReferenceUp);
                    state.RawOrientation = q;
                }
            }
        }
    }
}
