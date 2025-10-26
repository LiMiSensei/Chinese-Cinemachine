# Cinemachine 冲击源（Cinemachine Impulse Sources）

冲击源（Impulse Source）是一种组件，可从场景空间（Scene space）中的某个点发射振动信号。游戏事件可触发冲击源从事件发生的位置发射信号：事件**触发（triggers）** 冲击信号，而冲击源**生成（generates）** 冲击信号。带有“冲击监听器（Impulse Listener）”扩展组件的 Cinemachine 相机，则会通过抖动**响应（react）** 冲击信号。

下图中，角色的脚部是冲击源。当脚部与地面碰撞时（A），会生成冲击信号；相机作为冲击监听器，会通过抖动对冲击信号做出响应（B），进而导致游戏视图（game view）中的画面产生抖动（C）。

![在此场景中，角色的脚部是冲击源。当脚部与地面碰撞时（A），会生成冲击信号；相机作为冲击监听器，会通过抖动对冲击信号做出响应（B），进而导致游戏视图中的画面产生抖动（C）。](images/ImpulseOverview.png)

Cinemachine 内置了两种类型的冲击源组件：
- **[Cinemachine 碰撞冲击源（Cinemachine Collision Impulse Source）](CinemachineCollisionImpulseSource.md)**：响应碰撞和触发区域事件，生成冲击信号。
- **[Cinemachine 冲击源（Cinemachine Impulse Source）](CinemachineImpulseSource.md)**：响应非碰撞类事件，生成冲击信号。

你可以在场景中添加任意数量的冲击源。以下是场景中可能使用冲击源组件的几种示例：
- 添加到巨人的双脚上，使巨人行走时地面产生抖动效果；
- 添加到投射物上，使投射物击中目标爆炸时产生冲击；
- 添加到“凝胶星球”表面，使星球在被物体触碰时产生晃动。

默认情况下，一个冲击源会影响范围内的所有[冲击监听器（Impulse Listener）](CinemachineImpulseListener.md)，但你可以通过[通道过滤（channel filtering）](CinemachineImpulseFiltering.md#ChannelFiltering)设置，让冲击源只影响特定的监听器，而忽略其他监听器。


## 冲击源的核心属性（Key Impulse Source properties）

振动信号决定了相机抖动的基本“形态”，而冲击源还会控制其他几个重要属性，这些属性共同定义了它所生成的冲击信号。

有关所有冲击源属性的描述，以及向场景添加冲击源的操作步骤，请参阅[冲击源（Impulse Source）](CinemachineImpulseSource.md)和[碰撞冲击源（Collision Impulse Source）](CinemachineCollisionImpulseSource.md)组件的相关文档。