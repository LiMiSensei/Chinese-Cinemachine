using UnityEngine;

namespace Unity.Cinemachine
{
    /// <summary>
    /// This component will generate CinemachineBrain-specific events.
    /// </summary>
    [AddComponentMenu("Cinemachine/Helpers/Cinemachine Brain Events")]
    [SaveDuringPlay]
    [HelpURL(Documentation.BaseURL + "manual/CinemachineBrainEvents.html")]
    public class CinemachineBrainEvents : CinemachineMixerEventsBase
    {
        /// <summary>
        /// 这是发出事件的CinemachineBrain组件。如果为空且当前
        /// GameObject拥有CinemachineBrain组件，则将使用该组件。
        /// </summary>
        [Tooltip("这是发出事件的CinemachineBrain组件。如果为空且当前"
            + "GameObject拥有CinemachineBrain组件，则将使用该组件。")]
        public CinemachineBrain Brain;

        /// <summary>此事件将在brain更新其摄像机后触发。
        /// 只有在此组件附加到CinemachineBrain上时，此事件才会触发。</summary>
        [Tooltip("此事件将在brain更新其摄像机后触发。")]
        public CinemachineCore.BrainEvent BrainUpdatedEvent = new ();

        /// <summary>
        /// Get the object being monitored.  This is the object that will generate the events.
        /// </summary>
        /// <returns>The ICinemachineMixer object that is the origin of the events.</returns>
        protected override ICinemachineMixer GetMixer() => Brain;

        void OnEnable()
        {
            if (Brain == null)
                TryGetComponent(out Brain);
            if (Brain != null)
            {
                InstallHandlers(Brain);
                CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdated);
            }
        }

        void OnDisable()
        {
            UninstallHandlers();
            CinemachineCore.CameraUpdatedEvent.RemoveListener(OnCameraUpdated);
        }

        void OnCameraUpdated(CinemachineBrain brain)
        {
            if (brain == Brain)
                BrainUpdatedEvent.Invoke(brain);
        }
    }
}
