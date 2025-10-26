# 程序化运动（Procedural Motion）

Cinemachine 相机本身是一个**被动（passive）** 的游戏对象，充当相机占位符，你可以：
* 将其放置在固定位置并进行静态瞄准。
* 将其作为子对象附加到另一个游戏对象上，使其随之移动和旋转。
* 通过自定义脚本对其进行操控，以实现移动、旋转和镜头控制。

然而，为了获得更复杂的效果，你可以向任何 Cinemachine 相机添加**程序化（procedural）** 行为和扩展，使其能够动态移动、抖动、跟踪目标、自动构图、响应用户输入、沿预设路径移动、对外部脉冲信号做出反应、产生后期处理效果等。


## 程序化行为与扩展（Procedural Behaviors and Extensions）

[Cinemachine 相机组件（Cinemachine Camera component）](CinemachineCamera.md) 允许你选择多种行为和扩展，以驱动 Cinemachine 相机的位置、旋转和镜头参数。

### 位置与旋转控制（Position and Rotation Control）

选择并配置**位置控制（Position Control）** 和**旋转控制（Rotation Control）** 行为，可使 Cinemachine 相机根据某些约束或条件**移动**并**瞄准** Unity 相机。

大多数可用行为旨在[跟踪或看向目标游戏对象](#目标游戏对象跟踪（target-gameobject-tracking）)。此外，部分行为支持通过用户输入来环绕或旋转相机。

借助这些行为，你可以：
* 以[固定偏移（fixed offset）](CinemachineFollow.md)、[轨道配置（orbital configuration）](CinemachineOrbitalFollow.md) 或[第三人称/第一人称视角（third or first person）](CinemachineThirdPersonFollow.md) 跟随目标。

* 通过自适应的相机[位置构图（position）](CinemachinePositionComposer.md) 和[旋转构图（rotation）](CinemachineRotationComposer.md)，或居中的[强制瞄准（hard look）](CinemachineHardLookAt.md) 来构建镜头，确保目标始终在相机画面中。

* 将目标的[位置（position）](CinemachineHardLockToTarget.md) 和[旋转（rotation）](CinemachineRotateWithFollowTarget.md) 应用到相机上，而非让目标出现在相机画面中。

* 使相机沿预设的样条线（Spline）移动，模拟[移动摄影轨道（dolly camera path）](CinemachineSplineDolly.md) 效果。

* 围绕可配置的[摇移和俯仰（pan and tilt）](CinemachinePanTilt.md) 轴旋转相机。

### 噪声（Noise）

选择并配置[**噪声（Noise）** 行为](CinemachineBasicMultiChannelPerlin.md)，可使 Cinemachine 相机产生**抖动（shake）** 效果，模拟真实物理相机的特性，营造电影感。

在每帧更新时，Cinemachine 会在相机跟随目标的移动之外单独添加噪声。噪声不会影响相机在后续帧中的位置。这种分离确保了“阻尼（damping）”等属性能按预期发挥作用。

### 扩展（Extensions）

添加**扩展（Extension）** 可增强 Cinemachine 相机的行为，以满足更具体或高级的需求。

例如，[去遮挡器（Deoccluder）](CinemachineDeoccluder.md) 扩展能将相机从遮挡其目标视野的游戏对象旁移开。

以下是所有可用的 Cinemachine 相机扩展列表：

  * [Cinemachine 自动对焦（Cinemachine Auto Focus）](CinemachineAutoFocus.md)
  * [Cinemachine 3D 限制器（Cinemachine Confiner 3D）](CinemachineConfiner3D.md)
  * [Cinemachine 2D 限制器（Cinemachine Confiner 2D）](CinemachineConfiner2D.md)
  * [Cinemachine 去碰撞器（Cinemachine Decollider）](CinemachineDecollider.md)
  * [Cinemachine 去遮挡器（Cinemachine Deoccluder）](CinemachineDeoccluder.md)
  * [Cinemachine 跟随缩放（Cinemachine Follow Zoom）](CinemachineFollowZoom.md)
  * [Cinemachine 自由视角修改器（Cinemachine FreeLook Modifier）](CinemachineFreeLookModifier.md)
  * [Cinemachine 群组构图（Cinemachine Group Framing）](CinemachineGroupFraming.md)
  * [Cinemachine 像素完美（Cinemachine Pixel Perfect）](CinemachinePixelPerfect.md)
  * [Cinemachine 后期处理（Cinemachine Post Processing）](CinemachinePostProcessing.md)
  * [Cinemachine 重构图器（Cinemachine Recomposer）](CinemachineRecomposer.md)
  * [Cinemachine 镜头质量评估器（Cinemachine Shot Quality Evaluator）](CinemachineShotQualityEvaluator.md)
  * [Cinemachine 故事板（Cinemachine Storyboard）](CinemachineStoryboard.md)
  * [Cinemachine 第三人称瞄准（Cinemachine Third Person Aim）](CinemachineThirdPersonAim.md)
  * [Cinemachine 体积设置（Cinemachine Volume Settings）](CinemachineVolumeSettings.md)
  <!---* Cinemachine 相机偏移（Cinemachine Camera Offset）（组件/扩展，文档中缺失）--->


## 目标游戏对象跟踪（Target GameObject Tracking）

目标游戏对象跟踪是定义程序化运动的关键要素。偏移量和屏幕构图都是相对于这些目标指定的，因此当目标在世界中移动时，相机会自动调整以保持镜头效果。

### 跟踪目标与看向目标属性（Tracking Target and Look At Target Properties）

默认情况下，Cinemachine 相机有一个**跟踪目标（Tracking Target）** 属性，该属性有两个用途：

* 当你定义需要位置控制的行为时，它指定了 Cinemachine 相机要跟随移动的变换（Transform）。
* 当你定义需要旋转控制的行为时，它指定了要瞄准的变换（即看向目标）。

> [!注意]
> 如果你需要为这两个用途使用不同的变换，可通过**跟踪目标（Tracking Target）** 字段右侧的按钮选择**使用单独的看向目标（Use Separate LookAt Target）** 选项。

### 目标跟踪与混合过渡（Target Tracking and Blends）

在 Cinemachine 执行镜头间的混合过渡时，目标也很重要。Cinemachine 会尝试维持目标在镜头中期望的屏幕位置；如果镜头间的目标发生变化，Cinemachine 会在目标位置之间进行插值。

如果相机混合过渡未指定目标，Cinemachine 只能独立地对位置和旋转进行插值，这通常会导致关注对象在屏幕上以不理想的方式移动。如果 Cinemachine 知道关注对象是什么，就能解决这个问题。


## 行为与扩展选择（Behavior and Extension Selection）

当你从 Cinemachine 相机组件中选择行为或添加扩展时，Unity 会自动向 Cinemachine 相机游戏对象添加额外的组件。要修改 Cinemachine 相机的行为，你需要编辑这些附加组件的属性。

> [!注意]
> 你也可以像添加其他[游戏对象组件](https://docs.unity3d.com/Manual/UsingComponents.html)一样，手动添加这些组件，以达到相同的效果。

如果没有程序化组件，Cinemachine 相机会通过其[变换（Transform）](https://docs.unity3d.com/Manual/class-Transform.html) 控制 Unity 相机的位置和旋转。

### 自定义行为与扩展（Custom Behaviors and Extensions）

你可以编写继承自 `CinemachineComponentBase` 或 `CinemachineExtension` 类的自定义脚本，以实现自己的自定义移动行为或扩展。创建此类行为或扩展后，它会自动出现在现有选项中供选择。

## 其他资源（Additional Resources）

* [向 Cinemachine 相机添加程序化行为](setup-procedural-behavior.md)