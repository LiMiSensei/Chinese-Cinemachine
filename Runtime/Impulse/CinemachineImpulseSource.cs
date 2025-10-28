using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
    /// <summary>
    /// 一个事件驱动的类，向监听器广播脉冲信号。
    ///
    /// 这是自定义脉冲源的基类。它包含一个脉冲定义，
    /// 其中定义了脉冲信号的特征。
    ///
    /// 提供了用于实际广播脉冲的 API 方法。从您的自定义代码中调用这些方法，
    /// 或在编辑器中将它们连接到游戏事件。
    ///
    /// </summary>
    [SaveDuringPlay]
    [AddComponentMenu("Cinemachine/Helpers/Cinemachine Impulse Source")]
    [HelpURL(Documentation.BaseURL + "manual/CinemachineImpulseSource.html")]
    public class CinemachineImpulseSource : MonoBehaviour
    {
        [RedHeader("Help：这是冲击波生成器，会按照对象位置生成冲击波")]
        [Space(30)]
        /// <summary>
        /// 这将定义要广播的完整脉冲信号。
        /// </summary>
        [FormerlySerializedAs("m_ImpulseDefinition")]
        public CinemachineImpulseDefinition ImpulseDefinition = new CinemachineImpulseDefinition();

        /// <summary>
        /// 在没有任何指定覆盖的情况下，脉冲信号的默认方向和力。
        /// 可以通过调用 API 中适当的 GenerateImpulse 方法来指定覆盖。
        /// </summary>
        [Header("默认调用")]
        [Tooltip("在没有指定任何覆盖的情况下，脉冲信号的默认方向和力。"
            + "可以通过调用API中适当的GenerateImpulse方法来指定覆盖。")]
        [FormerlySerializedAs("m_DefaultVelocity")]
        public Vector3 DefaultVelocity = Vector3.down;

        void OnValidate()
        {
            ImpulseDefinition.OnValidate();
        }

        void Reset()
        {
            ImpulseDefinition = new CinemachineImpulseDefinition
            {
                ImpulseChannel = 1,
                ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Bump,
                CustomImpulseShape = new AnimationCurve(),
                ImpulseDuration = 0.2f,
                ImpulseType = CinemachineImpulseDefinition.ImpulseTypes.Uniform,
                DissipationDistance = 100,
                DissipationRate = 0.25f,
                PropagationSpeed = 343
            };
            DefaultVelocity = Vector3.down;
        }

        /// <summary>使用自定义位置和冲击速度，将脉冲信号广播到适当的通道</summary>
        /// <param name="position">脉冲将从中发出的世界空间位置</param>
        /// <param name="velocity">冲击幅度和方向</param>
        public void GenerateImpulseAtPositionWithVelocity(Vector3 position, Vector3 velocity)
        {
            if (ImpulseDefinition != null)
                ImpulseDefinition.CreateEvent(position, velocity);
        }

        /// <summary>使用自定义冲击速度，以及此变换的位置，将脉冲信号广播到适当的通道</summary>
        /// <param name="velocity">冲击幅度和方向</param>
        public void GenerateImpulseWithVelocity(Vector3 velocity)
        {
            GenerateImpulseAtPositionWithVelocity(transform.position, velocity);
        }

        /// <summary>使用自定义冲击力，标准方向，以及此变换的位置，将脉冲信号广播到适当的通道</summary>
        /// <param name="force">冲击幅度。1 为正常</param>
        public void GenerateImpulseWithForce(float force)
        {
            GenerateImpulseAtPositionWithVelocity(transform.position, DefaultVelocity * force);
        }

        /// <summary>使用默认速度 = (0, -1, 0) 和默认位置（此变换的位置），
        /// 将脉冲信号广播到适当的通道</summary>
        public void GenerateImpulse()
        {
            GenerateImpulseWithVelocity(DefaultVelocity);
        }

        /// <summary>旧版 API：请改用 GenerateImpulseAtPositionWithVelocity()。
        /// 使用自定义位置和冲击速度，将脉冲信号广播到适当的通道</summary>
        /// <param name="position">脉冲将从中发出的世界空间位置</param>
        /// <param name="velocity">冲击幅度和方向</param>
        public void GenerateImpulseAt(Vector3 position, Vector3 velocity)
            => GenerateImpulseAtPositionWithVelocity(position, velocity);

        /// <summary>旧版 API：请改用 GenerateImpulseWithVelocity()。
        /// 使用自定义冲击速度，以及此变换的位置，将脉冲信号广播到适当的通道</summary>
        /// <param name="velocity">冲击幅度和方向</param>
        public void GenerateImpulse(Vector3 velocity) => GenerateImpulseWithVelocity(velocity);

        /// <summary>旧版 API：请改用 GenerateImpulseWithForce()。
        /// 使用自定义冲击力，标准方向，以及此变换的位置，将脉冲信号广播到适当的通道</summary>
        /// <param name="force">冲击幅度。1 为正常</param>
        public void GenerateImpulse(float force) => GenerateImpulseWithForce(force);
    }
}