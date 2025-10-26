# Cinemachine 冲击系统（Cinemachine Impulse）

Cinemachine 冲击系统可根据游戏事件生成并管理相机抖动效果。例如，当一个游戏对象（GameObject）与另一个发生碰撞，或场景中出现爆炸等情况时，你可以通过冲击系统让 Cinemachine 相机产生抖动。

冲击系统包含两部分：

**1. [冲击源（Impulse Source）](CinemachineImpulseSourceOverview.md)**：一种组件，能从空间中的某个点发出信号并向外传播，类似声波或冲击波。这种信号的发射由游戏中的事件触发。

信号包含“方向”和“强度时间曲线”两部分——后者是一条用于定义信号强度随时间变化的曲线。两者结合，可明确实现“沿指定轴进行抖动、持续指定时长”的效果。该抖动从原点向外扩散，当传播到“冲击监听器”所在位置时，监听器会对其做出响应。

**2. [冲击监听器（Impulse Listener）](CinemachineImpulseListener.md)**：Cinemachine 的一款扩展组件，可让 Cinemachine 相机“感知”到冲击信号，并通过抖动做出反应。

将整个系统理解为一个个独立的“冲击事件（impulses）”会更清晰：冲击事件是冲击源发射信号的单次发生过程。场景中的碰撞、事件等**触发**冲击事件，冲击源**生成**冲击事件，而冲击监听器**响应**冲击事件。


## 冲击系统入门（Getting started with Impulse）

若要在场景中设置并使用冲击系统，请按以下步骤操作：

1. 为一个或多个你希望触发相机抖动的游戏对象，添加 **[Cinemachine 冲击源（Cinemachine Impulse Source）](CinemachineImpulseSource.md)** 或 **[Cinemachine 碰撞冲击源（Cinemachine Collision Impulse Source）](CinemachineCollisionImpulseSource.md)** 组件。

2. 为一个或多个 Cinemachine 相机（CinemachineCamera）添加 **[Cinemachine 冲击监听器（Cinemachine Impulse Listener）](CinemachineImpulseListener.md)** 扩展组件，使其能够检测冲击信号并做出响应。


### 术语补充说明（游戏开发场景适配）
- **camera shake**：相机抖动，指通过改变相机位置或旋转角度，模拟物理冲击（如碰撞、爆炸）带来的画面晃动效果，增强游戏沉浸感；
- **signal propagates outwards**：信号向外传播，此处指冲击信号从生成点向周围空间扩散，距离越远，信号强度可能越弱（类似现实中冲击波的传播规律）；
- **strength as a function of time**：强度随时间变化，通过曲线（如“快速增强后缓慢减弱”）定义抖动力度的动态变化，让效果更贴近真实物理现象。