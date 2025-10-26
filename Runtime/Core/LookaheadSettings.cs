using UnityEngine;
using System;

namespace Unity.Cinemachine
{
    /// <summary>
    /// This structure holds settings for procedural lookahead.
    /// </summary>
    [Serializable]
    public struct LookaheadSettings
    {
        /// <summary>Enable or disable procedural lookahead</summary>
        public bool Enabled;

        /// <summary>预测未来多少秒的位置。
        /// 注意：此设置对带有噪声的动画很敏感，可能会放大噪声，
        /// 导致不期望的抖动。
        /// 如果目标运动时摄像机出现不可接受的抖动，请调低此设置，
        /// 或增加平滑设置，或使目标动画更加平滑。</summary>
        [Tooltip("预测未来多少秒的位置。"
            + "注意：此设置对带有噪声的动画很敏感，可能会放大噪声，导致"
            + "不期望的抖动。如果目标运动时摄像机出现不可接受的抖动，"
            + "请调低此设置，或使目标动画更加平滑。")]
        [Range(0f, 1f)]
        public float Time;

        /// <summary>控制前瞻算法的平滑度。较大的值可以平滑
        /// 抖动的预测结果，但也会增加预测延迟</summary>
        [Tooltip("控制前瞻算法的平滑度。较大的值可以平滑"
            + "抖动的预测结果，但也会增加预测延迟")]
        [Range(0, 30)]
        public float Smoothing;

        /// <summary>如果勾选，前瞻计算将忽略Y轴上的运动</summary>
        [Tooltip("如果勾选，前瞻计算将忽略Y轴上的运动")]
        public bool IgnoreY;

    }
}