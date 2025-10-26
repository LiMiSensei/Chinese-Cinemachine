using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Unity.Cinemachine
{
    /// <summary>
    /// As a part of the Cinemachine Pipeline implementing the Noise stage, this
    /// component adds Perlin Noise to the Camera state, in the Correction
    /// channel of the CameraState.
    ///
    /// The noise is created by using a predefined noise profile asset.  This defines the
    /// shape of the noise over time.  You can scale this in amplitude or in time, to produce
    /// a large family of different noises using the same profile.
    /// </summary>
    /// <seealso cref="NoiseSettings"/>
    [AddComponentMenu("Cinemachine/Procedural/Noise/Cinemachine Basic Multi Channel Perlin")]
    [SaveDuringPlay]
    [DisallowMultipleComponent]
    [CameraPipeline(CinemachineCore.Stage.Noise)]
    [HelpURL(Documentation.BaseURL + "manual/CinemachineBasicMultiChannelPerlin.html")]
    public class CinemachineBasicMultiChannelPerlin
        : CinemachineComponentBase, CinemachineFreeLookModifier.IModifiableNoise
    {
        [Header("Help：程序化组件-Noise-2 自然抖动效果\n可以设置相机震动，颠簸，呼吸感")]
        [Space(30)]
        /// <summary>
        /// Serialized property for referencing a NoiseSettings asset
        /// </summary>
        [Tooltip("包含噪声配置文件的资源。在此处定义频率和振幅，以创建独特的噪声特征。你可以自定义配置文件，也可以直接使用众多预设中的一个。")]
        [FormerlySerializedAs("m_Definition")]
        [FormerlySerializedAs("m_NoiseProfile")]
        public NoiseSettings NoiseProfile;

        /// <summary>
        /// When rotating the camera, offset the camera's pivot position by this much (camera space)
        /// </summary>
        [Tooltip("旋转相机时，以此数值偏移相机的旋转轴心位置（基于相机自身坐标系）。")]
        [FormerlySerializedAs("m_PivotOffset")]
        public Vector3 PivotOffset = Vector3.zero;

        /// <summary>
        /// Gain to apply to the amplitudes defined in the settings asset.
        /// </summary>
        [Tooltip("应用于 NoiseSettings 资源中定义的振幅的增益值。1 为正常状态。将此值设为 0 可完全消除噪声。")]
        [FormerlySerializedAs("m_AmplitudeGain")]
        public float AmplitudeGain = 1f;

        /// <summary>
        /// Scale factor to apply to the frequencies defined in the settings asset.
        /// </summary>
        [Tooltip("应用于 NoiseSettings 资源中定义的频率的缩放系数。1 为正常状态。数值越大，噪声抖动越剧烈。")]
        [FormerlySerializedAs("m_FrequencyGain")]
        public float FrequencyGain = 1f;

        private bool m_Initialized = false;
        private float m_NoiseTime = 0;

        [SerializeField, HideInInspector, NoSaveDuringPlay, FormerlySerializedAs("mNoiseOffsets")]
        private Vector3 m_NoiseOffsets = Vector3.zero;

        (float, float) CinemachineFreeLookModifier.IModifiableNoise.NoiseAmplitudeFrequency
        {
            get => (AmplitudeGain, FrequencyGain);
            set { AmplitudeGain = value.Item1; FrequencyGain = value.Item2; }
        }

        /// <summary>True if the component is valid, i.e. it has a noise definition and is enabled.</summary>
        public override bool IsValid { get => enabled && NoiseProfile != null; }

        /// <summary>Get the Cinemachine Pipeline stage that this component implements.
        /// Always returns the Noise stage</summary>
        public override CinemachineCore.Stage Stage { get { return CinemachineCore.Stage.Noise; } }

        /// <summary>Applies noise to the Correction channel of the CameraState if the
        /// delta time is greater than 0.  Otherwise, does nothing.</summary>
        /// <param name="curState">The current camera state</param>
        /// <param name="deltaTime">How much to advance the perlin noise generator.
        /// Noise is only applied if this value is greater than or equal to 0</param>
        public override void MutateCameraState(ref CameraState curState, float deltaTime)
        {
            if (!IsValid || deltaTime < 0)
            {
                m_Initialized = false;
                return;
            }

            if (!m_Initialized)
                Initialize();

            if (TargetPositionCache.CacheMode == TargetPositionCache.Mode.Playback
                    && TargetPositionCache.HasCurrentTime)
                m_NoiseTime = TargetPositionCache.CurrentTime * FrequencyGain;
            else
                m_NoiseTime += deltaTime * FrequencyGain;
            curState.PositionCorrection += curState.GetCorrectedOrientation() * NoiseSettings.GetCombinedFilterResults(
                    NoiseProfile.PositionNoise, m_NoiseTime, m_NoiseOffsets) * AmplitudeGain;
            Quaternion rotNoise = Quaternion.Euler(NoiseSettings.GetCombinedFilterResults(
                    NoiseProfile.OrientationNoise, m_NoiseTime, m_NoiseOffsets) * AmplitudeGain);
            if (PivotOffset != Vector3.zero)
            {
                Matrix4x4 m = Matrix4x4.Translate(-PivotOffset);
                m = Matrix4x4.Rotate(rotNoise) * m;
                m = Matrix4x4.Translate(PivotOffset) * m;
                curState.PositionCorrection += curState.GetCorrectedOrientation() * m.MultiplyPoint(Vector3.zero);
            }
            curState.OrientationCorrection = curState.OrientationCorrection * rotNoise;
        }

        /// <summary>Generate a new random seed</summary>
        public void ReSeed()
        {
            m_NoiseOffsets = new Vector3(
                    Random.Range(-1000f, 1000f),
                    Random.Range(-1000f, 1000f),
                    Random.Range(-1000f, 1000f));
        }

        void Initialize()
        {
            m_Initialized = true;
            m_NoiseTime = CinemachineCore.CurrentTime * FrequencyGain;
            if (m_NoiseOffsets == Vector3.zero)
                ReSeed();
        }
    }
}
