using UnityEngine;

namespace Unity.Cinemachine
{
    /// <summary>
    /// This is a base class for components that will generate mixer-specific events.
    /// </summary>
    [SaveDuringPlay]
    public abstract class CinemachineMixerEventsBase : MonoBehaviour
    {
        /// <summary>
        /// 当虚拟摄像机变为活动状态时，此事件将触发。
        /// 如果涉及混合，将在混合开始时触发。
        /// </summary>
        [Space]
        [Tooltip("当虚拟摄像机变为活动状态时，此事件将触发。如果涉及混合，"
            + "则事件将在混合的第一帧触发。")]
        public CinemachineCore.CameraEvent CameraActivatedEvent = new ();

        /// <summary>
        /// 当虚拟摄像机停止处于活动状态时，此事件将触发。
        /// 如果涉及混合，则事件将在混合的最后一帧之后触发。
        /// </summary>
        [Tooltip("当虚拟摄像机停止处于活动状态时，此事件将触发。如果涉及混合，"
            + "则事件将在混合的最后一帧之后触发。")]
        public CinemachineCore.CameraEvent CameraDeactivatedEvent = new ();

        /// <summary>
        /// 当在此Brain的根帧中创建混合时，此事件将触发。
        /// 处理程序可以修改混合中的任何设置，但摄像机本身除外。
        /// 注意：时间轴轨道不会生成这些事件。
        /// </summary>
        [Tooltip("当在此Brain的根帧中创建混合时，此事件将触发。"
            + "处理程序可以修改混合中的任何设置，但摄像机本身除外。"
            + "注意：时间轴轨道不会生成这些事件。")]
        public CinemachineCore.BlendEvent BlendCreatedEvent = new ();

        /// <summary>
        /// 当虚拟摄像机完成混合进入时，此事件将触发。
        /// 如果混合长度为零，则不会触发。
        /// </summary>
        [Tooltip("当虚拟摄像机完成混合进入时，此事件将触发。"
            + "如果混合长度为零，则不会触发。")]
        public CinemachineCore.CameraEvent BlendFinishedEvent = new ();

        /// <summary>
        /// 当发生摄像机剪切时触发此事件。摄像机剪切是指
        /// 具有零长度混合的摄像机激活。
        /// </summary>
        [Tooltip("当发生摄像机剪切时触发此事件。摄像机剪切是指"
            + "具有零长度混合的摄像机激活。")]
        public CinemachineCore.CameraEvent CameraCutEvent = new ();

        /// <summary>
        /// Get the object being monitored.  This is the object that will generate the events.
        /// </summary>
        /// <returns>The ICinemachineMixer object that is the origin of the events.</returns>
        protected abstract ICinemachineMixer GetMixer();

        /// <summary>Install the event handlers.  Call this from OnEnable().</summary>
        /// <param name="mixer">The mixer object to monitor.</param>
        protected void InstallHandlers(ICinemachineMixer mixer)
        {
            if (mixer != null)
            {
                CinemachineCore.CameraActivatedEvent.AddListener(OnCameraActivated);
                CinemachineCore.CameraDeactivatedEvent.AddListener(OnCameraDeactivated);
                CinemachineCore.BlendCreatedEvent.AddListener(OnBlendCreated);
                CinemachineCore.BlendFinishedEvent.AddListener(OnBlendFinished);
            }
        }

        /// <summary>Uninstall the event handlers.  Call the from OnDisable().</summary>
        protected void UninstallHandlers()
        {
            CinemachineCore.CameraActivatedEvent.RemoveListener(OnCameraActivated);
            CinemachineCore.CameraDeactivatedEvent.RemoveListener(OnCameraDeactivated);
            CinemachineCore.BlendCreatedEvent.RemoveListener(OnBlendCreated);
            CinemachineCore.BlendFinishedEvent.RemoveListener(OnBlendFinished);
        }

        void OnCameraActivated(ICinemachineCamera.ActivationEventParams evt)
        {
            var mixer = GetMixer();
            if (evt.Origin == mixer)
            {
                CameraActivatedEvent.Invoke(mixer, evt.IncomingCamera);
                if (evt.IsCut)
                    CameraCutEvent.Invoke(mixer, evt.IncomingCamera);
            }
        }

        void OnCameraDeactivated(ICinemachineMixer mixer, ICinemachineCamera cam)
        {
            if (mixer == GetMixer())
                CameraDeactivatedEvent.Invoke(mixer, cam);
        }

        void OnBlendCreated(CinemachineCore.BlendEventParams evt)
        {
            if (evt.Origin == GetMixer())
                BlendCreatedEvent.Invoke(evt);
        }

        void OnBlendFinished(ICinemachineMixer mixer, ICinemachineCamera cam)
        {
            if (mixer == GetMixer())
                BlendFinishedEvent.Invoke(mixer, cam);
        }
    }
}
