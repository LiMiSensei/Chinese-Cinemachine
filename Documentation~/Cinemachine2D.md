# 2D 图形

Cinemachine 支持正交相机。当你将 Unity 相机的投影模式设置为正交（Orthographic）时，Cinemachine 会自动调整以适配这种模式。在 Cinemachine 相机的“镜头（Lens）”属性中，“视野（FOV）”会被“正交大小（Orthographic Size）”替代。请注意，如果相机处于正交模式，所有与 FOV 相关的设置以及某些基于 FOV 的行为（如“跟随缩放（Follow Zoom）”）都将失效。

在正交环境中，旋转相机通常没有实际意义。因此，Cinemachine 提供了“位置合成器（Position Composer）”组件，用于在不旋转相机的情况下处理画面构图和取景。

当主相机带有“像素完美（Pixel Perfect）”组件时，你可以为 Cinemachine 相机添加“Cinemachine 像素完美扩展（Cinemachine Pixel Perfect extension）”，使其能与像素完美环境良好兼容。

若要将相机限制在 2D 世界的特定区域内，可使用“Cinemachine 2D 限制器扩展（Cinemachine Confiner 2D extension）”。