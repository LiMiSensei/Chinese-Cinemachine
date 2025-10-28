using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
    /// <summary>
    /// 此行为可以附加到任何对象上，使其响应脉冲而产生震动。
    ///
    /// 此行为可以附加到带有 CinemachineBrain 的主摄像机上，
    /// 允许主摄像机震动而无需在虚拟摄像机上放置监听器。
    /// 在这种情况下，摄像机震动不依赖于活动的虚拟摄像机。
    ///
    /// 也可以将此行为附加到其他场景对象上，使它们
    /// 响应脉冲而产生震动。
    /// </summary>
    [SaveDuringPlay]
    [AddComponentMenu("Cinemachine/Helpers/Cinemachine External Impulse Listener")]
    [HelpURL(Documentation.BaseURL + "manual/CinemachineExternalImpulseListener.html")]
    public class CinemachineExternalImpulseListener : MonoBehaviour
    {
        
        Vector3 m_ImpulsePosLastFrame;
        Quaternion m_ImpulseRotLastFrame;


        [RedHeader("Help：冲击波接收器，使其响应脉冲而产生震动")]
        [Space(30)]
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
        [Tooltip("应用于脉冲信号的增益。1 为正常强度。设置为 0 将完全静音信号。")]
        [FormerlySerializedAs("m_Gain")]
        public float Gain;

        /// <summary>
        /// 启用此选项以在 2D 中执行距离计算（忽略 Z 轴）。
        /// </summary>
        [Tooltip("启用此选项以在 2D 中执行距离计算（忽略 Z 轴）")]
        [FormerlySerializedAs("m_Use2DDistance")]
        public bool Use2DDistance;

        /// <summary>
        /// 启用此选项以在本地空间中处理所有脉冲信号。
        /// </summary>
        [Tooltip("启用此选项以在本地空间中处理所有脉冲信号")]
        [FormerlySerializedAs("m_UseLocalSpace")]
        public bool UseLocalSpace;

        /// <summary>
        /// 这控制监听器对传入脉冲的次级反应。
        /// 脉冲可能是一个剧烈的冲击，而次级反应可能是
        /// 一个振动，其振幅和持续时间由原始脉冲的
        /// 大小控制。这允许不同的监听器以不同的方式
        /// 对相同的脉冲信号做出反应。
        /// </summary>
        [Tooltip("这控制监听器对传入脉冲的次级反应。"
            + "脉冲可能是一个剧烈的冲击，而次级反应可能是"
            + "一个振动，其振幅和持续时间由原始脉冲的"
            + "大小控制。这允许不同的监听器以不同的方式"
            + "对相同的脉冲信号做出反应。")]
        [FormerlySerializedAs("m_ReactionSettings")]
        public CinemachineImpulseListener.ImpulseReaction ReactionSettings;

        private void Reset()
        {
            ChannelMask = 1;
            Gain = 1;
            Use2DDistance = false;
            UseLocalSpace = true;
            ReactionSettings = new CinemachineImpulseListener.ImpulseReaction
            {
                AmplitudeGain = 1,
                FrequencyGain = 1,
                Duration = 1f
            };
        }

        private void OnEnable()
        {
            m_ImpulsePosLastFrame = Vector3.zero;
            m_ImpulseRotLastFrame = Quaternion.identity;
        }

        private void Update()
        {
            // Unapply previous shake
            transform.position -= m_ImpulsePosLastFrame;
            transform.rotation = transform.rotation * Quaternion.Inverse(m_ImpulseRotLastFrame);
        }

        // We do this in LateUpdate specifically to support attaching this script to the
        // Camera with the CinemachineBrain.  Script execution order is after the brain.
        private void LateUpdate()
        {
            // Apply the shake
            bool haveImpulse = CinemachineImpulseManager.Instance.GetImpulseAt(
                transform.position, Use2DDistance, ChannelMask,
                out m_ImpulsePosLastFrame, out m_ImpulseRotLastFrame);
            bool haveReaction = ReactionSettings.GetReaction(
                Time.deltaTime, m_ImpulsePosLastFrame, out var reactionPos, out var reactionRot);

            if (haveImpulse)
            {
                m_ImpulseRotLastFrame = Quaternion.SlerpUnclamped(
                    Quaternion.identity, m_ImpulseRotLastFrame, Gain);
                m_ImpulsePosLastFrame *= Gain;
            }
            if (haveReaction)
            {
                m_ImpulsePosLastFrame += reactionPos;
                m_ImpulseRotLastFrame *= reactionRot;
            }
            if (haveImpulse || haveReaction)
            {
                if (UseLocalSpace)
                    m_ImpulsePosLastFrame = transform.rotation * m_ImpulsePosLastFrame;

                transform.position += m_ImpulsePosLastFrame;
                transform.rotation = transform.rotation * m_ImpulseRotLastFrame;
            }
        }
    }
}
