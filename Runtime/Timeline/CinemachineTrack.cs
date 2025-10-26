#if CINEMACHINE_TIMELINE

using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Unity.Cinemachine
{
    /// <summary>
    /// Timeline track for CinemachineCamera activation
    /// </summary>
    [Serializable]
    [TrackClipType(typeof(CinemachineShot))]
    [TrackBindingType(typeof(CinemachineBrain), TrackBindingFlags.None)]
    [TrackColor(0.53f, 0.0f, 0.08f)]
    public class CinemachineTrack : TrackAsset
    {
        [Tooltip("优先级控制此轨道相对于其他 Cinemachine 轨道的优先顺序。"
            + "优先级较高的轨道将覆盖优先级较低的轨道。如果两个"
            + "同时存在的轨道具有相同的优先级，则实例化时间较晚的轨道将"
            + "优先。轨道优先级与 Cinemachine 摄像机优先级无关。")]
        public int TrackPriority;

        /// <summary>
        /// TrackAsset implementation
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="go"></param>
        /// <param name="inputCount"></param>
        /// <returns></returns>
        public override Playable CreateTrackMixer(
            PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixer = ScriptPlayable<CinemachinePlayableMixer>.Create(graph);
            mixer.SetInputCount(inputCount);
            mixer.GetBehaviour().Priority = TrackPriority;
            return mixer;
        }
    }
}
#endif
