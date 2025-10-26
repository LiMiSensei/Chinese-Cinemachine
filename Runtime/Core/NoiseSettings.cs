using UnityEngine;
using System;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
    /// <summary>
    /// This is an asset that defines a noise profile.  A noise profile is the
    /// shape of the noise signal as a function of time.  You can build arbitrarily complex shapes by
    /// combining different base perlin noise frequencies at different amplitudes.
    ///
    /// The frequencies and amplitudes should be chosen with care, to ensure an interesting
    /// noise quality that is not obviously repetitive.
    ///
    /// As a mathematical side-note, any arbitrary periodic curve can be broken down into a
    /// series of fixed-amplitude sine-waves added together.  This is called fourier decomposition,
    /// and is the basis of much signal processing.  It doesn't really have much to do with this
    /// asset, but it's super interesting!
    /// </summary>
    [HelpURL(Documentation.BaseURL + "manual/CinemachineNoiseProfiles.html")]
    public sealed class NoiseSettings : SignalSourceAsset
    {
        /// <summary>Describes the behaviour for a channel of noise</summary>
        [Serializable]
        public struct NoiseParams
        {
            /// <summary>此通道的噪声频率。幅度越大振动越快</summary>
            [Tooltip("此通道的噪声频率。幅度越大振动越快。")]
            public float Frequency;

            /// <summary>此通道的噪声振幅。数值越大振动幅度越高</summary>
            [Tooltip("此通道的噪声振幅。数值越大振动幅度越高。")]
            public float Amplitude;

            /// <summary>如果勾选，则振幅和频率将不会随机化</summary>
            [Tooltip("如果勾选，则振幅和频率将不会随机化。")]
            public bool Constant;

            /// <summary>在给定时间获取信号值，可添加时间偏移</summary>
            /// <param name="time">当前时间</param>
            /// <param name="timeOffset">要添加到当前时间的（未缩放）偏移量</param>
            /// <returns>指定时间的信号值</returns>
            public float GetValueAt(float time, float timeOffset)
            {
                float t = (Frequency * time) + timeOffset;
                if (Constant)
                    return Mathf.Cos(t * 2 * Mathf.PI) * Amplitude * 0.5f;
                return (Mathf.PerlinNoise(t, 0f) - 0.5f) * Amplitude;
            }
            }

            /// <summary>
            /// 包含摄像机所有3个基本轴向的噪声模块行为
            /// </summary>
            [Serializable]
            public struct TransformNoiseParams
            {
            /// <summary>X轴的噪声定义</summary>
            [Tooltip("X轴的噪声定义")]
            public NoiseParams X;
            /// <summary>Y轴的噪声定义</summary>
            [Tooltip("Y轴的噪声定义")]
            public NoiseParams Y;
            /// <summary>Z轴的噪声定义</summary>
            [Tooltip("Z轴的噪声定义")]
            public NoiseParams Z;

            /// <summary>在给定时间获取信号值，可添加时间偏移</summary>
            /// <param name="time">当前时间</param>
            /// <param name="timeOffsets">要添加到当前时间的（每通道）偏移量</param>
            /// <returns>指定时间的信号值</returns>
            public Vector3 GetValueAt(float time, Vector3 timeOffsets)
            {
                return new Vector3(
                    X.GetValueAt(time, timeOffsets.x),
                    Y.GetValueAt(time, timeOffsets.y),
                    Z.GetValueAt(time, timeOffsets.z));
            }
            }

            /// <summary>此<c>NoiseSettings</c>的位置噪声通道数组</summary>
            [Tooltip("这些是虚拟摄像机位置噪声通道。令人信服的噪声设置"
                + "通常混合低频、中频和高频，因此建议从大小为3开始")]
            [FormerlySerializedAs("m_Position")]
            public TransformNoiseParams[] PositionNoise = new TransformNoiseParams[0];

            /// <summary>此<c>NoiseSettings</c>的旋转噪声通道数组</summary>
            [Tooltip("这些是虚拟摄像机旋转噪声通道。令人信服的噪声"
                + "设置通常混合低频、中频和高频，因此建议从大小为3开始")]
            [FormerlySerializedAs("m_Orientation")]
            public TransformNoiseParams[] OrientationNoise = new TransformNoiseParams[0];


        /// <summary>Get the noise signal value at a specific time</summary>
        /// <param name="noiseParams">The parameters that define the noise function</param>
        /// <param name="time">The time at which to sample the noise function</param>
        /// <param name="timeOffsets">Start time offset for each channel</param>
        /// <returns>The 3-channel noise signal value at the specified time</returns>
        public static Vector3 GetCombinedFilterResults(
            TransformNoiseParams[] noiseParams, float time, Vector3 timeOffsets)
        {
            Vector3 pos = Vector3.zero;
            if (noiseParams != null)
            {
                for (int i = 0; i < noiseParams.Length; ++i)
                    pos += noiseParams[i].GetValueAt(time, timeOffsets);
            }
            return pos;
        }

        /// <summary>
        /// Returns the total length in seconds of the signal.
        /// Returns 0 for signals of indeterminate length.
        /// </summary>
        public override float SignalDuration { get { return 0; } }

        /// <summary>Interface for raw signal provider</summary>
        /// <param name="timeSinceSignalStart">Time at which to get signal value</param>
        /// <param name="pos">The position impulse signal</param>
        /// <param name="rot">The rotation impulse signal</param>
        public override void GetSignal(float timeSinceSignalStart, out Vector3 pos, out Quaternion rot)
        {
            pos = GetCombinedFilterResults(PositionNoise, timeSinceSignalStart, Vector3.zero);
            rot = Quaternion.Euler(GetCombinedFilterResults(OrientationNoise, timeSinceSignalStart, Vector3.zero));
        }

    }
}
