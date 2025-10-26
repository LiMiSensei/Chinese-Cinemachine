using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
    /// <summary>
    /// An extension for CinemachineCamera which post-processes
    /// the final position of the camera.  It listens for CinemachineImpulse
    /// signals on the specified channels, and moves the camera in response to them.
    /// </summary>
    [SaveDuringPlay]
    [AddComponentMenu("Cinemachine/Procedural/Extensions/Cinemachine Impulse Listener")]
    [ExecuteAlways]
    [HelpURL(Documentation.BaseURL + "manual/CinemachineImpulseListener.html")]
    public class CinemachineImpulseListener : CinemachineExtension
    {
        /// <summary>
        /// 何时应用脉冲反应。默认为 Noise 阶段。
        /// 必要时修改此设置以影响扩展效果的顺序
        /// </summary>
        [Tooltip("何时应用脉冲反应。默认为 Noise 阶段之后。"
            + "必要时修改此设置以影响扩展效果的顺序")]
        [FormerlySerializedAs("m_ApplyAfter")]
        public CinemachineCore.Stage ApplyAfter = CinemachineCore.Stage.Aim; // 向后兼容设置

        /// <summary>
        /// 掩码中未包含的通道上的脉冲事件将被忽略。
        /// </summary>
        [Tooltip("掩码中未包含的通道上的脉冲事件将被忽略。")]
        [CinemachineImpulseChannelProperty]
        [FormerlySerializedAs("m_ChannelMask")]
        public int ChannelMask;

        /// <summary>
        /// 应用于脉冲信号的增益。
        /// </summary>
        [Tooltip("应用于脉冲信号的增益。1 为正常强度。"
            + "设置为 0 将完全静音信号。")]
        [FormerlySerializedAs("m_Gain")]
        public float Gain;

        /// <summary>
        /// 启用此选项以在 2D 中执行距离计算（忽略 Z 轴）。
        /// </summary>
        [Tooltip("启用此选项以在 2D 中执行距离计算（忽略 Z 轴）")]
        [FormerlySerializedAs("m_Use2DDistance")]
        public bool Use2DDistance;

        /// <summary>
        /// 启用此选项以在摄像机空间中处理所有脉冲信号。
        /// </summary>
        [Tooltip("启用此选项以在摄像机空间中处理所有脉冲信号")]
        [FormerlySerializedAs("m_UseCameraSpace")]
        public bool UseCameraSpace;

        /// <summary>
        /// 监听器如何处理多个重叠脉冲信号的选择。
        /// </summary>
        public enum SignalCombinationModes 
        {
            /// <summary>
            /// 将所有活动信号组合在一起，类似于
            /// 声波在空气中的组合方式。
            /// </summary>
            Additive,
            /// <summary>
            /// 仅考虑振幅最大的信号。
            /// 其他信号将被忽略。
            /// </summary>
            UseLargest
        }

        /// <summary>
        /// 指定脉冲监听器如何组合当前空间点上活动的多个脉冲。
        /// </summary>
        [Tooltip("控制脉冲监听器如何组合当前空间点上活动的多个脉冲。\n\n"
            + "<b>叠加</b>: 将所有活动信号组合在一起，就像声波一样。"
            + "这是默认设置。\n\n"
            + "<b>使用最大</b>: 仅考虑振幅最大的信号；忽略"
            + "任何其他信号。")]
        public SignalCombinationModes SignalCombinationMode = SignalCombinationModes.Additive;

        /// <summary>
        /// 这控制监听器对传入脉冲的次级反应。
        /// 脉冲可能是一个剧烈的冲击，而次级反应可能是
        /// 一个振动，其振幅和持续时间由原始脉冲的
        /// 大小控制。这允许不同的监听器以不同的方式
        /// 对相同的脉冲信号做出反应。
        /// </summary>
        [Serializable]
        public struct ImpulseReaction
        {
            /// <summary>
            /// 将由主脉冲触发的次级抖动
            /// </summary>
            [Tooltip("将由主脉冲触发的次级抖动。")]
            public NoiseSettings m_SecondaryNoise;

            /// <summary>
            /// 应用于信号源资源中定义振幅的增益。
            /// </summary>
            [Tooltip("应用于信号源中定义振幅的增益。"
                + "1 为正常。设置为 0 将完全静音信号。")]
            [FormerlySerializedAs("m_AmplitudeGain")]
            public float AmplitudeGain;

           /// <summary>
            /// 应用于时间轴的缩放因子。
            /// </summary>
            [Tooltip("应用于时间轴的缩放因子。1 为正常。"
                + "较大的值将使信号进展更快。")]
            [FormerlySerializedAs("m_FrequencyGain")]
            public float FrequencyGain;

            /// <summary>
            /// 次级反应的持续时间。
            /// </summary>
            [Tooltip("次级反应的持续时间。")]
            [FormerlySerializedAs("m_Duration")]
            public float Duration;

            float m_CurrentAmount;
            float m_CurrentTime;
            float m_CurrentDamping;

            bool m_Initialized;

            [SerializeField, HideInInspector, NoSaveDuringPlay]
            Vector3 m_NoiseOffsets;

            /// <summary>Generate a new random seed</summary>
            public void ReSeed()
            {
                m_NoiseOffsets = new Vector3(
                        UnityEngine.Random.Range(-1000f, 1000f),
                        UnityEngine.Random.Range(-1000f, 1000f),
                        UnityEngine.Random.Range(-1000f, 1000f));
            }

            /// <summary>
            /// Get the rection effect for a given impulse at a given time.
            /// </summary>
            /// <param name="deltaTime">Current time interval</param>
            /// <param name="impulsePos">The input impulse signal at this time</param>
            /// <param name="pos">output reaction position delta</param>
            /// <param name="rot">output reaction rotation delta</param>
            /// <returns>True if there is a reaction effect, false otherwise</returns>
            public bool GetReaction(
                float deltaTime, Vector3 impulsePos,
                out Vector3 pos, out Quaternion rot)
            {
                if (!m_Initialized)
                {
                    m_Initialized = true;
                    m_CurrentAmount = 0;
                    m_CurrentDamping = 0;
                    m_CurrentTime = CinemachineCore.CurrentTime * FrequencyGain;
                    if (m_NoiseOffsets == Vector3.zero)
                        ReSeed();
                }

                // Is there any reacting to do?
                pos = Vector3.zero;
                rot = Quaternion.identity;
                var sqrMag = impulsePos.sqrMagnitude;
                if (m_SecondaryNoise == null || (sqrMag < 0.001f && m_CurrentAmount < 0.0001f))
                    return false;

                // Advance the current reaction time
                if (TargetPositionCache.CacheMode == TargetPositionCache.Mode.Playback
                        && TargetPositionCache.HasCurrentTime)
                    m_CurrentTime = TargetPositionCache.CurrentTime * FrequencyGain;
                else
                    m_CurrentTime += deltaTime * FrequencyGain;

                // Adjust the envelope height and duration of the secondary noise,
                // according to the strength of the incoming signal
                m_CurrentAmount = Mathf.Max(m_CurrentAmount, Mathf.Sqrt(sqrMag));
                m_CurrentDamping = Mathf.Max(m_CurrentDamping, Mathf.Max(1, Mathf.Sqrt(m_CurrentAmount)) * Duration);

                var gain = m_CurrentAmount * AmplitudeGain;
                pos = NoiseSettings.GetCombinedFilterResults(
                        m_SecondaryNoise.PositionNoise, m_CurrentTime, m_NoiseOffsets) * gain;
                rot = Quaternion.Euler(NoiseSettings.GetCombinedFilterResults(
                        m_SecondaryNoise.OrientationNoise, m_CurrentTime, m_NoiseOffsets) * gain);

                m_CurrentAmount -= Damper.Damp(m_CurrentAmount, m_CurrentDamping, deltaTime);
                m_CurrentDamping -= Damper.Damp(m_CurrentDamping, m_CurrentDamping, deltaTime);
                return true;
            }
        }

        /// <summary>
        /// This controls the secondary reaction of the listener to the incoming impulse.
        /// The impulse might be for example a sharp shock, and the secondary reaction could
        /// be a vibration whose amplitude and duration is controlled by the size of the
        /// original impulse.  This allows different listeners to respond in different ways
        /// to the same impulse signal.
        /// </summary>
        [Tooltip("This controls the secondary reaction of the listener to the incoming impulse.  "
            + "The impulse might be for example a sharp shock, and the secondary reaction could "
            + "be a vibration whose amplitude and duration is controlled by the size of the "
            + "original impulse.  This allows different listeners to respond in different ways "
            + "to the same impulse signal.")]
        [FormerlySerializedAs("m_ReactionSettings")]
        public ImpulseReaction ReactionSettings;

        private void Reset()
        {
            ApplyAfter = CinemachineCore.Stage.Noise; // this is the default setting
            ChannelMask = 1;
            Gain = 1;
            Use2DDistance = false;
            UseCameraSpace = true;
            SignalCombinationMode = SignalCombinationModes.Additive;
            ReactionSettings = new ImpulseReaction
            {
                AmplitudeGain = 1,
                FrequencyGain = 1,
                Duration = 1f
            };
        }

        /// <summary>React to any detected impulses</summary>
        /// <param name="vcam">The virtual camera being processed</param>
        /// <param name="stage">The current pipeline stage</param>
        /// <param name="state">The current virtual camera state</param>
        /// <param name="deltaTime">The current applicable deltaTime</param>
        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage == ApplyAfter && deltaTime >= 0)
            {
                bool haveImpulse = false;
                var impulsePos = Vector3.zero;
                var impulseRot = Quaternion.identity;

                if (SignalCombinationMode == SignalCombinationModes.Additive)
                {
                    // Get all impulses on the specified channels, and combine them
                    haveImpulse = CinemachineImpulseManager.Instance.GetImpulseAt(
                        state.GetFinalPosition(), Use2DDistance, ChannelMask,
                        out impulsePos, out impulseRot);
                }
                else
                {
                    // Get the largest impulse on the specified channels
                    haveImpulse = CinemachineImpulseManager.Instance.GetStrongestImpulseAt(
                        state.GetFinalPosition(), Use2DDistance, ChannelMask,
                        out impulsePos, out impulseRot);
                }

                bool haveReaction = ReactionSettings.GetReaction(
                    deltaTime, impulsePos, out var reactionPos, out var reactionRot);

                if (haveImpulse)
                {
                    impulseRot = Quaternion.SlerpUnclamped(Quaternion.identity, impulseRot, Gain);
                    impulsePos *= Gain;
                }
                if (haveReaction)
                {
                    impulsePos += reactionPos;
                    impulseRot *= reactionRot;
                }
                if (haveImpulse || haveReaction)
                {
                    if (UseCameraSpace)
                        impulsePos = state.RawOrientation * impulsePos;
                    state.PositionCorrection += impulsePos;
                    state.OrientationCorrection = state.OrientationCorrection * impulseRot;
                }
            }
        }
    }
}
