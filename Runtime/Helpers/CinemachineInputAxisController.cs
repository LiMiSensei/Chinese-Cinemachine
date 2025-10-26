using System;
using UnityEngine;

#if CINEMACHINE_UNITY_INPUTSYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
#endif

namespace Unity.Cinemachine
{
    /// <summary>
    /// This is a behaviour that is used to drive other behaviours that implement IInputAxisOwner,
    /// which it discovers dynamically.  It is the bridge between the input system and
    /// Cinemachine cameras that require user input.  Add it to a Cinemachine camera that needs it.
    ///
    /// This implementation can read input from the Input package, or from the legacy input system,
    /// or both, depending on what is installed in the project.
    /// </summary>
    [ExecuteAlways]
    [SaveDuringPlay]
    [AddComponentMenu("Cinemachine/Helpers/Cinemachine Input Axis Controller")]
    [HelpURL(Documentation.BaseURL + "manual/CinemachineInputAxisController.html")]
    public class CinemachineInputAxisController
        : InputAxisControllerBase<CinemachineInputAxisController.Reader>
    {
#if CINEMACHINE_UNITY_INPUTSYSTEM
        /// <summary>
        /// 对于单人游戏，保持此值为-1。
        /// 对于多人游戏，将其设置为玩家索引，操作将从该玩家的控制中读取
        /// </summary>
        [Tooltip("对于单人游戏，保持此值为-1。"
            + "对于多人游戏，将其设置为玩家索引，操作将从该玩家的控制中读取")]
        public int PlayerIndex = -1;

        /// <summary>如果设置为true，输入操作将在开始时自动启用</summary>
        [Tooltip("如果设置为true，输入操作将在开始时自动启用")]
        public bool AutoEnableInputs = true;
#endif
        /// <summary>
        /// 这是一种机制，允许检查器在组件重置时设置默认值
        /// </summary>
        /// <param name="axis">输入轴的信息</param>
        /// <param name="controller">要更改的控制器的引用</param>
        internal delegate void SetControlDefaultsForAxis(
            in IInputAxisOwner.AxisDescriptor axis, ref Controller controller);
        internal static SetControlDefaultsForAxis SetControlDefaults;

#if CINEMACHINE_UNITY_INPUTSYSTEM
        /// <summary>
        /// CinemachineInputAxisController.Reader 只能处理 float 或 Vector2 类型的 InputAction。
        /// 要处理其他类型，您可以安装一个处理程序来读取不同类型的 InputAction
        /// </summary>
        /// <param name="action">要读取的操作</param>
        /// <param name="hint">操作的轴提示</param>
        /// <param name="context">所有者对象，可用于记录诊断信息</param>
        /// <param name="defaultReadValue">如果您不处理该类型，要调用的默认读取器</param>
        /// <returns>控件的值</returns>
        public Reader.ControlValueReader ReadControlValueOverride;

        /// <inheritdoc />
        protected override void Reset()
        {
            base.Reset();
            PlayerIndex = -1;
            AutoEnableInputs = true;
        }
#endif

        /// <summary>
        /// 为轴创建默认控制器。
        /// 如果默认轴控制器不适合您的轴，请重写此方法
        /// </summary>
        /// <param name="axis">需要设置默认控制器的轴的描述</param>
        /// <param name="controller">驱动轴的控制器</param>
        protected override void InitializeControllerDefaultsForAxis(
            in IInputAxisOwner.AxisDescriptor axis, Controller controller)
        {
            SetControlDefaults?.Invoke(axis, ref controller);
        }

        //TODO 同时支持固定更新。输入系统有一个设置，仅在固定更新期间更新输入。
        //TODO 如果启用此设置，这将无法准确工作
        void Update()
        {
            if (Application.isPlaying)
                UpdateControllers();
        }

        /// <summary>从旧版输入系统或输入操作中读取输入值</summary>
        [Serializable]
        public sealed class Reader : IInputAxisReader
        {
#if CINEMACHINE_UNITY_INPUTSYSTEM
            /// <summary>Input包使用的操作映射（如果使用）</summary>
            [Tooltip("Input包使用的操作映射")]
            public InputActionReference InputAction;

            /// <summary>输入值在处理前会乘以这个量。
            /// 控制输入力度。设置为负值可以反转输入</summary>
            [Tooltip("输入值在处理前会乘以这个量。"
                + "控制输入力度。设置为负值可以反转输入")]
            public float Gain = 1;

            /// <summary>为玩家解析的实际操作</summary>
            [NonSerialized] internal InputAction m_CachedAction;

            /// <summary>
            /// CinemachineInputAxisController.Reader 只能处理 float 或 Vector2 类型的 InputAction。
            /// 要处理其他类型，您可以安装一个处理程序来读取不同类型的 InputAction
            /// </summary>
            /// <param name="action">要读取的操作</param>
            /// <param name="hint">操作的轴提示</param>
            /// <param name="context">所有者对象，可用于记录诊断信息</param>
            /// <param name="defaultReader">如果您不处理该类型，要调用的默认读取器</param>
            /// <returns>控件的值</returns>
            public delegate float ControlValueReader(
                InputAction action, IInputAxisOwner.AxisDescriptor.Hints hint, UnityEngine.Object context,
                ControlValueReader defaultReader);
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            /// <summary>旧版输入系统的轴名称（如果使用）。
            /// 将使用此名称调用 CinemachineCore.GetInputAxis()</summary>
            [InputAxisNameProperty]
            [Tooltip("旧版输入系统的轴名称（如果使用）。"
                + "此值将用于控制轴")]
            public string LegacyInput;

            /// <summary>LegacyInput值在处理前会乘以这个量。
            /// 控制输入力度。设置为负值可以反转输入</summary>
            [Tooltip("LegacyInput值在处理前会乘以这个量。"
                + "控制输入力度。设置为负值可以反转输入")]
            public float LegacyGain = 1;
#endif

            /// <summary>如果输入值固有地依赖于帧时间，则启用此选项。
            /// 例如，鼠标增量对于较长的帧自然更大，因此通常不应按deltaTime进行缩放</summary>
            [Tooltip("如果输入值固有地依赖于帧时间，则启用此选项。"
                + "例如，鼠标增量对于较长的帧自然更大，因此"
                + "在这种情况下应取消默认的deltaTime缩放")]
            public bool CancelDeltaTime = false;

            /// <inheritdoc />
            public float GetValue(
                UnityEngine.Object context,
                IInputAxisOwner.AxisDescriptor.Hints hint)
            {
                float inputValue = 0;
#if CINEMACHINE_UNITY_INPUTSYSTEM
                if (InputAction != null)
                {
                    if (context is CinemachineInputAxisController c)
                        inputValue = ResolveAndReadInputAction(c, hint) * Gain;
                }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
                if (inputValue == 0 && !string.IsNullOrEmpty(LegacyInput))
                {
                    try { inputValue = CinemachineCore.GetInputAxis(LegacyInput) * LegacyGain; }
                    catch (ArgumentException) {}
                    //catch (ArgumentException e) { Debug.LogError(e.ToString()); }
                }
#endif
                return (Time.deltaTime > 0 && CancelDeltaTime) ? inputValue / Time.deltaTime : inputValue;
            }

#if CINEMACHINE_UNITY_INPUTSYSTEM
            float ResolveAndReadInputAction(
                CinemachineInputAxisController context,
                IInputAxisOwner.AxisDescriptor.Hints hint)
            {
                // Resolve Action for player
                if (m_CachedAction != null && InputAction.action.id != m_CachedAction.id)
                    m_CachedAction = null;
                if (m_CachedAction == null)
                {
                    m_CachedAction = InputAction.action;
                    if (context.PlayerIndex != -1)
                        m_CachedAction = GetFirstMatch(InputUser.all[context.PlayerIndex], InputAction);
                    if (context.AutoEnableInputs && m_CachedAction != null)
                        m_CachedAction.Enable();

                    // local function to wrap the lambda which otherwise causes a tiny gc
                    static InputAction GetFirstMatch(in InputUser user, InputActionReference aRef)
                    {
                        var iter = user.actions.GetEnumerator();
                        while (iter.MoveNext())
                            if (iter.Current.id == aRef.action.id)
                                return iter.Current;
                        return null;
                    }
                }

                // Update enabled status
                if (m_CachedAction != null && m_CachedAction.enabled != InputAction.action.enabled)
                {
                    if (InputAction.action.enabled)
                        m_CachedAction.Enable();
                    else
                        m_CachedAction.Disable();
                }

                // Read the value
                if (m_CachedAction != null)
                {
                    // If client installed an override, use it
                    if (context.ReadControlValueOverride != null)
                        return context.ReadControlValueOverride.Invoke(m_CachedAction, hint, context, ReadInput);
                    return ReadInput(m_CachedAction, hint, context, null);
                }
                return 0;
            }

            /// <summary>
            /// Definition of how we read input. Override this in your child classes to specify
            /// the InputAction's type to read if it is different from float or Vector2.
            /// </summary>
            /// <param name="action">The action being read.</param>
            /// <param name="hint">The axis hint of the action.</param>
            /// <param name="context">The owner object, can be used for logging diagnostics</param>
            /// <param name="defaultReader">Not used</param>
            /// <returns>Returns the value of the input device.</returns>
            float ReadInput(
                InputAction action, IInputAxisOwner.AxisDescriptor.Hints hint,
                UnityEngine.Object context, ControlValueReader defaultReader)
            {
                if (action.activeValueType != null)
                {
                    if (action.activeValueType == typeof(Vector2))
                    {
                        var value = action.ReadValue<Vector2>();
                        return hint == IInputAxisOwner.AxisDescriptor.Hints.Y ? value.y : value.x;
                    }
                    if (action.activeValueType == typeof(float))
                        return action.ReadValue<float>();
            
                    Debug.LogError($"{context.name} - {action.name}: CinemachineInputAxisController.Reader can only read "
                        + "actions of type float or Vector2.  To read other types you can install a custom handler for "
                        + "CinemachineInputAxisController.ReadControlValueOverride.");
                }
                return 0f;
            }
#endif
        }
    }
}
