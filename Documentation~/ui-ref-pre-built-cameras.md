# 预构建的 Cinemachine 相机

Cinemachine 包包含一系列针对特定使用场景的预构建 Cinemachine 相机快捷方式。

若要使用预构建的 Cinemachine 相机，可从编辑器菜单中选择 **游戏对象（GameObject）> Cinemachine**，然后根据以下列表选择相应的相机类型：

| 菜单项 | 说明 |
| :--- | :--- |
| **状态驱动相机（State-Driven Camera）** | 创建一个管理器相机（Manager Camera），该相机可作为一组 Cinemachine 相机的父对象，并能根据[动画状态变化](CinemachineStateDrivenCamera.md)对这些相机进行管理。 |
| **目标相机（Targeted Cameras）> 跟随相机（Follow Camera）** | 创建一个预配置了适用行为的 Cinemachine 相机，适用于[跟随角色并为其构图](setup-follow-camera.md)的场景。 |
| **目标相机（Targeted Cameras）> 目标组相机（Target Group Camera）** | 创建一个预配置了适用行为和扩展组件的 Cinemachine 相机，适用于[跟随群组并为其构图](GroupingTargets.md)的场景。同时会创建一个空的目标组（Target Group），并将其分配给新 Cinemachine 相机的“跟踪目标（Tracking Target）”字段。 |
| **目标相机（Targeted Cameras）> 自由视角相机（FreeLook Camera）** | 创建一个预配置了适用行为和扩展组件的 Cinemachine 相机，适用于[需用户输入的、以角色为中心的自由视角场景](FreeLookCameras.md)。 |
| **目标相机（Targeted Cameras）> 第三人称瞄准相机（Third Person Aim Camera）** | 创建一个预配置了适用位置行为和瞄准扩展组件的 Cinemachine 相机，适用于[第三人称瞄准场景](ThirdPersonCameras.md)。第三人称瞄准相机是一个固定装备（rig），由跟踪目标的旋转和位置驱动，不提供直接的用户控制功能。若需可由用户控制相机位置的相机，请选择**自由视角相机（FreeLook Camera）**。 |
| **目标相机（Targeted Cameras）> 2D 相机（2D Camera）** | 创建一个预配置了适用[位置构图器行为](CinemachinePositionComposer.md)的 Cinemachine 相机，适用于 2D 游戏场景。 |
| **Cinemachine 相机（Cinemachine Camera）** | 创建一个未预配置任何行为的默认 Cinemachine 相机。<br />可用于[创建被动式 Cinemachine 相机](setup-cinemachine-environment.md)，或[从零开始构建自定义 Cinemachine 相机](setup-procedural-behavior.md)。该相机的位置和旋转会与当前场景视图相机保持一致。 |
| **序列相机（Sequencer Camera）** | 创建一个管理器相机（Manager Camera），该相机可作为一组 Cinemachine 相机的父对象，并能根据[指定序列](CinemachineSequencerCamera.md)对这些相机进行管理。 |
| **带样条线的移动相机（Dolly Camera with Spline）** | 创建一个预配置了适用行为的 Cinemachine 相机，使其能[沿样条线（Spline）移动](CinemachineUsingSplinePaths.md)。同时会创建一条样条线，并将其分配给该相机。你可以修改此样条线，也可以用其他样条线替换它。 |
| **带样条线的移动小车（Dolly Cart with Spline）** | 创建一个空的游戏对象，其[变换（Transform）受样条线约束](CinemachineSplineCart.md)。<br />可用于让任意游戏对象沿路径动画，或作为 Cinemachine 相机的跟踪目标。同时会创建一条样条线，并将其分配给移动小车。你可以修改此样条线，也可以用其他样条线替换它。 |
| **混合相机（Mixing Camera）** | 创建一个管理器相机（Manager Camera），该相机可作为一组 Cinemachine 相机的父对象，并能[根据指定权重提供连续的混合过渡效果](CinemachineMixingCamera.md)。 |
| **清晰视角相机（ClearShot Camera）** | 创建一个管理器相机（Manager Camera），该相机可作为一组 Cinemachine 相机的父对象，并能根据[镜头质量标准](CinemachineClearShot.md)从这些相机中选择最优镜头。 |


> [!注意]
> 若你在某个游戏对象上右键点击，并通过弹出菜单创建上述任意一种目标相机，新相机的“跟踪目标（Tracking Target）”会自动填充为你右键点击的那个对象。