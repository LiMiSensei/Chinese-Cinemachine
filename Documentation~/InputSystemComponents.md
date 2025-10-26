# 将输入系统与 Cinemachine 结合使用

对于更复杂的输入配置（例如支持多种设备），你需要通过输入系统包提供的 `PlayerInput` 组件接收输入。以下部分假设你已了解如何设置此组件。如需更多信息，请参阅 [输入系统](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.5/manual/index.html) 文档和示例。


### 从 PlayerInput 读取输入

若要从行为设置为 `InvokeCSharpEvents` 的 `PlayerInput` 中读取值，需创建一个自定义 `InputAxisController` 来订阅 `onActionTriggered` 事件。下面的示例展示了如何接收这些输入并进行相应连接。将此脚本添加到你的 `CinemachineCamera` 中，并为 `PlayerInput` 字段赋值。

```cs
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

// 此类从 PlayerInput 组件接收输入，并将其分派到相应的 Cinemachine 输入轴。
// PlayerInput 组件应位于同一游戏对象上，或在 PlayerInput 字段中指定。
class CustomInputHandler : InputAxisControllerBase<CustomInputHandler.Reader>
{
    [Header("输入源覆盖")]
    public PlayerInput PlayerInput;

    void Awake()
    {
        // 当 PlayerInput 接收到输入时，将其发送给所有控制器
        if (PlayerInput == null)
            TryGetComponent(out PlayerInput);
        if (PlayerInput == null)
            Debug.LogError("找不到 PlayerInput 组件");
        else
        {
            PlayerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
            PlayerInput.onActionTriggered += (value) =>
            {
                for (var i = 0; i < Controllers.Count; i++)
                    Controllers[i].Input.ProcessInput(value.action);
            };
        }
    }

    // 我们在 Update 时钟上处理用户输入
    void Update()
    {
        if (Application.isPlaying)
            UpdateControllers();
    }

    // 控制器将是此类的实例
    [Serializable]
    public class Reader : IInputAxisReader
    {
        public InputActionReference Input;
        Vector2 m_Value; // 输入的缓存值

        public void ProcessInput(InputAction action)
        {
            // 如果是我的动作，则缓存新值
            if (Input != null && Input.action.id == action.id)
            {
                if (action.expectedControlType == "Vector2")
                    m_Value = action.ReadValue<Vector2>();
                else
                    m_Value.x = m_Value.y = action.ReadValue<float>();
            }
        }

        // IInputAxisReader 接口：由框架调用以读取输入值
        public float GetValue(UnityEngine.Object context, IInputAxisOwner.AxisDescriptor.Hints hint)
        {
            return (hint == IInputAxisOwner.AxisDescriptor.Hints.Y ? m_Value.y : m_Value.x);
        }
    }
}
```

如需动态实例化相机，请参阅 [Cinemachine 多相机](CinemachineMultipleCameras.md) 文档和示例以获取更多信息。