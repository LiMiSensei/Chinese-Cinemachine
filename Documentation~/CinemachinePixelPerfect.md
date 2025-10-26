# 使用 Cinemachine 像素完美扩展（Using the Cinemachine Pixel Perfect extension）

**像素完美相机（Pixel Perfect Camera）** 和 Cinemachine 都会修改相机的正交尺寸（orthographic size）。若在同一个场景中同时使用这两个系统，它们会争夺相机的控制权，进而产生不符合预期的结果。而**Cinemachine 像素完美（Cinemachine Pixel Perfect）** 扩展可解决这一兼容性问题。

**Cinemachine 像素完美**是用于 **Cinemachine 相机（CinemachineCamera）** 的一款[扩展组件](concept-procedural-motion.md#extensions)，能够调整 Cinemachine 相机的正交尺寸。该扩展会检测“像素完美相机”组件是否存在，并利用该组件的设置，计算出能让精灵（Sprites）最大程度保持像素完美分辨率的 Cinemachine 相机正交尺寸。


要为 Cinemachine 相机添加此扩展组件，需在 Cinemachine 相机的检视面板（Inspector）中，通过 **Add Extension（添加扩展）** 下拉菜单进行操作。请为项目中的每一台 Cinemachine 相机都添加此扩展。

对于每台附加了该扩展的 Cinemachine 相机，在**运行模式（Play Mode）** 下或启用 **编辑模式运行（Run In Edit Mode）** 后，“像素完美相机”组件会计算出一个像素完美的正交尺寸，该尺寸会尽可能匹配 Cinemachine 相机的原始尺寸。这样做是为了在应用像素完美计算后，尽可能让每台 Cinemachine 相机的原始构图保持不变。


当 [Cinemachine 控制器（Cinemachine Brain）](CinemachineBrain.md) 组件在多台 Cinemachine 相机之间进行[混合过渡（blends）](CinemachineBlending.md)时，在相机切换的过渡过程中，渲染出的图像暂时不会保持像素完美；只有当视图完全切换到某一台 Cinemachine 相机后，图像才会恢复像素完美效果。


该扩展目前存在以下限制：
- 若带有“像素完美扩展”的 Cinemachine 相机设置为跟随 [目标群组（Target Group）](CinemachineTargetGroup.md)，且通过“构图转换器（Framing Transposer）”组件调整相机位置时，可能会出现明显的画面卡顿（choppiness）。
- 若在“像素完美相机”上启用了 **放大渲染纹理（Upscale Render Texture）** 选项，能与 Cinemachine 相机原始正交尺寸匹配的像素完美分辨率会更少。这可能导致在应用像素完美计算后，Cinemachine 相机的构图出现较大偏差。


### 术语补充说明（游戏开发场景适配）
- **Pixel Perfect（像素完美）**：指游戏画面中精灵、UI 等元素的像素与屏幕像素完全对齐，无拉伸、模糊或失真，常见于 2D 游戏，以还原像素风格的精细视觉效果。
- **Orthographic Size（正交尺寸）**：正交相机（Orthographic Camera）的属性，决定相机可视范围的高度，数值越小，可视范围越窄（类似“放大”效果），数值越大，可视范围越宽（类似“缩小”效果）。
- **Framing Transposer（构图转换器）**：Cinemachine 中的一种位置控制组件，用于根据跟踪目标自动调整相机位置，确保目标始终处于画面合适位置。