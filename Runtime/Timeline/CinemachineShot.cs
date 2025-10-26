#if CINEMACHINE_TIMELINE

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Unity.Cinemachine
{
    /// <summary>
    /// Internal use only.  Not part of the public API.
    /// </summary>
    public sealed class CinemachineShot : PlayableAsset, IPropertyPreview
    {
        /// <summary>轨道上显示的名称。如果为空，将使用 CmCamera 的名称。</summary>
        [Tooltip("轨道上显示的名称。如果为空，将使用 CmCamera 的名称。")]
        public string DisplayName;

        /// <summary>用于此镜头的 Cinemachine 摄像机</summary>
        [Tooltip("用于此镜头的 Cinemachine 摄像机")]
        public ExposedReference<CinemachineVirtualCameraBase> VirtualCamera;

        /// <summary>PlayableAsset implementation</summary>
        /// <param name="graph"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<CinemachineShotPlayable>.Create(graph);
            playable.GetBehaviour().VirtualCamera = VirtualCamera.Resolve(graph.GetResolver());
            return playable;
        }

        /// <summary>IPropertyPreview implementation</summary>
        /// <param name="director"></param>
        /// <param name="driver"></param>
        public void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            driver.AddFromName<Transform>("m_LocalPosition.x");
            driver.AddFromName<Transform>("m_LocalPosition.y");
            driver.AddFromName<Transform>("m_LocalPosition.z");
            driver.AddFromName<Transform>("m_LocalRotation.x");
            driver.AddFromName<Transform>("m_LocalRotation.y");
            driver.AddFromName<Transform>("m_LocalRotation.z");
            driver.AddFromName<Transform>("m_LocalRotation.w");

            driver.AddFromName<Camera>("field of view");
            driver.AddFromName<Camera>("near clip plane");
            driver.AddFromName<Camera>("far clip plane");
        }
    }
}
#endif
