using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
    /// <summary>
    /// This component will expose a non-cinemachine camera to the cinemachine system,
    /// allowing it to participate in blends.
    /// Just add it as a component alongside an existing Unity Camera component.
    /// </summary>
    [RequireComponent(typeof(Camera)), DisallowMultipleComponent]
    [AddComponentMenu("Cinemachine/Cinemachine External Camera")]
    [ExecuteAlways]
    [HelpURL(Documentation.BaseURL + "manual/CinemachineExternalCamera.html")]
    public class CinemachineExternalCamera : CinemachineVirtualCameraBase
    {
        /// <summary>与此CinemachineCamera之间转场的提示。提示可以组合，尽管并非所有组合都有意义。在提示冲突的情况下，Cinemachine将做出任意选择。</summary>
        [Tooltip("与此CinemachineCamera之间转场的提示。提示可以组合，尽管"
            + "并非所有组合都有意义。在提示冲突的情况下，Cinemachine将"
            + "做出任意选择。")]
        [FormerlySerializedAs("m_PositionBlending")]
        [FormerlySerializedAs("m_BlendHint")]
        public CinemachineCore.BlendHints BlendHint = 0;

        /// <summary>相机正在观察的对象。设置此值可能会改善与此相机之间转场的质量</summary>
        [Tooltip("相机正在观察的对象。设置此值可能会改善"
            + "与此相机之间转场的质量")]
        [NoSaveDuringPlay]
        [FormerlySerializedAs("m_LookAt")]
        public Transform LookAtTarget = null;

        Camera m_Camera;
        CameraState m_State = CameraState.Default;

        /// <summary>Get the CameraState, as we are able to construct one from the Unity Camera</summary>
        public override CameraState State => m_State;

        /// <summary>The object that the camera is looking at</summary>
        override public Transform LookAt
        {
            get => LookAtTarget;
            set => LookAtTarget = value;
        }

        /// <summary>This vcam defines no targets</summary>
        [HideInInspector]
        public override Transform Follow { get; set; }

        /// <summary>Internal use only.  Do not call this method</summary>
        /// <param name="worldUp">Effective world up</param>
        /// <param name="deltaTime">Effective deltaTime</param>
        public override void InternalUpdateCameraState(Vector3 worldUp, float deltaTime)
        {
            // Get the state from the camera
            if (m_Camera == null)
                TryGetComponent(out m_Camera);

            m_State = CameraState.Default;
            m_State.RawPosition = transform.position;
            m_State.RawOrientation = transform.rotation;
            m_State.ReferenceUp = m_State.RawOrientation * Vector3.up;
            if (m_Camera != null)
                m_State.Lens = LensSettings.FromCamera(m_Camera);

            if (LookAtTarget != null)
            {
                m_State.ReferenceLookAt = LookAtTarget.transform.position;
                Vector3 dir = m_State.ReferenceLookAt - State.RawPosition;
                if (!dir.AlmostZero())
                    m_State.ReferenceLookAt = m_State.RawPosition + Vector3.Project(
                        dir, State.RawOrientation * Vector3.forward);
            }
            m_State.BlendHint = (CameraState.BlendHints)BlendHint;
            InvokePostPipelineStageCallback(this, CinemachineCore.Stage.Finalize, ref m_State, deltaTime);
        }
    }
}
