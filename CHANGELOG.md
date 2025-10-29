# 更新日志 (Changelog)
本包所有显著更改都将记录在此文件中。

格式基于 [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)，
并且本项目遵循 [Semantic Versioning](http://semver.org/spec/v2.0.0.html)。

## [3.1.5] - 2025-10-21

### 错误修复
- 当从进行中的混合退出时，并非总是使用正确的混合时间。
- CinemachineVolumeSettings：当启用自动对焦时，对焦距和光圈设置的更改未生效。
- InheritPosition 并非在所有情况下都继承相机位置。
- 当 Orbital Follow 的绑定模式为 Lazy Follow 时，未应用 Rotation Composer 阻尼。
- GroupFraming 扩展未遵守 PreviousStateIsValid 标志，因此无法动态重置。
- 当在没有注视目标的第三人称相机和具有注视目标的相机之间混合时，混合不正确。
- ForceCameraPosition 对于 LazyFollow 相机未正常工作。
- 传送具有非零目标偏移量的相机时不顺畅。
- 当删除 OrbitalFollow 组件时，有时会生成异常。
- 在游戏模式域重新加载后，输入轴控制器丢失用户输入。
- 输入轴控制器并非总是正确读取复合动作。
- CinemachineConfiner2D.BakeBoundingShape() 有时会使 Confiner 留下未完全烘焙的形状。

### 新增
- 添加了 `CinemachineConfiner2D.CameraWasDisplaced()` 和 `CinemachineConfiner2D.GetCameraDisplacementDistance()` 方法。
- 添加了 `InputAxisControllerBase.GetController()` 方法，以便于获取具有特定名称的输入控制器。
- 添加了 `InputAxisControllerBase.TriggerRecentering()` 方法，以触发具有特定名称的轴的重新居中。
- 在输入轴检查器中添加了"重新居中"按钮，可立即使轴居中。
- 添加了 PerspectiveToOrthoCustomBlend 示例场景。
- 添加了新的 Portals 示例场景，以演示相机传送。

### 更改
- 输入轴重新居中仅在游戏运行时操作。

## [3.1.4] - 2025-06-10

### 错误修复
- 解除遮挡器并非总是正确重置其状态。
- 当 FreeLook 轨道大小改变时，解除遮挡器和解除碰撞器会引入虚假的阻尼。
- 仅限 Mac：Cinemachine 检查器中的某些下拉菜单和弹出窗口无法稳定工作。
- 回归修复：当相机新激活时，Confiner2D 并非总是进行限制。
- RotationComposer 和 PositionComposer 不再对来自 FreeLookModifier 的构图变化进行阻尼。
- 修复了导入时示例资源的选择，取决于当前的渲染管线和输入配置。您需要重新导入示例以修复任何现有问题。
- 由于精度问题，Confiner2D 对于大尺寸边界框的行为不一致。
- StateDrivenCamera 检查器未正确填充指令列表中的状态。
- 修复了导致 ForceCameraPosition 行为不符合预期的一些问题。
- 最大化包含自定义混合的检查器会生成控制台错误并导致检查器无响应。

### 更改
- Timeline 的 Cinemachine Shot Editor 在编辑预制件时不再显示不正确的 UX 来创建相机。
- 当 FreeLookModifier 更改构图时，游戏视图的 Composer 参考线会动态反映当前构图。
- 当为构图启用 FreeLookModifier 时，游戏视图的 Composer 参考线不再可拖动。

### 新增
- 向 FreeLookModifier 添加了缓动设置，以平滑 FreeLook 上下半球之间的过渡。
- 为 `CinemachineBrain.ManualUpdate` 添加了一个重载，该重载接受自定义帧计数和 deltaTime，允许对游戏循环进行更自定义的控制。
- 添加了 `CINEMACHINE_TRANSPARENT_POST_PROCESSING_BLENDS` 脚本定义以调整后处理混合行为。
- 向 Impulse Listener 添加了信号组合模式，允许以不同方式组合多个脉冲信号。

## [3.1.3] - 2025-02-01

### 错误修复
- 回归修复：CinemachinePanTilt 重新居中忽略了轴中心设置。
- 当一个混合在完成前中断另一个混合时，CameraDeactivated 事件未能一致发送。
- 当激活是由于时间线混合引起时，CameraActivated 事件未能一致发送。
- 带有死区的 FramingTransposer 有时会漂移。
- Decollider 有时会导致相机滑入相邻碰撞器之间的缝隙。
- Deoccluder 在初始启用时未能重置其状态，并且有时会导致小的虚假相机旋转。
- 修复了 CinemachineOrbitalFollow 组件中的 Radial Axis 输入轴映射到 y 轴的问题。
- 当使用异构混合进入和混合退出时间中断混合时，会遵守期望的混合时间。

### 更改
- 为 CinemachineCamera 镜头的近裁剪平面和远裁剪平面检查器字段添加了延迟处理。
- 更新了 CinemachineDefaultInputActions 资源中的游戏手柄输入，以更接近标准游戏手柄惯例。将 Player 动作映射重命名为 CM Default。

### 新增
- CinemachineConfiner2D.BoundingShapeIsBaked 可用于检查 Confiner 的边界形状是否已烘焙。
- CinemachineConfiner2D.BakeBoundingShape() 可用于强制完成 Confiner 对边界形状的烘焙。

## [3.1.2] - 2024-10-01

### 新增
- 添加了 CinemachineVirtualCameraBaseEditor，以便于为自定义虚拟相机和虚拟相机管理器制作符合规范的检查器。
- 向 CinemachineConfiner2D 添加了 Padding 选项。
- 添加了 CinemachineSplineSmoother，用于创建适用于相机路径的平滑样条线。这复制了 CM2 中 CinemachineSmoothPath 的行为。
- 向 CinemachineSplineRoll 添加了 Easing 选项。
- 向 CinemachineSplineRoll 添加了 SaveDuringPlay 支持。
- 添加了偏好设置选项以显示样条法线而不是轨道式 Gizmo。

### 更改
- 跟踪目标的存在不再影响 CinemachineCamera 状态的位置和旋转是否被推回变换组件。
- TargetPositionCache.GetTargetPosition() 和 TargetPositionCache.GetTargetRotation() 现在是公共的，以便自定义类支持缓存的时间线擦洗。

### 错误修复
- 有时深度嵌套的被动相机的位置会因精度不准确而逐渐偏移。
- 当从过场动画退出时，有时无法恢复原始相机。
- 样条滚动方向被反转。
- 升级程序有时无法删除所有过时的组件。
- 路径升级程序使用 Broken 而不是 Mirrored 模式创建节点，并且未正确设置节点旋转。这导致样条向上向量并非总是平滑的。

## [3.1.1] - 2024-06-15

### 新增
- 新的旋转控制行为 SplineDollyLookAtTargets 允许您沿着样条轨道轨迹在所需位置指定注视点。
- 向 CinemachinePanTilt 添加了 Recenter Target 设置。

### 修复
- 错误修复：InputAxis.TriggerRecentering() 函数导致轴立即捕捉到其重新居中值。
- 错误修复：当存在多个 CM Brain 时，FixedUpdate 相机有时更新过于频繁，导致抖动运动。
- 回归修复：StateDrivenCamera 检查器未能正确设置指令列表中的状态。
- 错误修复：Decollider 在解析障碍物时忽略了地形层。
- 错误修复：CinemachineTargetGroup 中的 GroupAverage 旋转模式计算不正确。
- 错误修复：在 CinemachineTargetGroup.WeightedMemberBoundsForValidMember 中添加了缺失的空值检查。
- 错误修复：在 CinemachineDeoccluder.PushCameraBack() 中添加了缺失的空值检查。
- 错误修复：Cinemachine 无法与缩放样条正常工作。

### 更改
- CinemachineGroupFraming 现在具有兼容模式，因此可以开箱即用地与 CinemachineConfiner2D 配合工作。
- 移除了 FadeOutNearbyObjects 示例场景和着色器，因为它们没有显示任何有用的内容。
- SimplePlayerController 不再使用 PlayerController.isGrounded，因为它在 FixedUpdate 之外不可靠。

## [3.1.0] - 2024-04-01

### 修复
- 错误修复：当向时间线轨道添加 Cm Shot 而其 CinemachineBrain 处于非活动状态时，出现索引超出范围异常。
- 错误修复：当 LookAt 和 Follow 位于不同垂直位置时，使用 3 环设置的 OrbitalFollow 会不恰当地对垂直输入进行阻尼。
- 错误修复：StateDrivenCamera 的最小激活时间已损坏。
- 错误修复：当 Brain 处于 FixedUpdate 时，Solo 未更新。
- 错误修复：当样条旋转时，样条滚动计算不正确。
- 错误修复：当在两个相机之间多次切换时，"Freeze When Blending Out" 不起作用。
- 错误修复：由于设置变换旋转时的浮点精度问题，Cinemachine 相机有时会不必要地污染场景。

### 新增
- 添加了 CinemachineDecollider，用于解决相机与碰撞器和地形的交叉问题，而不一定保持到目标的视线。
- CinemachineDeoccluder 提供了朝向 Follow 目标而不是 LookAt 目标进行解析的选项。
- 添加了 CinemachineShotQualityEvaluator，它是 Deoccluder 中评估代码的独立版本。
- 添加了 StateDrivenCamera.CancelWait() 方法，以取消对挂起状态更改的当前等待。
- 添加了 FlyAround 示例场景，展示了一个简单的环绕飞行相机。
- 向 InputAxisControllerBase 添加了 IgnoreTimeScale 选项。
- 当启用 Save During Play 并且已对可保存属性进行更改时，退出游戏模式时添加了确认对话框。

### 更改
- SplineDolly 和 SplineCart：当单位更改时，样条上的位置被保留。
- SimplePlayerAimController 示例已升级，可在任意表面上工作（不再依赖于世界向上方向）。
- SimplePlayerController 示例具有逻辑，可在玩家相对于相机颠倒时避免输入万向节锁。
- PlayerOnSphere 示例现在改为 PlayerOnSurface - 可以在任意表面上行走。
- 轴重新居中在具有不同重新居中时间的轴上独立发生，因此一个轴上的输入不会影响其他轴的重新居中状态。
- FreeLookOnSphericalSurface 示例得到改进，添加了移动表面和第二个相机。
- 将 SameAsFollowTarget 替换为 RotateWithFollowTarget（SameAsFollowTarget 仍然存在但已弃用）。
- Deoccluder 在 Finalize 阶段而不是 Aim 阶段评估镜头。

## [3.0.1] - 2023-11-27

### 修复
- 错误修复：在 InputAxis.CancelDeltaTime 中，如果 deltaTime 为零，则出现除零错误。
- 回归修复：CinemachineCamera 检查器的 Solo 功能未正确更新。
- 回归修复：旧版镜头设置失去了其动画能力。
- 错误修复：升级程序在某些情况下未能正确升级动画绑定。

### 新增
- 添加了 CinemachineVirtualCameraBase.CancelDamping() 便捷方法，可将相机快速对齐到其目标位置。
- 添加了 CinemachineOrbitalFollow.TargetOffset 以重新定位轨道中心。
- 添加了 CinemachineGroupFraming.CenterOffset 以在屏幕上重新定位组中心。
- 向 CinemachineHardLookAt 行为添加了 LookAtOffset。
- 新增支持新的相机叠加层。

### 更改
- RuntimeUtility.GetScratchCollider 和 RuntimeUtility.DestroyScratchCollider 现在是公共的，以允许自定义扩展使用它们。
- SaveDuringPlay 支持多场景编辑。

## [3.0.0] - 2023-10-25

### 修复
- 回归修复：扩展和组件无法修改混合提示。
- 错误修复：HardLockToTarget 组件忽略了 PreviousStateIsValid 标志。

### 新增
- 现在可以在 ManagerCameras 中取消活动混合，与在 CM Brains 中相同。

### 更改
- CinemachineBrain.TopCameraFromPriorityQueue() 现在是受保护的虚方法。

## [3.0.0-pre.9] - 2023-10-01

### 修复
- 回归修复：Sequencer 检查器的子相机弹出窗口未被填充。
- 回归修复：Manager Camera 的子相机警告图标未正确更新。
- 回归修复：ManagerCameras 正在破坏其子项中的 PreviousStateIsValid。
- 错误修复：CinemachineInputAxisController 编辑器在 Driven Axis 项上缺少折叠箭头。

## [3.0.0-pre.8] - 2023-09-22

### 修复
- 错误修复：当相机旋转恰好为 180 度时，偶尔出现精度问题，导致旋转闪烁。
- 错误修复：在轴范围末端的减速度过于激进。
- 错误修复：过渡到相机时不应强制进行轨道重新居中。
- 错误修复：InheritPosition 获取实际相机位置，因此如果在混合中间过渡，它能一致地工作。
- 错误修复：当调用 OnTargetObjectWarped 时，CinemachineDeoccluder 导致了一次弹出。
- 错误修复：发出了虚假的相机剪切事件，尤其是在 HDRP 中。
- 错误修复：当检查器隐藏在另一个选项卡后面时，出现空引用异常。
- 错误修复：当 LookAt 目标是一个组时，GroupFraming 检查器显示了不正确的警告。
- 错误修复：GroupFraming 在游戏视图中显示更准确的组大小指示器。
- 错误修复：当目标组被删除但仍被 vcams 引用时，日志中出现空引用。
- 回归修复：如果未设置目标，CinemachineCollider 会生成 NaN 位置。

### 新增
- 新示例：ThirdPersonWithAimMode 展示了如何实现带有瞄准模式的 FreeLook 相机。
- 向 OrbitalFollow 添加了 Recentering Target。现在可以在 Lazy Follow 模式下进行重新居中。
- 在 Deoccluder 和 ThirdPersonFollow 中添加了 API，以访问哪些碰撞对象正在影响相机位置。
- 添加了 ICinemachineTargetGroup.IsValid 属性以检测已删除的组。
- 添加了在 CinemachineInputAxisProvider 中禁用 deltaTime 缩放的选项。

### 更改
- 改进了 OrbitalFollow 的 ForceCameraPosition 算法。
- Deoccluder 在所有模式下都考虑了相机半径。
- 将 CinemachineSplineDolly.CameraUp 重命名为 CameraRotation，这更准确地反映了它的作用。
- 将 InputAxis.DoRecentering() 重命名为 InputAxis.UpdateRecentering()。
- StateDrivenCamera：在选择当前活动相机时，现在会考虑子相机的启用状态和优先级。

### 弃用
- 移除了 CinemachineToolSettings 叠加层。

## [3.0.0-pre.7] - 2023-05-04

### 新增
- 添加了 CinemachineCore.GetCustomBlender 和 BlendCreatedEvent，以允许自定义混合行为。
- 新的 Custom Blends 示例场景，说明了如何自定义混合算法。
- CinemachineChannels 现在可以通过 CinemachineChannelNames 资源命名。OutputChannel 结构已被移除。
- 添加了 CinemachineCameraManagerEvents 行为。
- 添加了定义 CINEMACHINE_NO_CM2_SUPPORT 的选项，通过移除对旧版 CM2 的支持来减轻包的重量。
- 第三人称射击示例场景现在有一个切换肩膀的选项。

### 更改
- 改进了对嵌套混合的处理。
- CinemachineCameraEvents、CinemachineBrainEvents 和 CinemachineCameraManagerEvents 可以添加到任何游戏对象，而不仅仅是目标对象。

## [3.0.0-pre.5] - 2023-04-25

### 修复
- 错误修复：MixingCamera 为其所有子项正确调用 OnTransitionFromCamera。

### 新增
- 新的 BlendHint：IgnoreTarget 将在不考虑跟踪目标的情况下混合旋转。
- 新的 BlendHint：FreezeWhenBlendingOut 将从相机状态的快照混合出去。
- InputAxisController 具有在附加相机混合时抑制输入的选项。
- 添加了 CinemachineCameraEvents 和 CinemachineBrainEvents 行为用于事件处理。
- 添加了 BlendFinished 和 CameraDeactivated 事件。
- 为输入系统添加了分屏示例。
- 示例 UI 同时适用于旧版输入系统和输入系统包。
- Timeline：在 CinemachineTrack 中添加了 Track Priority 字段，以控制当轨道包含在嵌套时间线中时的轨道优先级。
- 新的 2D 平台示例展示了自定义相机管理器。

### 更改
- 最低 Unity 版本现为 2022.2.15，以获得最佳检查器体验。
- 所有命名空间从 "Cinemachine" 更改为 "Unity.Cinemachine"。
- "Cinemachine.Utility" 命名空间合并到 "Unity.Cinemachine"。
- CinemachineBlendListCamera 已重命名为 CinemachineSequencerCamera。
- 重命名了 .asmdef 文件以遵循约定：Unity.[PackageName]。
- TrackedObjectOffset 重命名为 TargetOffset。
- 改进了 PositionComposer 和 RotationComposer 检查器的布局。
- LensSettings 被重构以改进对 SensorSize 和其他物理属性的处理。
- 对内建管线提供完整的物理相机支持。
- LensPresets 和 PhysicalLensPresets 现在是独立的资源。
- CinemachineInputAxisController 被重构以更易于自定义。
- 示例与内建渲染管线、通用渲染管线和高清渲染管线兼容。
- CinemachineUpgradeManager 在升级完成后重新打开原始场景。
- 事件系统被重构。
- 重构了 CinemachineCameraManagerBase 以使其对自定义更有用。
- CinemachineCore 被重构以移除单例模式。

## [3.0.0-pre.4] - 2023-02-09

### 新增

- 进度条已添加到 Cinemachine Upgrader。
- CinemachineDeoccluder 是一个新类，不仅仅是 CinemachineCollider 的重命名。
- CinemachineAutoFocus 扩展现在可用于内建和 URP 管线，但功能比 HDRP 少。
- 当相机处于物理模式时，Camera.focusDistance 由 CM 驱动。
- Confiner2D 提供了 API，可在镜头或宽高比更改时调整 Confiner 以适应当前窗口大小。
- TargetGroup 现在忽略其游戏对象处于非活动状态的成员。
- Cinemachine Samples 可以导入其包依赖项。
- CinemachinePathBase 搜索半径针对非循环路径进行了修复。
- 添加了 SplineAutoDolly.ISplineAutoDolly.Reset() 方法和 SplineAutoDolly.Enabled 标志。
- Confiner2D 和 Confiner3D 支持在边界边缘平滑停止。
- URP：在相机剪切时添加时间效果重置。
- 向 CinemachinePostProcessing 和 CinemachineVolumeSettings 添加了 Weight 设置。
- GroupFraming 也适用于 LookAt 目标。
- 添加了几个新的示例场景。

### 更改

- 改进了 CINEMACHINE_EXPERIMENTAL_DAMPING 算法的性能。
- 路径 Gizmo 绘制得到了优化。
- Confiner2D 减少了 GC 分配。
- CinemachineSmoothPath 现在可以正确升级到 Splines。
- InputAxis 重构以支持重新居中和瞬时轴支持。
- CinemachineIndependentImpulseListener 重命名为 CinemachineExternalImpulseListener。
- CmCamera 现在是 CinemachineCamera。
- SimpleFollowWithWorldUp 绑定模式已重命名为 LazyFollow。
- CinemachineExtension API 更改为 VirtualCamera、GetAllExtraStates、OnTargetObjectWarped 和 ForceCameraPosition。
- 第三人称射击示例使用对象池来处理抛射物。

### 弃用

- CinemachineConfiner 已弃用。新行为 CinemachineConfiner3D 处理 3D 限制。使用 CinemachineConfiner2D 进行 2D 限制。
- 3rdPersonFollow 和 3rdPersonAim 已弃用，并分别由 ThirdPersonFollow 和 ThirdPersonAim 替换。

### 修复

- 回归修复：POV 和 PanTilt 正确处理 ReferenceUp。
- 错误修复：镜头混合错误。
- 错误修复：CinemachineDeoccluder 的 Pull Forward 策略即使相机半径大于 0 也仅向前拉。
- 错误修复：扩展在域重新加载时未遵守执行顺序。
- 错误修复：AxisState 在 timescale == 0 时未遵守。
- 错误修复：当任何优先级值之间的差异小于整数最小值或大于整数最大值时，优先级排序错误。
- 错误修复：在视角为 +-90 度时，SimpleFollow 中偶尔出现轴漂移。
- 错误修复：物理镜头设置未正确应用。

## [3.0.0-pre.3] - 2022-10-28
- 错误修复：旋转作曲器的前视功能有时会弹出。

## [3.0.0-pre.2] - 2022-10-20
- 添加"显示层次结构图标"偏好选项。
- Cinemachine 的新图标（相机、组件、扩展、轨道）。
- Freelook ForcePosition 现在更精确。
- Confiner2D 现在支持 BoxCollider2D。
- 添加了对"将对象放置在世界原点"偏好选项的支持。
- 添加了通道设置，用于多个 CM Brain，而不是依附于图层系统。
- CinemachineCollider 重命名为 CinemachineDeoccluder。
- 构图参考线的新检查器和 API。
- 游戏视图参考线现在在热态时指示并可以被拖动。
- CinemachineTrack 在创建时分配默认的 CinemachineBrain。
- 添加对 HDRP 14 (Unity 2022.2) 的支持。
- 错误修复：StateDrivenCamera/Clearshot：当从进行中的过渡退出时出现过渡故障。
- 错误修复：在某些自由视角之间过渡时偶尔出现 1 帧故障。
- 错误修复：使用 LockToTarget 绑定的 Transposer 有时会出现万向节锁。
- 错误修复：轴输入的 InputValueGain 模式不是帧率无关的。
- 错误修复：如果启用了重新居中，POV 在其居中位置启动。

## [3.0.0-pre.1] - 2022-06-01
- 用于将 Cinemachine 2 升级到 Cinemachine 3 的升级机制。
- 虚拟相机重构：CinemachineVirtualCamera 现在是 CmCamera。
- FreeLook 重构：CinemachineFreeLook 现在是带有 FreeLook Modifier 的 CmCamera。
- 将 Follow 和 LookAt 目标合并为单个跟踪目标，并带有可选的 LookAt 目标。
- 添加用于自定义优先级设置的标志。
- 仅限 HDRP：向镜头添加了 FocusDistance 设置。
- 仅限 HDRP：新的 AutoFocus 扩展。用于焦点跟踪，代替 VolumeSettings。包括自动模式，该模式查询深度缓冲区而不是跟踪特定目标。
- 向 CM Brain 添加了 Lens Mode Override 属性。启用后，它允许 CM 相机覆盖镜头模式（透视 vs 正交 vs 物理）。
- 添加了 Unity Spline 支持。旧的 Cinemachine Paths 已弃用，改用 Unity Splines。
- 向相机和 Spline Cart 添加了可自定义的自动轨道跟踪。
- 添加了 IShotQualityEvaluator 以支持为 ClearShot 自定义镜头质量评估。
- 错误修复：没有冗余的 RepaintAllViews 调用。
- 错误修复：在极端的 FreeLook 配置下，碰撞器阻尼更加稳健。

## [2.9.0] - 2022-08-15
- 错误修复：当启用 Confine Screen Edges 并且相机旋转时，CinemachineConfiner 未能正确限制。
- 错误修复：当相机窗口大于输入 Confiner 的轴对齐边界框时，Confiner2D 限制到中点。
- 错误修复：当没有分配跟随目标时，3rdPersonFollow 显示警告消息，就像其他身体组件一样。
- 错误修复：FadeOut 示例场景着色器错误地剔除了一些对象。
- 错误修复：FreeLook 在第一帧有错误的朝向，可能导致轻微的抖动。
- 错误修复：当屏幕 X 和 Y 字段被修改时，FramingTransposer 和 Composer 的 Bias 字段存在轻微的舍入误差。
- 错误修复：在球面混合期间修复了虚假的 Z 旋转。
- 错误修复：当来回混合相同的相机时，混合速度未正确设置。
- 回归修复：POV 相对于其父变换。
- 错误修复：当不使用物理相机时，SensorSize 未保存。
- Clipper 库依赖不再与用户冲突。
- AimingRig 示例仅可选地依赖于 UnityEngine.UI。
- 错误修复：使用 LockToTarget 绑定的 Transposer 有时会出现万向节锁。
- 错误修复：StateDrivenCamera/Clearshot：当从进行中的过渡退出时出现过渡故障。

## [2.9.0-pre.7] - 2022-03-29
- 错误修复：如果相机上没有后处理层，则存在内存泄漏。
- 错误修复：独立分析器不再因 CM 而崩溃。
- 错误修复：在早于 2020 的 Unity 编辑器版本中，当安装输入系统包时，Cinemachine 不会产生编译器错误。
- 错误修复：EmbeddedAssetProperties 在编辑器中未正确显示。
- 向依赖它的脚本添加了时间线防护。
- 错误修复：SaveDuringPlay 现在适用于 IList。
- 错误修复：将 VirtualCamera 和 FreeLook 组件粘贴到预制件上适用于子组件。
- 错误修复：CinemachineInputProvider 现在正确跟踪输入动作的启用状态。
- 错误修复：使用世界向上方向覆盖时，POV 方向不正确。
- 向 CinemachineInputHandler 添加了 AutoEnable 选项。

## [2.9.0-pre.6] - 2022-01-12
- 错误修复：当相机为正交时，负的近裁剪平面值被保留。
- 回归修复：如果 CM 虚拟相机处于活动状态，则无法更改主相机的投影方式。
- 回归修复：轴输入忽略了 CM 的 IgnoreTimeScale 设置。
- 移除了旧版的 .unitypackages。
- 新功能：CinemachineBrain 可以控制其他游戏对象，而不是它附加到的那个。
- 错误修复：当旧版输入系统被禁用时，Cinemachine 分配一个默认的输入控制器委托，该委托返回 0。
- 当使用输入系统时，Cinemachine 示例场景显示信息性文本，而不是抛出错误消息。
- 回归修复：当物理模块不存在时，编译错误。
- 使用游戏对象菜单项创建的游戏对象现在遵循 Unity 命名约定。
- 回归修复：虚拟相机在域重新加载时不再忘记它们的目标是组。
- 将 Cinemachine 工具移到主工具叠加层中 (2022.1+)，将 FreeLook 装备选择移到单独的叠加层，更新了图标以支持浅色和深色主题。
- 错误修复：当直视上方或下方时，3rdPersonFollow 记录控制台消息。
- 错误修复：InputProvider 不再每帧导致微小的 GC 分配。
- 回归修复：CinemachineCollider 的平滑时间未正确重置，因此它仅工作一次。
- Cinemachine 支持 Splines 包。添加了新的 Body 组件：CinemachineSplineDolly。
- 错误修复：叠加工具提示名称不正确。
- 错误修复：当选择其 vcam 时，Confiner2D 现在显示计算出的限制区域。
- 示例不再对 HDRP 和 URP 抛出错误。3rdPersonWithAimMode 和 Timeline 示例不再具有无效引用。

## [2.9.0-pre.1] - 2021-10-26
- 添加了直接在 CinemachineBrain 中设置活动混合的能力。
- 错误修复：OnTargetObjectWarped() 对于 3rdPersonFollow 无法正常工作。
- 错误修复：POV 未能正确处理被覆盖的向上方向。
- 回归修复：在 UpdateTargetCache 中移除了 GC 分配。
- 错误修复：异步场景加载/卸载可能导致抖动。
- 错误修复：输入系统每渲染帧应仅读取一次。
- 错误修复：当源或目标相机沿世界向上轴观察时，混合有时不正确。
- 错误修复：提高了组构图框架的准确性。
- 新功能：为 Cinemachine 组件添加了场景视图叠加工具。
- 回归修复：前视功能再次工作。
- Cinemachine3rdPersonAim 暴露了 AimTarget，这是玩家将击中的位置。

## [2.8.0] - 2021-07-13
- 错误修复：FreeLook 预制件在通过其实例编辑预制件后不会损坏。
- 错误修复：3rdPersonFollow 现在可以与 Aim 组件配合工作。
- 错误修复：在 vcams 之间混合，这些 vcams 被旋转以至于它们的向上向量与世界向上方向不同，现在正确了。
- 错误修复：当轴范围受限时，POV 重新居中并非总是正确居中。
- 错误修复：当相机半径很大时，碰撞器有时会轻微反弹。
- 错误修复：CinemachineVolumeSettings 检查器导致游戏视图闪烁。
- 错误修复：当启用焦点跟踪时，CinemachineVolumeSettings 检查器在 URP 下显示误导性的警告消息。
- 错误修复：在混合完成之前快速切换活动相机未使用正确的混合时间。
- AimingRig 示例场景更新了更好的反应式十字准星设计。
- 添加了用于在 Clearshot 和 StateDrivenCamera 中访问活动混合的 API。
- 错误修复：当 Brain 的 BlendUpdateMode 为 FixedUpdate 时，虚拟相机在编辑模式下未更新。
- 错误修复：镜头模式覆盖在所有情况下都无法正常工作。
- Collider2D 检查器：当碰撞器类型错误时添加了警告。

## [2.8.0-pre.1] - 2021-04-21
- 默认情况下，切换目标（Follow, LookAt）是平滑的。对于旧行为，在更改目标后将 PreviousStateIsValid 设置为 false。
- 错误修复：反转进行中的混合尊重非对称混合时间。
- 回归修复：CmPostProcessing 和 CmVolumeSettings 组件设置景深现在与 Framing Transposer 正确配合工作。
- 回归修复：当 Z 阻尼很高时，3rdPersonFollow 将玩家保持在视图中。
- 回归修复：当未选择"覆盖模式：物理"时，物理相机属性被 vcams 覆盖。
- 新示例场景：Boss cam 演示了如何设置一个跟随玩家并注视玩家和 Boss 的相机。Boss cam 还展示了自定义扩展的示例。
- 向 Impulse Source 添加了简化模式。
- 向 Impulse Listener 添加了次级反应设置。
- 为 ScreenSpaceOverlay 和 ScreenSpaceCamera 相机渲染模式添加了 Storyboard 支持。
- 向 Cinemachine3rdPersonFollow 添加了 DampingIntoCollision 和 DampingFromCollision 属性，以控制相机移动以校正遮挡的逐渐程度。
- 添加了 CinemachineCore.OnTargetObjectWarped() 来扭曲所有以某个对象为目标的 vcams。
- 添加了 vcam 具有负近裁剪平面的能力。
- 在 Cinemachine 偏好设置中添加了"可拖动的游戏窗口参考线"切换选项。禁用时，游戏窗口参考线仅用于可视化。
- 向虚拟相机检查器添加了按钮，如果缺少 CinemachineInputProvider 组件，则自动生成它。
- 默认的后处理配置文件优先级现在可配置，默认为 1000。
- Cinemachine3rdPersonFollow 现在无需物理模块和碰撞解析即可运行。
- 错误修复：当相机半径很大时，3rdPersonFollow 碰撞解析失败。
- 错误修复：3rdPersonFollow 阻尼发生在世界空间而不是相机空间。
- 错误修复：当 Z 阻尼很高时，3rdPersonFollow 出现卡顿。
- 回归修复：CinemachineInputProvider 停止提供输入。
- 错误修复：当镜头 OverrideMode != None 时，镜头宽高比和 sensorSize 被更新。
- 错误修复：在活动 vcam 上更改目标行为异常。
- 错误修复：Framing transposer 未能处理空组。
- 错误修复：中断启用 InheritPosition 的过渡无效。
- 错误修复：Cinemachine3rdPersonFollow 默认处理碰撞，现在默认禁用。
- 错误修复：SaveDuringPlay 保存了一些没有 SaveDuringPlay 属性的组件。
- 回归修复：CM Brain 检查器中自定义混合编辑器中的条目不可选择。
- 仅当相应的检查器子部分展开时，才绘制游戏视图参考线。
- FreeLook 装备现在在检查器中以选项卡形式组织。
- 新示例场景：**Boss cam** 示例场景演示了一个相机设置，用于跟随玩家并注视玩家和 Boss。该场景提供了自定义扩展的示例。
- 新示例场景：**2D zoom**，展示了如何使用鼠标滚轮缩放正交相机。
- 新示例场景：**2D fighters**，展示了如何根据某些条件（这里是玩家的 y 坐标）逐渐向目标组添加/移除目标。
- 错误修复：CinemachineCollider 的位移阻尼是在世界空间而不是相机空间中计算的。
- 错误修复：当默认向上方向且无 Aim 行为时，TrackedDolly 有时会引入虚假的旋转。
- 错误修复：3rdPersonFollow 的肩膀现在随着世界向上向量的变化而平滑变化。

## [2.7.2] - 2021-02-15
- CinemachineConfiner2D 现在处理相机窗口过大的情况。
- 新示例场景 (FadeOutNearbyObjects) 演示了使用着色器在相机和目标之间的对象的淡出效果。该示例包括一个 Cinemachine 扩展，可方便地控制着色器参数。
- 错误修复 (1293429) - Brain 在某些情况下可能选择优先级不是最高的 vcam。
- 错误修复：SaveDuringPlay 也适用于预制件实例。
- 错误修复 (1272146) - 向预制件资源添加 vcam 不再在控制台中引起错误。
- 错误修复 (1290171) - 脉冲管理器在游戏模式开始时未被清除。
- 移除了嵌套的 Scrub Bubble 示例（文件名太长），现在作为嵌入式包提供。
- 为物理、动画和 imgui 添加了编译保护。Cinemachine 现在不硬依赖任何东西。
- 错误修复：CM StoryBoard 有 1 像素的边框。
- 错误修复：CM StoryBoard 在热重载后丢失视口引用。
- 错误修复：FramingTransposer 的 TargetMovementOnly 阻尼导致闪烁。
- 错误修复：如果没有用户输入且使用 SimpleFollowWithWorldUp，FreeLook 会出现小的漂移。
- 错误修复：InheritPosition 无法与 SimpleFollow 绑定模式配合工作。
- 错误修复：当没有活动 vcams 时，清理残留的后处理配置文件。
- 错误修复：在将传递给 CinemachineInputHandler 的输入动作投入使用前检查其是否启用。
- 错误修复：当 ReferenceLookAt 设置为遥远目标时，3rdPersonFollow FOV 混合不正确。
- 错误修复：位置预测器未正确重置。
- 错误修复：通过菜单创建不会作为选定对象的子项创建。
- 错误修复：当没有活动 vcams 时，后处理配置文件未清理。
- 错误修复：在 2018.4 / macOS 上，"Install CinemachineExamples Asset Package" 菜单项失败。
- 新示例场景 (2DConfinerComplex) 演示了新的 CinemachineConfiner2D 扩展。
- 更新了 2D 示例场景 (2DConfinedTargetGroup, 2DConfiner, 2DConfinerUndersized, 2DTargetGroup) 中的 CharacterMovement2D 脚本，使跳跃响应更灵敏。
- 更新了 2DConfinedTargetGroup 和 2DConfiner 场景以使用新的 CinemachineConfiner2D 扩展。

## [2.7.1] - 2020-11-14
- 新功能：CinemachineConfiner2D - 改进的 2D 限制器。
- 向 ImpulseListener 添加了 ApplyAfter 选项，以增加对扩展顺序的控制。
- UI 更新 - 将 Cinemachine 菜单移至 GameObject 创建菜单和层次结构的右键上下文菜单。
- 虚拟相机镜头检查器支持显示水平视野。
- 虚拟相机镜头可以覆盖正交和物理相机设置。
- 错误修复 (1060230) - 镜头检查器有时短暂地错误显示正交与透视。
- 错误修复 (1283984) - 加载带有 DontDestroyOnLoad 的新场景时的错误消息。
- 错误修复 (1284701) - 删除 vcam 时的边界情况异常。
- Storyboard Global Mute 从 Cinemachine 菜单移至 Cinemachine 偏好设置。
- 错误修复 - 长时间空闲的 vcams 在重新唤醒时有时会出现具有巨大 deltaTime 的单个帧。
- 错误修复 - 退出游戏模式后，后处理暂时停止应用。

## [2.6.3] - 2020-09-16
- 回归修复 (1274989) - OnTargetObjectWarped 对 OrbitalTransposer 无效。
- 错误修复 (1276391) - CM Brain 重置未重置检查器中的自定义混合资源。
- 错误修复 (1276343) - CM Brain 检查器自定义混合下拉箭头未对齐。
- 错误修复 (1256530) - 在适当位置不允许使用多个组件。
- 错误修复：BlendList 相机错误地保持了 0 长度的相机剪切。
- 错误修复 (1174993) - 在首次导入 CM 后添加 vcam 后，CM Brain 徽标未添加到主相机旁边的层次结构中。
- 错误修复 (1100131) - Confiner 知道 2D 碰撞器的偏移属性。

## [2.6.2] - 2020-09-02
### 错误修复
- 回归修复：当将 Cinemachine 与 PostProcessing 包一起使用时，OnCameraCut 内存泄漏。
- 错误修复 (1272146)：在绘制 Gizmos 之前检查空管线。
- 添加了对禁用物理模块的支持。

## [2.6.1] - 2020-08-13
### 错误修复
- 回归修复：PostProcessing/VolumeSettings FocusTracksTarget 未考虑 LookAt 目标偏移。
- 回归修复：Confiner 不再限制噪声和脉冲。
- 错误修复：如果混合状态中只有 1 个剪辑，即使为该剪辑分配了 vcam，StateDrivenCamera 也会选择父状态。
- 错误修复：垂直组构图未能正确构图。
- 错误修复：CinemachineNewVirtualCamera.AddComponent() 现在正常工作。
- 错误修复：当 Physics2D 模块被禁用时，移除了编译错误。
- 错误修复：在加载或卸载场景时，Brain 会更新。
- 错误修复 (1252431)：修复了在使用时间线时每帧不必要的 GC 内存分配。
- 错误修复 (1260385)：正确检查预制件实例。
- 错误修复 (1266191) 点击偏好设置面板中的折叠标签会切换其展开状态。
- 错误修复 (1266196) 偏好设置面板中的 Composer 目标大小标签太大。
- 错误修复：擦洗缓存正在锁定超出缓存范围的虚拟相机变换。
- 改进了路径 Gizmo 绘制的性能。
- 时间线擦洗缓存支持嵌套时间线，但存在一些已知限制，将在未来的时间线包版本中解决。
- 在受控渲染的上下文中支持确定性噪声（通过 CinemachineCore.CurrentTimeOverride）。
- 向 Framing Transposer 添加了目标偏移字段。
- 向虚拟相机和扩展添加了多对象编辑功能。
- 在检查器中添加了按钮以清除擦洗缓存。

## [2.6.0] - 2020-06-04
### 新功能和错误修复
- 添加了 AxisState.IInputProvider API 以更好地支持自定义输入系统。
- 添加了 CinemachineInpiutProvider 行为以支持 Unity 的新输入系统。
- 添加了时间线擦洗缓存：启用后，在时间线中擦洗时模拟阻尼和噪声。
- 向 Brain 添加了 ManualUpdate 模式，以允许自定义游戏循环逻辑。
- VolumeSettings/PostProcessing：添加了为焦点跟踪选择自定义目标的能力。
- 添加了 CinemachineRecomposer，用于对程序化或录制的 vcam Aim 输出进行时间线调整。
- 添加了 GroupWeightManipulator 用于动画化组成员权重。
- 脉冲：添加了传播速度，允许脉冲以波的形式向外传播。
- 脉冲：添加了对连续脉冲的支持。
- 添加了 CinemachineIndependentImpulseListener，使任何游戏对象都具有 ImpulseListener 能力。
- 添加了 3rdPersonFollow 和 3rdPersonAim，用于精确的第三人称瞄准相机。
- 添加了虚拟相机的 ForceCameraPosition API，用于手动初始化相机的位置和旋转。
- 添加了示例场景：Aiming Rig 和 Dual Target 以展示不同的第三人称相机风格。
- FramingTransposer 在 Aim 之后完成其工作，因此它与 Aim 组件更好地配合。
- 框架合成器：添加了阻尼旋转选项。如果取消选中，对 vcam 旋转的更改将绕过阻尼，只有目标运动会被阻尼。
- 重构了前视功能 - 更好的稳定性。新行为可能需要调整现有内容中的一些参数。
- Composer 和 Framing Transposer：改进了在硬边缘区域的处理（无抖动）。
- Orbital Transposer / FreeLook：改进了目标移动时的阻尼。
- 自定义混合编辑器 UX 改进：允许直接编辑 vcam 名称，以及下拉菜单。
- 在 LookAt 和 Follow 目标字段上添加了"转换为目标组"选项。
- Confiner：当选择 ConfineScreenEdges 并且限制形状太小时，提高了稳定性。
- 扩展现在具有 PrePipelineMutateState 回调。
- CinemachineCore.UniformDeltaTimeOverride 在编辑模式下工作。
- 向 vcams 添加了 TargetAttachment 属性。通常为 1，这可用于放松对目标的关注 - 实际上是阻尼覆盖。
- 错误修复：混合更新方法处理不正确，在某些情况下导致抖动。
- 错误修复：如果体积改变了不可线性插值的值，则当权重为 epsilon 时，VolumeSettings 混合会弹出。
- 错误修复 (1234813) - 检查已删除的 FreeLook。
- 错误修复 (1219867) - 如果混合，vcam 在禁用时弹出。
- 错误修复 (1214301, 1213836) - 在编辑 vcam 预制件时不允许结构更改。
- 错误修复 (1213471, 1213434)：在编辑器中添加空值检查。
- 错误修复 (1213488)：预制件 vcam 没有 Solo。
- 错误修复 (1213819)：编辑器更改时重新绘制游戏视图。
- 错误修复 (1217306)：当为空或当成员是组的后代时，目标组位置漂移。
- 错误修复 (1218695)：完全限定 UnityEditor.Menu 以避免在某些情况下出现编译错误。
- 错误修复 (1222740)：对轴值范围没有控制的绑定模式不受其影响。
- 错误修复 (1227606)：对于具有手动动画旋转的 Composer，时间线预览和游戏模式不一致。
- 错误修复：当边界形状/体积更改时，Confiner 的缓存被重置。
- 错误修复 (1232146)：Vcam 在限制器边界框边缘不再抖动。
- 错误修复 (1234966)：CompositeCollider 缩放被应用了两次。

## [2.5.0] - 2020-01-15
### 同时支持 HDRP 7 和 URP
- 适应 HDRP 和 URP 的同时存在。
- 回归修复：即使重新居中关闭，轴在编辑模式下也总是重新居中。

## [2.4.0] - 2020-01-10
### HDRP 7 支持和错误修复
- Storyboard：添加了全局静音功能。
- 新的 vcams 默认创建与场景视图相机匹配。
- 向 POV 组件添加了 ApplyBeforeBody 选项，以支持与 FramingTransposer 配合工作。
- 向 POV 组件添加了 RectenterTarget。
- 向扩展添加了 OnTransitionFromCamera 回调。
- 向 SameAsFollowTarget 和 HardLockToTarget 组件添加了阻尼。
- URP 7.1.3：添加了 CinemachinePixelPerfect 扩展。
- 向 AxisState 添加了速度模式，以支持在没有最大速度的情况下直接控制轴。
- 新示例场景：OverTheShoulderAim 说明了如何实现过肩第三人称相机，具有正常和瞄准模式。
- Impulse Manager：添加了忽略时间缩放的选项。
- Framing Transposer：如果 InheritPosition，则添加了对相机旋转的 OnTransition 处理。
- 升级以支持 HDRP 和 Universal RP 7.0.0 API。
- 升级以支持 HDRP 和 Universal RP 7.1.0 API。
- 移除了 Resources 目录。
- 示例场景现在可通过包管理器获得。
- 向时间线中的 Cinemachine Shot 添加了可选的"显示名称"字段。
- 向 vcam 检查器上下文菜单添加了"采用当前相机设置"项。
- Composer 和 FramingTransposer：允许死区扩展到 2，屏幕 X,Y 范围可以从 -0.5 到 1.5。
- HDRP：如果使用物理相机，镜头预设包括物理设置。
- 回归修复：Framing Transposer：忽略 LookAt 目标。专门使用 Follow。
- 错误修复：Framing Transposer 未能正确处理 FOV 的动态更改。
- 错误修复：当在管理器 Vcams 上时，PostProcessing 扩展未能正确处理待机更新。
- 错误修复：当场景卸载时，PostProcessing 扩展泄漏少量内存。
- 错误修复：(fogbugz 1193311, 1193307, 1192423, 1192414)：不允许 vcams 的预设。
- 错误修复：在某些朝向模式中，FreeLook 在激活时不恰当地修改了轴。
- 错误修复：在 TargetForward 朝向模式中，Orbital transposer 不恰当地过滤了朝向。
- 错误修复：添加了 EmbeddedAssetHelper 空值检查。
- 错误修复：对于物理相机，作曲器屏幕参考线绘制在正确的位置。
- 错误修复：FreeLook 不尊重 X 轴重新居中的等待时间。
- 错误修复：FreeLook X 轴并非总是在装备之间完美同步。
- 错误修复 (fogbugz 1176866)：碰撞器：在退出时清理静态刚体。
- 错误修复 (fogbugz 1174180)：框架合成器错误的正交大小计算。
- 错误修复 (fogbugz 1158509)：将 brain.UpdateMethod 拆分为 VcamUpdateMethod 和 BrainUpdateMethod，以使混合正确工作。
- 错误修复 (fogbugz 1162074)：框架合成器和组合成器仅达到最大正交大小的一半。
- 错误修复 (fogbugz 1165599)：Transposer：修复在 LockToTargetWithWorldUp 中的万向节锁问题。
- 错误修复：VolumeSettings：处理 HDAdditionalCameraData 中的图层蒙版。
- 错误修复：在绘制 Gizmos 时使用 vcam 的向上方向（轨道合成器和自由视角）。

## [2.3.4] - 2019-05-22
### PostProcessing V3 和错误修复
- 添加了对 PostProcessing V3 的支持 - 现在称为 CinemachineVolumeSttings。
- 添加了 CinemachineCore.GetBlendOverride 委托，以允许应用程序在发生任何 vcam 混合时覆盖它。
- 当一个混合被相反的混合取消时，减少混合时间。
- 正交相机允许近裁剪平面为 0。
- 当有东西拖到时间线上时，时间线不会自动创建 CM Brain。
- Confiner：当路径点数更改时，自动路径无效性的改进。
- 添加了 CinemachineInpuitAxisDriver 实用程序，用于覆盖默认的 AxisState 行为。
- CinemachineCameraOffset：添加了可自定义的阶段以确定何时应用偏移。
- 向 BlendList Camera 添加了循环选项。
- 改进了前视功能：不会自动重新居中。
- Brain 不再将时间缩放应用于固定增量时间。
- 添加了对 Unity.ugui 的依赖（2019.2 及以上版本）。
- 错误修复：在碰撞器中使用 Ignore 标签时可能出现无限循环。
- 错误修复：允许外部驱动的 FreeLook XAxis 与 SimpleFollow 正常工作。
- 错误修复：带有噪声的 vcams 有时在激活且待机更新不是 Always 时显示一帧无噪声。
- 错误修复：如果剪切到进行中的混合，则生成剪切事件 (fogbugz 1150847)。
- 错误修复：如果不是物理相机，则重置镜头偏移。
- 错误修复：碰撞器必须考虑实际的目标位置，而不是前视位置。
- 错误修复：FreeLook 朝向 RecenterNow 无效。
- 错误修复：前视功能现在考虑了被覆盖的向上方向。
- 错误修复：画中画的屏幕合成器参考线绘制在错误的位置。
- 错误修复：FreeLook 现在一次只绘制 1 个活动合成器参考线 (fogbugz 1138263)。
- 错误修复：相机有时在中断混合时突然移动。
- 错误修复：路径手柄不再随路径对象缩放。
- 错误修复：Framing Transposer 激活时居中未能正常工作 (fogbugz 1129824)。
- 错误修复：FreeLook 继承位置。
- 错误修复：如果有多个重叠的障碍物，碰撞器将相机推得太远。
- 错误修复：在几个地方使用 IsAssignableFrom 而不是 IsSubclass。
- 错误修复：当中断进行中的混合时，未遵守 Cut。
- 错误修复：碰撞器最小遮挡时间和平滑时间交互。
- 错误修复：TargetGroup.RemoveMember 错误 (fogbugz 1119028)。
- 错误修复：当成员权重接近 0 时，TargetGroup 成员插值抖动。
- 错误修复：如果绑定模式不是 LockToTarget，则 Transposer 角阻尼应为 0。

## [2.3.3] - 2019-01-08
### 临时补丁以绕过 Unity 在条件依赖项中的一个错误
- 移除了 Cinemachine.Timeline 命名空间，作为对 fogbugz 1115321 的变通方案。

## [2.3.1] - 2019-01-07
### 错误修复
- 添加了时间线依赖项。
- OnTargetObjectWarped 不再生成垃圾。

## [2.3.0] - 2018-12-20
### 支持 Unity 2019.1
- 添加了对新 unity.timeline 的依赖。
- 添加了对 PostProcessingV2 的条件依赖。
- 不再将 CM gizmo 复制到资源文件夹。
- FreeLook：如果从类似的 FreeLook 继承位置，则绕过阻尼。
- Timeline：改进了当 vcam 值在镜头检查器内部调整时的处理 (fogbugz 1109024)。

## [2.2.8] - 2018-12-10
### 错误修复、优化和一些实验性内容
- Transposer：添加了角阻尼模式，以支持在万向节锁情况下的四元数计算。
- Framing Transposer 和 Group Transposer：组构图错误修复，遵守最小/最大限制。
- 添加了 CinemachineCameraOffset 扩展，用于在管线末端将相机偏移固定距离。
- Dolly Cart：添加了对 LateUpdate 的支持。
- State-driven-camera：向 Animated Target 和 Layer Index 添加了 [NoSaveDuringPlay]。
- 添加了 AxisState.Recentering.RecenterNow() API 调用，以跳过等待时间并立即开始重新居中（如果启用）。
- 添加了 NoLens 混合提示，以保持相机镜头设置不变。
- 更新了文档（更正，并重新定位以防止导入）。
- 升级：添加了对 Unity 2018.3 中嵌套预制件的支持 (fogbugz 1077395)。
- 优化：位置预测器更高效。
- 优化：Composer 缓存了一些计算。
- 优化：修复了当镜头预设资源丢失时编辑器速度变慢的问题。
- 实验性：可选的新阻尼算法：尝试减少对可变帧率的敏感性。
- 实验性：可选的新高效版本的 vcam 和 FreeLook（不向后兼容）。
- Timeline：播放/暂停不会踢出时间线 vcam。
- 路径编辑器：确保当在场景视图中拖动路径路径点时，游戏视图得到更新。
- 即使相机附加到渲染纹理，也会显示作曲器参考线。
- 错误修复：允许脉冲定义是一个非公共字段（属性绘制器在抱怨）。
- 错误修复：当没有活动虚拟相机时添加了空值检查。
- 错误修复：CollisionImpulseSource 在检测 2D 碰撞器时出现拼写错误。
- 错误修复：向预制件 vcams 和 FreeLook 粘贴组件值会损坏场景和预制件。
- 错误修复：时间线混合器在混合结束时对单帧出现故障。
- 错误修复：向 POV 和 OrbitalTransposer 添加了 OnTransitionFromCamera()，以智能地过渡轴。
- 回归修复：如果没有活动 vcam，不要设置相机的变换。

## [2.2.7] - 2018-07-24
### 主要是错误修复
- 错误修复：fogbugz 案例 1053595：Cinemachine Collider 在原点留下隐藏的碰撞器，干扰场景对象。
- 错误修复：fogbugz 案例 1063754：空目标组产生控制台消息。
- 错误修复：FreeLook 粘贴组件值现在也粘贴 CM 子组件。
- 错误修复：添加了额外的空值检查以支持当前 vcam 被动态删除的情况。
- 错误修复：启用时重置 BlendList。
- 回归修复：FreeLook 轴值在类似的 vcams 之间过渡时被传递。
- 错误修复：剪切到 BlendList vcam 有时会产生几帧坏帧。
- 错误修复：智能更新更有效和正确地跟踪目标，并支持刚体插值（2018.2 及以上版本）。
- 增强：如果存在父变换，POV 组件将 POV 解释为相对于父变换。
- API 更改：OnCameraLive 和 CameraActivated 事件也将传出 vcam 作为参数（可能为空）。

## [2.2.0] - 2018-06-18
### 脉冲模块及更多
- 新的 Cinemachine Impulse 模块，用于事件驱动的相机抖动。
- 新的事件助手脚本 CinemachineTriggerAction 对 Collider 和 Collider2D 进入/退出事件采取行动，并将它们暴露为 UnityEvents。
- 新的性能调优功能：待机更新。控制当 vcam 处于待机状态时更新它的频率。
- 新的 NoiseSettings 编辑器，带有信号预览。
- 为相机镜头添加了焦距或命名 FOV 预设。
- 添加了对物理相机的支持：焦距和镜头偏移。
- 新的改进的组框架算法：在 GroupComposer 和 FramingTransposer 中更紧密的组框架。
- 碰撞器：如果目标在屏幕外，现在返回 TargetIsObscured（对于具有固定目标的相机非常有用）。
- 碰撞器：添加了最小遮挡时间设置，以忽略短暂的障碍物。
- 碰撞器：添加了透明图层蒙版，以指定不遮挡视线的固体对象。
- 碰撞器：阻尼不再使相机穿过障碍物。
- 碰撞器：添加了单独的音尼设置，用于目标被遮挡时与相机返回到其正常位置时。
- 碰撞器：添加了平滑设置，以减少在具有大量障碍物的环境中的相机跳跃。
- NoiseSettings：添加了复选框，用于纯正弦波而不是柏林噪声波。
- 如果没有 LookAt 目标，PostProcessing FocusTracksTarget 偏移相对于相机。
- TrackedDolly：默认向上模式将向上设置为世界向上。
- 虚拟相机：检查器中新的过渡部分，可以更好地控制混合：
  - 混合提示提供了一些关于如何插值位置和旋转的控制。
  - 继承位置复选框，以确保从传出相机平滑地位置交接。
  - 当相机激活时，OnCameraLive 事件被触发。用于自定义处理程序。
- 添加了 ScreenSpaceAimWhenTargetsDiffer 作为 vcam 混合提示。这影响了在具有不同 LookAt 目标的 vcams 之间混合时发生的情况。
- 提高了具有非常小 FOV 的 vcams 的稳定性。
- Framing Transposer 不再要求 LookAt 为 null。
- LensSettings 宽高比、正交、IsPhysicalCamera、SensorSize 属性不再是内部的。
- 噪声配置文件：不会神奇地创建资源。提示用户新配置文件或克隆配置文件的文件名和位置。
- 重构了时间线和 CM Brain 之间的交互，以改进对边界情况的处理 (fogbugz 案例 1048497)。
- 错误修复：如果目标是 OverrideController，StateDrivenCamera 编辑器找不到状态。
- 拖动轨道合成器变换时的错误修复：考虑偏置。
- 错误修复：SaveDuringPlay 未能正确处理资源字段 - 有时会破坏资源。
- 错误修复：SimpleFollow 合成器在游戏开始时未能正确初始化其位置。
- 错误修复：在某些 FixedUpdate 情况下，带有 CM 镜头的时间线导致抖动。
- 错误修复：具有异构更新方法的多个 Brain 未能正确行为。CM 现在将支持这一点，但您必须确保 Brain 具有不同的图层蒙版。
- 示例场景现在包括使用 CinemachineTriggerAction 脚本。

## [2.1.13] - 2018-05-09
### 移除了对不存在的时间线包的依赖，小错误修复
- 错误修复：自定义混合"任意到任意"无效（回归）。
- 错误修复：如果有多个具有不同宽高比的 Brain，Composer 有时会得到错误的宽高比。
- 错误修复：如果多个检查器且一个被隐藏，则无法拖动 vcam 变换。
- 错误修复：Framing Transposer 在错误的位置初始化 - 如果有死区则明显。

## [2.1.12] - 2018-02-26
### 故事板、错误修复和其他增强功能。也为包管理器进行了一些结构调整。
- 项目结构重构：从项目根目录移除了 Base、Timeline 和 PostFX 文件夹。后处理代码现在必须从 Cinemachine 菜单手动导入。不再依赖脚本定义。
- 新的 Storyboard 扩展，用于在 vcams 上显示图像。附带一个用于色彩分级的波形监视器窗口。
- 新的选项来指定 vcam 位置混合样式：线性、球形或圆柱形，基于 LookAt 目标。
- 添加了 API 以支持目标对象的无缝位置扭曲：OnTargetObjectWarped()。
- 添加了对自定义混合曲线的支持。
- 前视功能：添加了忽略 Y 轴移动选项。
- 添加了对级联混合的支持（即从混合中间混合看起来更好）。
- POV/Orbital/FreeLook 轴：在 UI 中暴露了最小值、最大值和环绕，用于自定义轴范围。
- FreeLook：添加了 Y 轴重新居中。
- POV：向两个轴添加了重新居中功能。
- 路径：添加了标准化路径单位选项：0 是路径起点，1 是终点。
- 路径：在检查器中添加了长度显示。
- 时间线剪辑编辑器：vcam 部分现在可折叠。
- API 增强：向管线阶段添加了 Finalize，即使对于管理器样式的 vcams 也会调用。
- 错误修复：PostProcessing V2 景深混合效果更好。
- 错误修复：OrbitalTransposer 与世界向上方向覆盖更好地配合工作。
- 错误修复：移除 StateDrivenCamera"未播放控制器"警告。
- 错误修复：处理不想被内省的程序集抛出的异常。
- 错误修复：跟随物理对象的 vcams 在退出游戏模式后错误地捕捉到原点。
- 错误修复：预测器现在支持时间暂停。
- 错误修复：将 Brain 中的 StartCoroutine 移至 OnEnable()。
- 错误修复：碰撞器在 Android 平台上的物理中导致问题。
- 错误修复：拖动 vcam 的位置会正确更新预制件。
- 错误修复：所有扩展现在都遵守"启用"复选框。
- 错误修复：扩展添加的撤消操作不再生成空引用。

## [2.1.10] - 2017-11-28
### 这是 *Unity Package Cinemachine* 的第一个 UPM 发布。
- 新的 Aim 组件：Same As Follow Target 简单地使用与 Follow 目标相同的方向。
- Perlin Noise 组件：添加了检查器 UI 以克隆或定位现有的噪声配置文件，并创建新的配置文件。
- 噪声预设已移出示例文件夹。
- 示例资源现在作为嵌入式包包含，默认不导入。
- 错误修复：带有 PositionDelta 的 FreeLook 未能正确更新朝向。
- 错误修复：在 FreeLook 之间过渡有时导致相机短暂冻结。
- 错误修复：向 FreeLook 添加了一些空值检查，以防止在构建时出现错误消息。

## [2.1.9] - 2017-11-17
### 初始版本。
*版本 2.1.9 从私有开发存储库克隆，对应于在资源商店发布的包*