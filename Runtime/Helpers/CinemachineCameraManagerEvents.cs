using UnityEngine;

namespace Unity.Cinemachine
{
    /// <summary>
    /// This component will generate mixer-specific events.
    /// </summary>
    [AddComponentMenu("Cinemachine/Helpers/Cinemachine Camera Manager Events")]
    [SaveDuringPlay]
    [HelpURL(Documentation.BaseURL + "manual/CinemachineCameraManagerEvents.html")]
    public class CinemachineCameraManagerEvents : CinemachineMixerEventsBase
    {
        /// <summary>
        /// 这是发出事件的CinemachineBrain组件。如果为空且当前
        /// GameObject拥有CinemachineBrain组件，则将使用该组件。
        /// </summary>
        [Tooltip("这是发出事件的CinemachineCameraManager组件。如果为空且当前"
            + "GameObject拥有CinemachineCameraManager组件，则将使用该组件。")]
        public CinemachineCameraManagerBase CameraManager;

        /// <summary>
        /// Get the object being monitored.  This is the object that will generate the events.
        /// </summary>
        /// <returns>The ICinemachineMixer object that is the origin of the events.</returns>
        protected override ICinemachineMixer GetMixer() => CameraManager;

        void OnEnable()
        {
            if (CameraManager == null)
                TryGetComponent(out CameraManager);
            InstallHandlers(CameraManager);
        }

        void OnDisable()
        {
            UninstallHandlers();
        }
    }
}
