# 控制和自定义混合过渡（Control and Customize Blends）

当活跃相机发生切换时，Cinemachine 提供了多种方式来控制相机间的混合过渡效果。

* 最简单且最常用的方式是通过创建资源，定义特定相机或相机组之间的混合过渡规则。
* 对于高阶用户，可根据游戏事件或其他动态条件来控制混合过渡风格，甚至自定义混合过渡算法，但这些技术需要编写一些自定义代码。


## 默认混合过渡（Default Blend）
最基础的策略是在 [Cinemachine 控制器（Cinemachine Brain）](CinemachineBrain.md) 中设置**默认混合过渡（Default Blend）**。对于所有未被更具体的设置和规则覆盖的混合过渡，Cinemachine 都会使用此混合过渡设置。

**默认混合过渡**不仅存在于 Cinemachine 控制器中，也存在于可控制一组子相机并实现子相机间混合过渡的 Cinemachine 管理类相机中，例如 [清晰视角相机（Clear Shot Camera）](CinemachineClearShot.md) 和 [状态驱动相机（State-Driven Camera）](CinemachineStateDrivenCamera.md)。


## 使用混合器设置资源（Blender Settings Asset）实现自定义混合过渡
所有包含**默认混合过渡（Default Blend）** 属性的 Cinemachine 组件，均同时包含一个**自定义混合过渡（Custom Blends）** 设置，该设置关联着一个 [混合器设置资源（Blender Settings Asset）](CinemachineBlending.md)。

混合器设置资源包含一个混合过渡设置列表，当具有特定名称的相机之间进行混合过渡时，会应用列表中的对应设置。通过该资源，你可以为不同相机间的混合过渡设置混合曲线（blend curves）和混合时长（blend durations），从而精确控制单个相机与其他相机的混合效果。

对于未在**混合器设置（Blender Settings）** 规则中列出的相机，其混合过渡会回退到**默认混合过渡（Default Blend）**。


## 时间线镜头重叠（Timeline Shot Overlapping）
通过在时间线的 [Cinemachine 轨道（Cinemachine Track）](concept-timeline.md) 上[重叠放置 Cinemachine 镜头片段](setup-timeline.md)，时间线会直接控制混合过渡效果。这类混合过渡不受“默认混合过渡（Default Blend）”和“使用混合器设置资源的自定义混合过渡”等其他混合控制设置的影响。

混合过渡时长由片段的重叠范围决定，你可以通过 Cinemachine 镜头片段中指定的缓动曲线（默认是“淡入-淡出”曲线）来控制混合曲线。

> [!注意]
> 只有当 Cinemachine 镜头片段重叠时，才能获得这种精确控制。如果使用 [时间线中的激活轨道（Activation Tracks）](https://docs.unity3d.com/Packages/com.unity.timeline@latest/index.html?subfolder=/manual/insp-trk-act.html) 来激活和停用 Cinemachine 相机，那么这类混合过渡始终会应用标准混合控制设置（即“默认混合过渡”和“使用混合器设置资源的自定义混合过渡”）。


## 混合过渡提示（Blend Hints）
每个 [Cinemachine 相机（Cinemachine Camera）](CinemachineCamera.md) 都有一个**混合过渡提示（Blend Hint）** 属性，该属性会影响当前相机与其他相机的混合过渡方式。它不控制过渡时机，而是影响过渡算法，因此与上述其他控制方式相互独立。

你在该属性中设置的提示，会影响 Cinemachine 对相机位置和旋转的插值计算方式：例如，可控制是否考虑“看向目标（LookAt Target）”，也可控制混合过渡是基于活跃状态下移动的相机，还是基于过渡开始时对即将退出的相机所做的快照。


## `CinemachineCore.GetBlendOverride` 委托
每当 Cinemachine 创建混合过渡时，都会调用 `CinemachineCore.GetBlendOverride` 委托。这使你有机会根据任意动态条件，覆盖该混合过渡的设置。

如果你为该委托注册了处理函数，该函数可以检查游戏上下文（如当前游戏状态、角色行为等），并决定是保留混合过渡的原始设置，还是覆盖混合过渡风格、混合时长或混合算法等参数。

这是一种高阶技术，需要通过编写脚本来实现事件处理函数。

> [!注意]
> 对于通过时间线镜头片段重叠创建的混合过渡，不会调用此委托。因为时间线对混合过渡的控制被设计为“精确可控”，且这种控制无法被覆盖。


## `CinemachineCore.GetCustomBlender` 委托
最高阶的控制方式是直接自定义混合过渡算法本身。Cinemachine 拥有一套复杂的相机状态插值算法（`CameraState.Lerp()`），该算法会在考虑混合过渡提示和“看向目标（LookAt Target）”屏幕位置的同时，对相机的位置、旋转、镜头设置及其他属性进行插值计算。

默认情况下，这些属性的插值速率相同，因此位置、旋转和镜头参数会同步变化。但在某些场景下，你可能需要特殊效果——例如让旋转先完成、让位置沿特定路径移动，或实现其他 Cinemachine 原生不支持的需求。

此时，你可以创建一个自定义混合器（custom blender），使其实现 `CinemachineBlend.IBlender` 接口。通过挂钩（hook）`CinemachineCore.GetCustomBlender` 委托，你可以将该自定义混合器提供给 Cinemachine。无论混合过渡是如何创建的（即使是通过时间线创建），Cinemachine 都会在创建混合过渡时调用此委托。你可以检查待混合的相机及其他任意状态，然后决定提供自定义混合器，或返回 null 以使用默认混合器。

Cinemachine 提供了两个 [示例场景](samples-tutorials.md)——`Early LookAt Custom Blend`（提前看向目标的自定义混合）和 `Perspective To Ortho Custom Blend`（透视到正交的自定义混合），用于演示该技术的实现。使用此功能需要具备一定的编码能力。


## 其他资源（Additional Resources）
* [相机控制与过渡](concept-camera-control-transitions.md)
* [Cinemachine 与时间线](concept-timeline.md)