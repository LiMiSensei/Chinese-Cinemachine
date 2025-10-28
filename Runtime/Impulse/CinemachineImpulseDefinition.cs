using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
    /// <summary>
    /// Definition of an impulse signal that gets propagated to listeners.
    ///
    /// Here you provide a Raw Signal source, and define an envelope for time-scaling
    /// it to craft the complete Impulse signal shape.  Also, you provide here parameters
    /// that define how the signal dissipates with spatial distance from the source location.
    /// Finally, you specify the Impulse Channel on which the signal will be sent.
    ///
    /// An API method is provided here to take these parameters, create an Impulse Event,
    /// and broadcast it on the channel.
    ///
    /// When creating a custom Impulse Source class, you will have an instance of this class
    /// as a field in your custom class.  Be sure also to include the
    /// [CinemachineImpulseDefinition] attribute on the field, to get the right
    /// property drawer for it.
    /// </summary>
    [Serializable]
    public class CinemachineImpulseDefinition
    {
        /// <summary>
        /// Impulse events generated here will appear on the channels included in the mask.
        /// </summary>
        [CinemachineImpulseChannelProperty]
        [Tooltip("此处生成的脉冲事件将出现在掩码包含的通道上。")]
        [FormerlySerializedAs("m_ImpulseChannel")]
        public int ImpulseChannel = 1;

        /// <summary>支持的预定义脉冲形状。</summary>
        public enum ImpulseShapes
        {
            /// <summary>自定义形状</summary>
            Custom,
            /// <summary>适用于后坐力的形状，例如枪击。</summary>
            Recoil,
            /// <summary>适用于碰撞的形状。</summary>
            Bump,
            /// <summary>适用于爆炸的形状。</summary>
            Explosion,
            /// <summary>适用于较长震动的形状。</summary>
            Rumble
        };

        /// <summary>碰撞信号的形状。</summary>
        [Tooltip("碰撞信号的形状")]
        [FormerlySerializedAs("m_ImpulseShape")]
        public ImpulseShapes ImpulseShape;

        /// <summary>
        /// 用户定义的脉冲形状，仅在 m_ImpulseShape 为 Custom 时使用。
        /// 定义将生成的信号。X 轴必须从 0...1，
        /// Y 轴是将应用于碰撞速度的缩放比例。
        /// </summary>
        [Tooltip("定义将生成的碰撞信号的自定义形状。")]
        [FormerlySerializedAs("m_CustomImpulseShape")]
        public AnimationCurve CustomImpulseShape = new AnimationCurve();

        /// <summary>
        /// 碰撞信号将持续的时间。
        /// 信号形状将被拉伸以填充该时间。
        /// </summary>
        [Tooltip("碰撞信号将持续的时间。"
            + "信号形状将被拉伸以填充该时间。")]
        [FormerlySerializedAs("m_ImpulseDuration")]
        public float ImpulseDuration = 0.2f;

        /// <summary>
        /// 此枚举表示脉冲在空间中传播的各种方式
        /// </summary>
        public enum ImpulseTypes
        {
            /// <summary>脉冲在空间中各处同时均匀感受</summary>
            Uniform,
            /// <summary>脉冲仅在指定半径内感受，其强度
            /// 随监听者距离增加而减弱</summary>
            Dissipating,
            /// <summary>
            /// 脉冲仅在指定半径内感受，其强度
            /// 随监听者距离增加而减弱。此外，脉冲从
            /// 碰撞点向外传播，以指定速度，类似于声波。
            /// </summary>
            Propagating,
            /// <summary>
            /// 用于旧项目的向后兼容模式。建议使用
            /// 其他脉冲类型（如果可能）。
            /// </summary>
            Legacy
        }

        /// <summary>
        /// 脉冲如何在空间和时间中传播。
        /// </summary>
        [Tooltip("脉冲如何在空间和时间中传播。均匀-半径减弱-声波-向后兼容")]
        [FormerlySerializedAs("m_ImpulseType")]
        public ImpulseTypes ImpulseType = ImpulseTypes.Legacy;    // 默认使用向后兼容模式

        /// <summary>
        /// 这定义了信号在效应半径内传播的范围，
        /// 然后随距离碰撞点的距离而消散
        /// </summary>
        [Tooltip("这定义了信号在效应半径内传播的范围，"
            + "然后随距离碰撞点的距离而消散")]
        [Range(0,1)]
        [FormerlySerializedAs("m_DissipationRate")]
        public float DissipationRate;

        /// <summary>
        /// 仅限旧模式：定义将生成的信号。
        /// </summary>
        [Header("信号形状")]
        [Tooltip("仅限旧模式：定义将生成的信号。")]
        [CinemachineEmbeddedAssetProperty(true)]
        [FormerlySerializedAs("m_RawSignal")]
        public SignalSourceAsset RawSignal = null;

        /// <summary>
        /// 仅限旧模式：应用于信号源资源中定义振幅的增益。
        /// </summary>
        [Tooltip("仅限旧模式：应用于信号源中定义振幅的增益。"
            + "1 为正常。设置为 0 将完全静音信号。")]
        [FormerlySerializedAs("m_AmplitudeGain")]
        public float AmplitudeGain = 1f;

        /// <summary>
        /// 仅限旧模式：应用于时间轴的缩放因子。
        /// </summary>
        [Tooltip("仅限旧模式：应用于时间轴的缩放因子。1 为正常。"
            + "较大的值将使信号进展更快。")]
        [FormerlySerializedAs("m_FrequencyGain")]
        public float FrequencyGain = 1f;

        /// <summary>仅限旧模式：如何将信号拟合到包络时间中</summary>
        public enum RepeatModes
        {
            /// <summary>时间拉伸信号以适合包络</summary>
            Stretch,
            /// <summary>循环信号以填充包络</summary>
            Loop
        }
        /// <summary>仅限旧模式：如何将信号拟合到包络时间中</summary>
        [Tooltip("仅限旧模式：如何将信号拟合到包络时间中")]
        [FormerlySerializedAs("m_RepeatMode")]
        public RepeatModes RepeatMode = RepeatModes.Stretch;

        /// <summary>仅限旧模式：随机化信号开始时间</summary>
        [Tooltip("仅限旧模式：随机化信号开始时间")]
        [FormerlySerializedAs("m_Randomize")]
        public bool Randomize = true;

        /// <summary>
        /// 仅限旧模式：这定义了信号的时间包络。
        /// 原始信号将按时间缩放以适合包络。
        /// </summary>
        [Tooltip("仅限旧模式：这定义了信号的时间包络。"
            + "原始信号将按时间缩放以适合包络。")]
        [FormerlySerializedAs("m_TimeEnvelope")]
        public CinemachineImpulseManager.EnvelopeDefinition TimeEnvelope
            = CinemachineImpulseManager.EnvelopeDefinition.Default;

        /// <summary>
        /// 仅限旧模式：信号在碰撞点周围此半径内具有完整振幅。
        /// 超出此范围将随距离消散。
        /// </summary>
        [Header("空间范围")]
        [Tooltip("仅限旧模式：信号在碰撞点周围此半径内具有完整振幅。"
            + "超出此范围将随距离消散。")]
        [FormerlySerializedAs("m_ImpactRadius")]
        public float ImpactRadius = 100;

        /// <summary>仅限旧模式：当监听者从原点移动时信号方向的行为方式。</summary>
        [Tooltip("仅限旧模式：当监听者从原点移动时信号方向的行为方式。")]
        [FormerlySerializedAs("m_DirectionMode")]
        public CinemachineImpulseManager.ImpulseEvent.DirectionModes DirectionMode
            = CinemachineImpulseManager.ImpulseEvent.DirectionModes.Fixed;

        /// <summary>
        /// 仅限旧模式：这定义了信号如何在碰撞半径之外随距离消散。
        /// </summary>
        [Tooltip("仅限旧模式：这定义了信号如何在碰撞半径之外随距离消散。")]
        [FormerlySerializedAs("m_DissipationMode")]
        public CinemachineImpulseManager.ImpulseEvent.DissipationModes DissipationMode
            = CinemachineImpulseManager.ImpulseEvent.DissipationModes.ExponentialDecay;

        /// <summary>
        /// 信号在碰撞点周围此半径之外没有效果。
        /// </summary>
        [Tooltip("信号在碰撞点周围此半径之外没有效果。")]
        [FormerlySerializedAs("m_DissipationDistance")]
        public float DissipationDistance = 100;

        /// <summary>
        /// 脉冲在空间中传播的速度（米/秒）。高速
        /// 允许监听者瞬时反应，而较慢的速度允许场景中的
        /// 监听者像对从源扩散的波一样反应。
        /// </summary>
        [Tooltip("脉冲在空间中传播的速度（米/秒）。高速"
            + "允许监听者瞬时反应，而较慢的速度允许场景中的"
            + "监听者像对从源扩散的波一样反应。")]
        [FormerlySerializedAs("m_PropagationSpeed")]
        public float PropagationSpeed = 343;  // 声速


        /// <summary>Call this from your behaviour's OnValidate to validate the fields here</summary>
        public void OnValidate()
        {
            RuntimeUtility.NormalizeCurve(CustomImpulseShape, true, false);
            ImpulseDuration = Mathf.Max(UnityVectorExtensions.Epsilon, ImpulseDuration);
            DissipationDistance = Mathf.Max(UnityVectorExtensions.Epsilon, DissipationDistance);
            DissipationRate = Mathf.Clamp01(DissipationRate);
            PropagationSpeed = Mathf.Max(1, PropagationSpeed);

            // legacy
            ImpactRadius = Mathf.Max(0, ImpactRadius);
            TimeEnvelope.Validate();
            PropagationSpeed = Mathf.Max(1, PropagationSpeed);
        }

        static AnimationCurve[] s_StandardShapes;
        static void CreateStandardShapes()
        {
            int max = 0;
            var iter = Enum.GetValues(typeof(ImpulseShapes)).GetEnumerator();
            while (iter.MoveNext())
                max = Mathf.Max(max, (int)iter.Current);
            s_StandardShapes = new AnimationCurve[max + 1];

            s_StandardShapes[(int)ImpulseShapes.Recoil] = new AnimationCurve(new Keyframe[]
            {
                new Keyframe(0, 1, -3.2f, -3.2f),
                new Keyframe(1, 0, 0, 0)
            });

            s_StandardShapes[(int)ImpulseShapes.Bump] = new AnimationCurve(new Keyframe[]
            {
                new Keyframe(0, 0, -4.9f, -4.9f),
                new Keyframe(0.2f, 0, 8.25f,  8.25f),
                new Keyframe(1, 0, -0.25f, -0.25f)
            });

            s_StandardShapes[(int)ImpulseShapes.Explosion] = new AnimationCurve(new Keyframe[]
            {
                new Keyframe(0, -1.4f, -7.9f, -7.9f),
                new Keyframe(0.27f, 0.78f, 23.4f, 23.4f),
                new Keyframe(0.54f, -0.12f, 22.6f, 22.6f),
                new Keyframe(0.75f, 0.042f, 9.23f, 9.23f),
                new Keyframe(0.9f, -0.02f, 5.8f, 5.8f),
                new Keyframe(0.95f, -0.006f, -3.0f, -3.0f),
                new Keyframe(1, 0, 0, 0)
            });

            s_StandardShapes[(int)ImpulseShapes.Rumble] = new AnimationCurve(new Keyframe[]
            {
                new Keyframe(0, 0, 0, 0),
                new Keyframe(0.1f, 0.25f, 0, 0),
                new Keyframe(0.2f, 0, 0, 0),
                new Keyframe(0.3f, 0.75f, 0, 0),
                new Keyframe(0.4f, 0, 0, 0),
                new Keyframe(0.5f, 1, 0, 0),
                new Keyframe(0.6f, 0, 0, 0),
                new Keyframe(0.7f, 0.75f, 0, 0),
                new Keyframe(0.8f, 0, 0, 0),
                new Keyframe(0.9f, 0.25f, 0, 0),
                new Keyframe(1, 0, 0, 0)
            });
        }

        internal static AnimationCurve GetStandardCurve(ImpulseShapes shape)
        {
            if (s_StandardShapes == null)
                CreateStandardShapes();
            return s_StandardShapes[(int)shape];
        }

        internal AnimationCurve ImpulseCurve
        {
            get
            {
                if (ImpulseShape == ImpulseShapes.Custom)
                {
                    if (CustomImpulseShape == null)
                        CustomImpulseShape = AnimationCurve.EaseInOut(0f, 0f, 1, 1f);
                    return CustomImpulseShape;
                }
                return GetStandardCurve(ImpulseShape);
            }
        }

        /// <summary>Generate an impulse event at a location in space,
        /// and broadcast it on the appropriate impulse channel</summary>
        /// <param name="position">Event originates at this position in world space</param>
        /// <param name="velocity">This direction is considered to be "down" for the purposes of the
        /// event signal, and the magnitude of the signal will be scaled according to the
        /// length of this vector</param>
        public void CreateEvent(Vector3 position, Vector3 velocity)
        {
            CreateAndReturnEvent(position, velocity);
        }

        /// <summary>Generate an impulse event at a location in space,
        /// and broadcast it on the appropriate impulse channel</summary>
        /// <param name="position">Event originates at this position in world space</param>
        /// <param name="velocity">This direction is considered to be "down" for the purposes of the
        /// event signal, and the magnitude of the signal will be scaled according to the
        /// length of this vector</param>
        /// <returns>The newly-created impulse event. This can be used to dynamically adjust the
        /// event settings while the event is active. Note that this event object may be recycled
        /// for future events, so the pointer should not be retained for too long.</returns>
        public CinemachineImpulseManager.ImpulseEvent CreateAndReturnEvent(
            Vector3 position, Vector3 velocity)
        {
            // Legacy mode
            if (ImpulseType == ImpulseTypes.Legacy)
                return LegacyCreateAndReturnEvent(position, velocity);

            const float kBigNumber = 9999999.0f;

            if ((ImpulseShape == ImpulseShapes.Custom && CustomImpulseShape == null)
                || Mathf.Abs(DissipationDistance) < UnityVectorExtensions.Epsilon
                || Mathf.Abs(ImpulseDuration) < UnityVectorExtensions.Epsilon)
                return null;

            var e = CinemachineImpulseManager.Instance.NewImpulseEvent();
            e.Envelope = new CinemachineImpulseManager.EnvelopeDefinition
            {
                SustainTime = ImpulseDuration
            };

            e.SignalSource = new SignalSource(this, velocity);
            e.Position = position;
            e.Radius = ImpulseType == ImpulseTypes.Uniform ? kBigNumber : 0;
            e.Channel = ImpulseChannel;
            e.DirectionMode = CinemachineImpulseManager.ImpulseEvent.DirectionModes.Fixed;
            e.DissipationDistance = ImpulseType == ImpulseTypes.Uniform ? 0 : DissipationDistance;
            e.PropagationSpeed = ImpulseType == ImpulseTypes.Propagating ? PropagationSpeed : kBigNumber;
            e.CustomDissipation = DissipationRate;

            CinemachineImpulseManager.Instance.AddImpulseEvent(e);
            return e;
        }

        CinemachineImpulseManager.ImpulseEvent LegacyCreateAndReturnEvent(
            Vector3 position, Vector3 velocity)
        {
            if (RawSignal == null || Mathf.Abs(TimeEnvelope.Duration) < UnityVectorExtensions.Epsilon)
                return null;

            var e = CinemachineImpulseManager.Instance.NewImpulseEvent();
            e.Envelope = TimeEnvelope;

            // Scale the time-envelope decay as the root of the amplitude scale
            e.Envelope = TimeEnvelope;
            if (TimeEnvelope.ScaleWithImpact)
                e.Envelope.DecayTime *= Mathf.Sqrt(velocity.magnitude);

            e.SignalSource = new LegacySignalSource(this, velocity);
            e.Position = position;
            e.Radius = ImpactRadius;
            e.Channel = ImpulseChannel;
            e.DirectionMode = DirectionMode;
            e.DissipationMode = DissipationMode;
            e.DissipationDistance = DissipationDistance;
            e.PropagationSpeed = PropagationSpeed;
            CinemachineImpulseManager.Instance.AddImpulseEvent(e);

            return e;
        }

        // Wrap the raw signal to handle gain, RepeatMode, randomization, and velocity
        class SignalSource : ISignalSource6D
        {
            CinemachineImpulseDefinition m_Def;
            Vector3 m_Velocity;

            public SignalSource(CinemachineImpulseDefinition def, Vector3 velocity)
            {
                m_Def = def;
                m_Velocity = velocity;
            }

            public float SignalDuration => m_Def.ImpulseDuration;

            public void GetSignal(float timeSinceSignalStart, out Vector3 pos, out Quaternion rot)
            {
                pos = m_Velocity * m_Def.ImpulseCurve.Evaluate(timeSinceSignalStart / SignalDuration);
                rot = Quaternion.identity;
            }
        }

        // Wrap the raw signal to handle gain, RepeatMode, randomization, and velocity
        class LegacySignalSource : ISignalSource6D
        {
            CinemachineImpulseDefinition m_Def;
            Vector3 m_Velocity;
            float m_StartTimeOffset = 0;

            public LegacySignalSource(CinemachineImpulseDefinition def, Vector3 velocity)
            {
                m_Def = def;
                m_Velocity = velocity;
                if (m_Def.Randomize && m_Def.RawSignal.SignalDuration <= 0)
                    m_StartTimeOffset = UnityEngine.Random.Range(-1000f, 1000f);
            }

            public float SignalDuration => m_Def.RawSignal.SignalDuration;

            public void GetSignal(float timeSinceSignalStart, out Vector3 pos, out Quaternion rot)
            {
                float time = m_StartTimeOffset + timeSinceSignalStart * m_Def.FrequencyGain;

                // Do we have to fit the signal into the envelope?
                float signalDuration = SignalDuration;
                if (signalDuration > 0)
                {
                    if (m_Def.RepeatMode == RepeatModes.Loop)
                        time %= signalDuration;
                    else if (m_Def.TimeEnvelope.Duration > UnityVectorExtensions.Epsilon)
                        time *= m_Def.TimeEnvelope.Duration / signalDuration; // stretch
                }

                m_Def.RawSignal.GetSignal(time, out pos, out rot);
                float gain = m_Velocity.magnitude;
                gain *= m_Def.AmplitudeGain;
                pos *= gain;
                pos = Quaternion.FromToRotation(Vector3.down, m_Velocity) * pos;
                rot = Quaternion.SlerpUnclamped(Quaternion.identity, rot, gain);
            }
        }
    }
}
