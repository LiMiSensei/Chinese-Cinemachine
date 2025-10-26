using UnityEngine;

namespace Unity.Cinemachine
{
    /// <summary>
    /// This component will generate camera-specific activation and deactivation events.
    /// Add it to a Cinemachine Camera.
    /// </summary>
    [AddComponentMenu("Cinemachine/Helpers/Cinemachine Camera Events")]
    [SaveDuringPlay]
    [HelpURL(Documentation.BaseURL + "manual/CinemachineCameraEvents.html")]
    public class CinemachineCameraEvents : MonoBehaviour
    {
        /// <summary>
        /// 这是被监视事件的对象。如果为空且当前
        /// GameObject拥有CinemachineVirtualCameraBase组件，则将使用该组件。
        /// </summary>
        [Tooltip("这是被监视事件的对象。如果为空且当前"
            + "GameObject拥有CinemachineVirtualCameraBase组件，则将使用该组件。")]
        public CinemachineVirtualCameraBase EventTarget;

        /// <summary>
        /// 这是发出事件的对象。
        /// 如果涉及混合，将在混合开始时触发。
        /// </summary>
        [Space]
        [Tooltip("当虚拟摄像机在混合器上下文中变为活动状态时，此事件将触发。"
            + "如果涉及混合，则事件将在混合的第一帧触发。")]
        public CinemachineCore.CameraEvent CameraActivatedEvent = new ();

        /// <summary>
        /// 当虚拟摄像机停止处于活动状态时，此事件将触发。
        /// 如果涉及混合，则事件将在混合的最后一帧之后触发。
        /// </summary>
        [Tooltip("当虚拟摄像机停止处于活动状态时，此事件将触发。如果涉及混合，"
            + "则事件将在混合的最后一帧之后触发。")]
        public CinemachineCore.CameraEvent CameraDeactivatedEvent = new ();

        /// <summary>
        /// 当创建涉及此摄像机的混合时，此事件将触发。
        /// 处理程序可以修改混合中的任何设置，但摄像机本身除外。
        /// </summary>
        [Tooltip("当创建涉及此摄像机的混合时，此事件将触发。"
            + "处理程序可以修改混合中的任何设置，但摄像机本身除外。")]
        public CinemachineCore.BlendEvent BlendCreatedEvent = new ();

        /// <summary>
        /// 当虚拟摄像机完成混合进入时，此事件将触发。
        /// 如果混合长度为零，则不会触发。
        /// </summary>
        [Tooltip("当虚拟摄像机完成混合进入时，此事件将触发。"
            + "如果混合长度为零，则不会触发。")]
        public CinemachineCore.CameraEvent BlendFinishedEvent = new ();

        void OnEnable()
        {
            if (EventTarget == null)
                TryGetComponent(out EventTarget);
            if (EventTarget != null)
            {
                CinemachineCore.CameraActivatedEvent.AddListener(OnCameraActivated);
                CinemachineCore.CameraDeactivatedEvent.AddListener(OnCameraDeactivated);
                CinemachineCore.BlendCreatedEvent.AddListener(OnBlendCreated);
                CinemachineCore.BlendFinishedEvent.AddListener(OnBlendFinished);
            }
        }

        void OnDisable()
        {
            CinemachineCore.CameraActivatedEvent.RemoveListener(OnCameraActivated);
            CinemachineCore.CameraDeactivatedEvent.RemoveListener(OnCameraDeactivated);
            CinemachineCore.BlendCreatedEvent.RemoveListener(OnBlendCreated);
            CinemachineCore.BlendFinishedEvent.RemoveListener(OnBlendFinished);
        }

        void OnCameraActivated(ICinemachineCamera.ActivationEventParams evt)
        {
            if (evt.IncomingCamera == (ICinemachineCamera)EventTarget)
                CameraActivatedEvent.Invoke(evt.Origin, evt.IncomingCamera);
        }

        void OnBlendCreated(CinemachineCore.BlendEventParams evt)
        {
            if (evt.Blend.CamB == (ICinemachineCamera)EventTarget)
                BlendCreatedEvent.Invoke(evt);
        }

        void OnBlendFinished(ICinemachineMixer mixer, ICinemachineCamera cam)
        {
            if (cam == (ICinemachineCamera)EventTarget)
                BlendFinishedEvent.Invoke(mixer, cam);
        }

        void OnCameraDeactivated(ICinemachineMixer mixer, ICinemachineCamera cam)
        {
            if (cam == (ICinemachineCamera)EventTarget)
                CameraDeactivatedEvent.Invoke(mixer, cam);
        }
    }
}
