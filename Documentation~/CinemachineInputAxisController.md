# Cinemachine 输入轴控制器（Cinemachine Input Axis Controller）

Cinemachine 相机不直接处理用户输入。相反，它们会暴露一些轴，这些轴可由脚本、动画或用户输入来**驱动**。Cinemachine 尽可能不限制输入的来源，这样它就能与 Unity 的 Input 包、Unity 的旧版输入管理器或其他第三方输入系统兼容。

Cinemachine 中包含了 CinemachineInputAxisController 组件。当你将其添加到 Cinemachine 相机时，它会自动检测所有可由用户输入驱动的轴，并暴露相关设置，让你能够控制这些轴的值。

该组件兼容 Unity 的 Input 包和旧版输入管理器。你也可以将其作为模板，编写自己的自定义输入处理器。

输入轴控制器不仅能将输入映射到暴露的轴上，还为每个轴提供了设置，可通过加速度/减速度和增益来调整响应性。

你也可以根据需要，将 CinemachineInputAxisController 与自己的脚本结合使用来驱动输入轴，例如在实现玩家移动的脚本中。有关示例，请参见 Cinemachine 的示例场景。


## 用法（Usage）

此组件可轻松在单人环境中通过鼠标、键盘或控制器控制 `CinemachineCamera`。


## 属性（Properties）：

| **属性** | **功能** |
|:---|:---|
| **玩家索引（Player Index）** | 要查询的玩家输入控制。单人游戏保持默认值 -1 即可。否则，此处应填写该玩家在 `UnityEngine.Input.InputUser.all` 列表中的索引。此设置仅在安装了 Unity 的 Input 包时显示。 |
| **自动启用输入（Auto Enable Inputs）** | 如果安装了 Unity 的 Input 包，则会显示此选项。它会在启动时自动启用所有映射的输入动作。 |
| **递归扫描（Scan Recursively）** | 若启用，将递归搜索 IInputAxisOwners 行为；否则，仅考虑直接附加到此游戏对象的行为，忽略子对象。 |
| **混合时抑制输入（Suppress Input While Blending）** | 若启用且此组件附加到 Cinemachine 相机，则相机参与混合时不会处理输入。 |
| **忽略时间缩放（Ignore Time Scale）** | 若启用，输入处理将使用未缩放的 deltaTime，而非缩放后的 deltaTime。这使得即使时间缩放设置为 0，输入也能继续响应。 |
| **启用（Enabled）** | 当此值为 true 时，控制器将驱动输入轴；若为 false，输入轴将不会被控制器驱动。 |
| **旧版输入（Legacy Input）** | 如果使用旧版输入管理器，此处指定要查询的输入轴名称。 |
| **旧版增益（Legacy Gain）** | 如果使用旧版输入管理器，读取的输入值将乘以该数值。 |
| **输入动作（Input Action）** | 如果使用 Unity Input 包，此处设置用于驱动轴的输入动作引用。 |
| **增益（Gain）** | 如果使用 Unity Input 包，读取的输入值将乘以该数值。 |
| **输入值（Input Value）** | 本帧读取的输入值。 |
| **加速时间（Accel Time）** | 输入值加速到更大值所需的时间。 |
| **减速时间（Decel Time）** | 输入值减速到更小值所需的时间。 |
| **取消 deltaTime 补偿（Cancel Delta Time）** | 这将取消输入轴内置的 deltaTime 补偿。如果输入值本身依赖于帧时间，请启用此选项。例如，鼠标增量在较长的帧中自然会更大，这种情况下应取消默认的 deltaTime 缩放。 |


## 创建自定义输入轴控制器（Creating your own Input Axis Controller）

`CinemachineInputAxisController` 的默认实现可处理来自 Input 包和 Unity 旧版输入系统的输入源。

对于更复杂的场景（如移动设备控制），你可以扩展此默认功能，通过脚本创建自己的输入轴控制器。

以下示例展示了如何使用自定义输入控制器脚本，通过滑块控制移动设备上的相机。该示例代码可作为模板，且易于修改以用于其他对象。

```cs
using UnityEngine;
using Unity.Cinemachine;
using System;
using UnityEngine.UI;
using Object = UnityEngine.Object;

// 你将添加到 Cinemachine 相机的组件
public class SliderInputController : InputAxisControllerBase<SliderInputController.SliderReader>
{
    void Update()
    {
        if (Application.isPlaying)
            UpdateControllers();
    }

    [Serializable]
    public class SliderReader : IInputAxisReader
    {

        public Slider m_Slider;

        public float GetValue(Object context, IInputAxisOwner.AxisDescriptor.Hints hint)
        {
            if (m_Slider is not null)
                return m_Slider.value;

            return 0;
        }
    }
}
```

如需了解更多信息，若你需要使用 Input System 包设置本地多人输入，请参见 [输入系统组件（Input System Components）](InputSystemComponents.md) 文档。