# 管理分组相机

**管理器相机（Manager Camera）** 负责监管多个 Cinemachine 相机（CinemachineCamera），但从 Cinemachine 控制器（Cinemachine Brain）和时间线（Timeline）的角度来看，它本身就相当于一个单独的 Cinemachine 相机。

Cinemachine 包含以下几种管理器相机：

* [序列相机（Sequencer Camera）](CinemachineSequencerCamera.md)：执行其子 Cinemachine 相机的一系列混合过渡（blend）或瞬间切换（cut）。
* [清晰视角相机（Clear Shot Camera）](CinemachineClearShot.md)：选择对子目标视野最佳的子 Cinemachine 相机。
* [状态驱动相机（State-Driven Camera）](CinemachineStateDrivenCamera.md)：根据动画状态的变化选择相应的子 Cinemachine 相机。
* [混合相机（Mixing Camera）](CinemachineMixingCamera.md)：通过对最多 8 个子 Cinemachine 相机进行加权平均，创建连续的混合过渡效果。

由于管理器相机的行为与普通 Cinemachine 相机类似，因此可以进行嵌套。也就是说，你可以创建任意复杂的相机系统，将常规 Cinemachine 相机和管理器相机组合使用。


## 创建自定义管理器相机

你也可以创建自定义的管理器相机，使其根据你提供的特定算法来选择当前激活的子相机。例如，在制作 2D 平台游戏时，若希望相机系统能根据角色是向左/向右移动、跳跃还是下落，以不同方式进行构图，那么自定义相机管理器类会是一个不错的方案。

创建自定义管理器相机的步骤如下：

1. 新建一个继承自 `CinemachineCameraManagerBase` 的类。该基类实现了子 Cinemachine 相机数组和混合器（blender）的功能。

2. 实现抽象方法 `ChooseCurrentCamera`。当管理器处于激活状态时，每帧都会调用该方法，且该方法应返回当前帧应激活的子相机。你的自定义类可以通过任意方式做出这一决策。在上述示例中，该方法会检测玩家状态（如朝向、跳跃/下落状态），并选择相应的子相机。

如果新选中的目标相机与上一帧的相机不同，`CinemachineCameraManagerBase` 会根据在“默认混合（DefaultBlend）”和“自定义混合（CustomBlends）”字段中的设置，启动混合过渡。

为每个玩家状态添加带有相应设置的子相机，并将它们关联到你的管理器实例后，你就拥有了一个能根据玩家状态自动调整的 Cinemachine 相机系统。该系统在整个 Cinemachine 中就像一个普通的 Cinemachine 相机，可在任何支持 Cinemachine 相机的地方使用，包括嵌套在其他相机系统中。

需要注意的是，Cinemachine 内置了[状态驱动相机（State-Driven Camera）](CinemachineStateDrivenCamera.md)，只要相关玩家状态是通过动画控制器状态机（Animation Controller State-Machine）编码的，它就能实现上述功能。只有当状态不是从动画控制器中读取时，才需要实现自定义管理器。


### 被管理的相机必须是管理器的游戏对象子级
这主要是为了避免因嵌套管理器而导致递归循环的问题。强制被管理的相机作为子级，可以防止递归情况的发生。