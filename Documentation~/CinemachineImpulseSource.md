# Cinemachine 冲击源（Cinemachine Impulse Source）

使用 **Cinemachine 冲击源** 组件，可在非碰撞或非碰撞体触发事件时生成冲击信号。这是一种通用的冲击源，提供了一系列 `GenerateImpulse()` API 方法。这些方法能在指定位置、以指定速度和强度生成冲击信号。你可以直接从游戏逻辑中调用这些方法，也可以将它们与 [Unity 事件（UnityEvents）](https://docs.unity3d.com/Manual/UnityEvents.html) 配合使用。

> [!提示]
> 你可以参考此组件的脚本，作为创建自定义冲击生成类时的示例。


为场景添加 Cinemachine 冲击源的步骤：

1. 选中要触发相机抖动的游戏对象，在其检视面板中点击 **Add Component** 按钮。
2. 依次选择 **Scripts > Cinemachine**，然后选中 **Cinemachine Impulse Source**。


默认情况下，一个冲击源会影响范围内的所有 [冲击监听器（Impulse Listener）](CinemachineImpulseListener.md)，但你可以通过 [通道过滤（channel filtering）](CinemachineImpulseFiltering.md#ChannelFiltering) 设置，让冲击源只影响特定的冲击监听器。


## 属性

Cinemachine 冲击源检视面板中的属性分为以下几个部分：

- [冲击通道（Impulse Channel）](#ImpulseChannel)
- [冲击类型（Impulse Type）](#ImpulseType)
- [冲击形状（Impulse Shape）](#ImpulseShape)


<a name="ImpulseChannel"></a>
### 冲击通道（Impulse Channel）

冲击监听器会根据通道过滤冲击信号，以控制它们对哪些冲击源做出反应。通道的作用类似相机图层（Camera Layers），但二者是不同的概念。以下属性用于控制冲击源广播冲击信号的通道。详情请参阅 [过滤（Filtering）](CinemachineImpulseFiltering.md) 文档。

| 属性 | 功能 |
| :--- | :--- |
| **冲击通道（Impulse Channel）** | 从下拉菜单中选择一个或多个通道。<br /><br />点击 **Edit** 可修改现有通道或添加新通道。 |


<a name="ImpulseType"></a>
### 冲击类型（Impulse Type）

你可以根据需求选择不同复杂程度的类型。更改冲击类型后，会显示相应的范围、衰减和传播速度控制选项。

| 属性 | 功能 |
| :--- | :--- |
| **冲击类型（Impulse Type）** | 可选择以下冲击类型：<ul> <li>**均匀（Uniform）**：冲击以无限速度传播，所有监听器无论在空间中处于什么位置，都会同时以相同方式接收到冲击。</li> <li>**衰减（Dissipating）**：冲击强度随与源的距离增加而减弱。距离较远的监听器感受到的信号比近处的弱。</li> <li>**传播（Propagating）**：除了具有衰减特性外，冲击信号从源向外以有限速度传播。距离较远的监听器会比近处的监听器晚感受到冲击。</li> <li>**旧版（Legacy）**：此模式用于兼容使用早期版本冲击系统的项目，其定义冲击信号的方式更复杂。建议使用其他设置。</li> </ul> |
| **衰减距离（Dissipation Distance）** | 定义冲击衰减的距离范围。超过此距离后，将不会感受到冲击。 |
| **传播速度（Propagation Speed）** | 定义冲击信号从原点向外在空间中传播的速度（单位：米/秒）。默认值 343 为声速。 |
| **衰减率（Dissipation Rate）** | 定义在衰减距离内冲击衰减的速度。如下图所示，展开曲线可看到一个图表，展示了在衰减半径范围内的信号强度。X 轴中心为原点。拖动滑块可调整蓝色曲线的形状。<br /><br>![衰减率设置示例：滑块改变蓝色曲线的形状](images/DissipationRate.png) |


<a name="ImpulseShape"></a>
### 冲击形状（Impulse Shape）

定义指定信号形状的曲线，以及曲线的发射时长。

| 属性 | 功能 |
| :--- | :--- |
| **预设冲击形状（Predefined Impulse Shape）** | 可从以下预设形状中选择：**后坐力（Recoil）**、**撞击（Bump）**、**爆炸（Explosion）** 或 **震动（Rumble）**。<br /><br>**s（秒）** 字段：设置冲击的持续时间。展开该属性可查看冲击的波形图：<br /><br>![示例：选择“撞击（Bump）”时的冲击形状](images/ImpulsePicture.png) |
| **自定义冲击形状（Custom Impulse Shape）** | 可绘制自己的自定义冲击形状（动画曲线）。从下拉菜单中选择 **Custom**，然后点击绿色图标弹出如下编辑器。<br /><br>![自定义冲击形状示例](images/ImpulseShapeCustom.png) |
| **默认速度（Default Velocity）** | 指定冲击默认的空间方向。 |
| **用力量测试（Test with Force）** | 允许在检视面板中（运行时）以指定的力量倍数触发默认冲击，以查看效果。 |