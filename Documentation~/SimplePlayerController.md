# 简易玩家控制器（Simple Player Controller）

Cinemachine 的简易玩家控制器是一套脚本集合，可通过组合和配置来创建角色控制器，用于不同场景以实现多种角色移动类型。这些脚本均作为教学示例代码提供，旨在为你提供一个起点，你可以根据自身需求对其进行修改和定制。

根据需求，简易玩家控制器既可以与 Unity 的 [角色控制器（Character Controller）](https://docs.unity3d.com/ScriptReference/CharacterController.html) 行为配合使用，也可以独立工作。当存在角色控制器时，角色移动和地面状态将由角色控制器处理；否则，简易玩家控制器会自行管理位置，并通过射线检测（raycast）确定地面位置。

简易玩家控制器提供了待机（Idle）、行走（Walk）、冲刺（Sprint）和跳跃（Jump）功能。

玩家的前进方向可以锁定为相机的前进方向（横移模式），也可以独立于相机（自由视角模式），还可以是两者的结合（移动时锁定）。重力方向可以锁定为世界坐标系的上方向，也可以跟随玩家的局部上方向，从而允许玩家在墙壁和天花板上行走。


## 用户输入（User Input）

用户输入可分为以下三种模式：

- **相机空间（Camera Space）**：前进方向为相机的前进方向
- **世界空间（World Space）**：前进方向为世界坐标系的前进方向
- **玩家空间（Player Space）**：前进方向为玩家自身的前进方向

用户输入的配置方式与 Cinemachine 相机相同：通过 InputAxis 成员和 [Cinemachine 输入轴控制器（CinemachineInputAxisController）](CinemachineInputAxisController.md) 行为实现。这样做是为了确保控制器与输入实现方式无关，既适用于 Unity 的 Input 包和旧版输入管理器，也可适配第三方输入管理器。这是其与 Cinemachine 唯一的依赖关系，除此之外，简易角色控制器是一个独立的解决方案。


## 横移模式（Strafe Mode）

控制器可以处于横移模式或非横移模式，且该模式状态可动态切换。在横移模式下，玩家不会转向移动方向，而是可以向侧面或后方移动；否则，玩家会转向自身的移动方向。


## 支持第三人称跟随（Support for ThirdPersonFollow）

默认情况下，简易玩家控制器（SimplePlayerController）不了解也不控制相机，相机相关功能由 Cinemachine 负责。但 Cinemachine 的 [第三人称跟随（ThirdPersonFollow）](CinemachineThirdPersonFollow.md) 组件会将相机视角的控制权委托给被跟随的对象。为支持这种情况，你可以向玩家添加一个“玩家瞄准核心（Player Aiming Core）”子游戏对象，并为其添加简易玩家瞄准控制器（SimplePlayerAimController）行为，该行为会与简易玩家控制器配合工作，控制相机视角和瞄准方向。


## 架构（Architecture）

除用户输入外，该控制器与 Cinemachine 无其他依赖关系，也不关心相机的实现方式。它只需知道从哪个相机提取输入坐标系（即明确“前进方向”的定义），为此，它仅使用 `Camera.main`（或你可能提供的替代 `Camera` 对象）。

简易玩家控制器是一套行为组件的集合，每个组件负责角色移动的特定部分。你可以通过混合搭配这些行为来创建所需的角色控制器。下面详细介绍这些行为：


### SimplePlayerControllerBase

这是 SimplePlayerController 和 SimplePlayerController2D 的基类，你也可以将其用作自定义控制器的基类。它提供以下服务和设置：

**服务（Services）**：
- 2D 运动轴（MoveX 和 MoveZ）
- 跳跃按钮（Jump button）
- 冲刺按钮（Sprint button）
- 横移模式的 API

**动作（Actions）**：
- PreUpdate - 在 `Update()` 开始时调用
- PostUpdate - 在 `Update()` 结束时调用
- StartJump - 在玩家开始跳跃时调用
- EndJump - 在玩家结束跳跃时调用

**事件（Events）**：
- Landed - 在玩家着陆时调用

**设置（Settings）**：

| 设置 | 说明 |
| :--- | :--- |
| **移动速度（Speed）** | 行走时的地面移动速度。 |
| **冲刺速度（Sprint Speed）** | 冲刺时的地面移动速度。 |
| **跳跃速度（Jump Speed）** | 跳跃时的初始垂直速度。重力会逐渐减小该速度，直到速度变为负值，玩家开始下落。 |
| **冲刺跳跃速度（Sprint Jump Speed）** | 与跳跃速度类似，但数值可以不同，用于实现更强的跳跃效果。 |


### SimplePlayerController

基于 SimplePlayerControllerBase，这是 3D 角色控制器。它控制角色移动和跳跃，但不管理任何动画。若需动画功能，可添加简易玩家动画器（SimplePlayerAnimator）组件（或其自定义变体）。

SimplePlayerController 提供以下服务和设置：
- 阻尼（应用于玩家的速度和旋转）
- 横移模式（Strafe Mode）
- 重力（Gravity）
- 输入坐标系（用于解析输入的参考坐标系：相机、世界或玩家）
- 地面检测（使用射线检测或委托给角色控制器）
- 相机替代（仅用于确定输入坐标系的相机）

该行为应附加到玩家游戏对象的根节点，它会移动游戏对象的变换（transform）。如果游戏对象还具有 Unity 角色控制器（Character Controller）组件，简易玩家控制器会将地面状态和移动委托给该组件；如果没有，简易玩家控制器会自行管理移动，并通过射线检测判断地面状态。

简易玩家控制器会在所选参考坐标系的上下文中尽力解析用户输入，通常效果良好。但在相机模式下，玩家可能会从相对于相机直立的状态过渡到倒置状态，此时输入解析可能会出现不连续。简易玩家控制器有一种专门的技术来解决这种不连续性（你可以在代码中看到），但仅在这种特定情况下使用。

**附加设置（Additional Settings）**：

| 设置 | 子项 | 说明 |
| :--- | :--- | :--- |
| **阻尼（Damping）** |  | 玩家改变速度或旋转时的过渡持续时间（以秒为单位）。 |
| **横移（Strafe）** |  | 使玩家在侧向移动时横移，否则玩家会转向移动方向。 |
| **输入前进方向（Input Forward）** |  | 输入控制的参考坐标系。 |
|  | _相机（Camera）_ | 输入前进方向为相机的前进方向。 |
|  | _玩家（Player）_ | 输入前进方向为玩家自身的前进方向。 |
|  | _世界（World）_ | 输入前进方向为世界坐标系的前进方向。 |
| **上方向模式（Up Mode）** |  | 用于计算运动的上方向。 |
|  | _玩家（Player）_ | 在玩家的局部 XZ 平面内移动。 |
|  | _世界（World）_ | 在全局 XZ 平面内移动。 |
| **相机替代（Camera Override）** |  | 若非空，则从该相机获取输入坐标系，而非 `Camera.main`。适用于分屏游戏。 |
| **地面层（Ground Layers）** |  | 通过射线检测进行地面检测时包含的层。 |
| **重力（Gravity）** |  | 向下方向的重力大小（米/秒²）。 |


### SimplePlayerController2D

这是 SimplePlayerControllerBase 的一个非常基础的 2D 实现。它要求玩家游戏对象上有 [Rigidbody2D](https://docs.unity3d.com/ScriptReference/Rigidbody2D.html) 组件。由于它与 Rigidbody2D 配合工作，运动控制在 `FixedUpdate()` 方法中实现。仅当玩家脚部有一个小的触发碰撞器（trigger collider）时，地面检测才能生效。

**附加设置（Additional Settings）**：

| 设置 | 说明 |
| :--- | :--- |
| **玩家几何（Player Geometry）** | 指向持有玩家可见几何的子对象的引用。该对象会旋转以面向移动方向。 |
| **空中运动控制（Motion Control While In Air）** | 允许在角色在空中时影响运动方向；否则，适用更符合现实的规则（必须脚触地面才能控制）。 |


### SimplePlayerAnimator

该行为的作用是根据玩家的运动驱动动画。它是一个示例实现，你可以对其进行修改或替换为自己的实现。默认情况下，它硬编码为与示例 `CameronSimpleController` 动画控制器配合工作，该控制器已设置简易玩家动画器已知的状态。你可以修改简易玩家动画器以适配自己的动画控制器。

简易玩家动画器可以与 SimplePlayerControllerBase 配合使用，也可以独立使用。独立使用时，它会监控变换组件的位置并相应地驱动动画。在一些示例场景（如 RunningRace 或 ClearShot）中可以看到这种用法，在这种模式下，它无法检测玩家的地面状态，因此始终假设玩家在地面上。

当检测到 SimplePlayerControllerBase 时，简易玩家动画器会安装回调，并期望由 SimplePlayerControllerBase 通过 StartJump、EndJump 和 PostUpdate 回调来驱动。

动画片段的速度可以通过以下设置控制。默认情况下，这些设置已针对提供的示例动画进行了调整，以确保玩家移动时脚部不会在地面上滑动。请注意：这些调整专门针对提供的示例动画，若替换动画，应重新调整这些值。

**调整设置（Tuning Settings）**：

| 设置 | 说明 |
| :--- | :--- |
| **正常行走速度（Normal Walk Speed）** | 根据模型中的动画调整此值：以该速度行走时，脚部不应滑动。 |
| **正常冲刺速度（Normal Sprint Speed）** | 根据模型中的动画调整此值：以该速度冲刺时，脚部不应滑动。 |
| **最大冲刺缩放（Max Sprint Scale）** | 冲刺动画的速度不应超过此值，以避免不合理的快速移动。 |
| **跳跃动画缩放（Jump Animation Scale）** | 跳跃动画整体速度的缩放因子。 |


### SimplePlayerAimController

该行为与简易玩家控制器配合工作，控制玩家的一个不可见子对象的旋转。它旨在与 Cinemachine 的第三人称跟随（ThirdPersonFollow）组件配合使用，且该子对象用作 Cinemachine 相机的跟踪目标（Tracking Target）。在这种情况下，简易玩家瞄准控制器会根据用户输入控制相机视角。

该组件应位于具有简易玩家控制器（SimplePlayerController）行为的玩家的子对象（“瞄准核心”）上，并与其紧密配合。瞄准核心的目的是将相机旋转与玩家旋转解耦：相机旋转由瞄准核心游戏对象的旋转决定，而该行为提供用于控制旋转的输入轴。

当瞄准核心用作带有第三人称跟随组件的 Cinemachine 相机的目标时，相机会沿核心的前进轴观察，并围绕核心的原点旋转。若玩家具备射击能力，瞄准核心还用于定义玩家射击的原点和方向。要实现玩家射击功能，可向瞄准核心游戏对象添加简易玩家射击（SimplePlayerShoot）行为。

“带瞄准模式的第三人称（ThirdPersonWithAimMode）”示例场景展示了其设置方法。

**设置（Settings）**：

| 设置 | 子项 | 说明 |
| :--- | :--- | :--- |
| **玩家旋转（Player Rotation）** |  | 玩家旋转与相机旋转的耦合方式。 |
|  | _耦合（Coupled）_ | 玩家随相机旋转，侧向移动会导致横移。 |
|  | _移动时耦合（Coupled When Moving）_ | 玩家静止时，相机可围绕玩家自由旋转；但玩家开始移动时，会转向相机前进方向。 |
|  | _解耦（Decoupled）_ | 玩家旋转独立于相机旋转。 |
| **旋转阻尼（Rotation Damping）** |  | 当玩家开始移动时，旋转至面向相机方向的速度。仅在玩家旋转模式为“移动时耦合”时使用。 |
| **水平视角（Horizontal Look）** |  | 水平旋转输入轴。值以度为单位，0 为居中。 |
| **垂直视角（Vertical Look）** |  | 垂直旋转输入轴。值以度为单位，0 为居中。 |


### SimplePlayerShoot

该组件管理玩家射击功能，应位于玩家对象上，或玩家的子对象简易玩家瞄准控制器（SimplePlayerAimController）上。

如果指定了瞄准目标管理器（AimTargetManager），则该行为会瞄准该目标；否则，会沿玩家对象的前进方向瞄准，或如果存在简易玩家瞄准控制器且未与玩家旋转解耦，则沿其前进方向瞄准。

**设置（Settings）**：

| 设置 | 说明 |
| :--- | :--- |
| **子弹预制体（Bullet Prefab）** | 射击时实例化的子弹预制体。 |
| **每秒最大子弹数（Max Bullets Per Sec）** | 每秒最大射击子弹数。 |
| **射击（Fire）** | 用于射击的布尔输入轴。值为 0（未射击）或 1（正在射击）。 |
| **瞄准目标管理器（AimTarget Manager）** | 要瞄准的目标。若为空，则瞄准方向由该游戏对象的前进向量定义。 |
| **射击事件（Fire Event）** | 射击时触发的事件。 |


### SimplePlayerOnSurface

该行为使玩家在表面上保持直立，可用于实现玩家在墙壁、天花板或任意网格表面上行走的效果。它会旋转玩家，使其上方向与所在表面的法线方向一致。此脚本假设玩家的 pivot 点在底部。

“球面自由视角（FreeLook on Spherical Surface）”示例场景展示了该行为的使用方法。

它通过射线检测来识别可行走表面。

使用该组件时，简易玩家控制器（SimplePlayerController）的上方向模式（Up Mode）应设置为“玩家（Player）”，且不应有角色控制器（Character Controller）组件，因为角色控制器与非标准上方向兼容性不佳。

此外，当使用 Cinemachine 相机跟踪角色时，[Cinemachine 控制器（CinemachineBrain）](CinemachineBrain.md) 的“世界上方向替代（World Up Override）”设置应设为玩家，以使相机的上方向与玩家的上方向一致。

**设置（Settings）**：

| 设置 | 说明 |
| :--- | :--- |
| **旋转阻尼（Rotation Damping）** | 玩家旋转以匹配表面法线的速度。 |
| **地面层（Ground Layers）** | 视为地面的层。 |
| **最大射线检测距离（Max Raycast Distance）** | 检测地面时射线的最大距离。 |
| **玩家高度（Player Height）** | 玩家的大致高度，用于计算射线检测的起点。 |
| **自由下落恢复（Free Fall Recovery）** | 使玩家在自由下落时向最近的表面下落。 |
| **表面改变（Surface Changed）** | 当玩家从一个表面移动到另一个表面时触发此事件。如果表面在移动，这是重新设置玩家父对象的好时机。 |