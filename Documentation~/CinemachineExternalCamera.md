# Cinemachine 外部相机（Cinemachine External Camera）-（已废弃）

**注意**：此组件已**废弃**，建议改用普通的 Cinemachine 相机，并将其“位置控制（Position Control）”和“旋转控制（Rotation Control）”均设为“无（None）”。

该组件可将非 Cinemachine 相机接入 Cinemachine 生态系统，使其能够参与混合过渡（blend）。只需将其作为组件，与已有的 Unity 相机组件（Unity Camera component）一同添加到对象上即可。你需要采取一些措施（例如禁用相机组件），以确保该相机不会与主 Cinemachine 相机产生冲突（即避免两者同时控制画面）。


## 属性（Properties）：

| **属性** || **功能** |
|:---|:---|:---|
| **看向目标（Look At）** || 若已定义，指相机所对准的对象。此属性可为空，但设置该属性可能会提升与该相机之间混合过渡的效果质量。 |
| **混合提示（Blend Hint）** || 为与该相机之间的位置混合提供提示，多个值可组合使用。 |
| | **球面位置（Spherical Position）** | 混合过程中，相机会围绕跟踪目标（Tracking Target）沿球面路径运动。 |
| | **柱面位置（Cylindrical Position）** | 混合过程中，相机会围绕跟踪目标沿柱面路径运动（垂直坐标采用线性插值）。 |
| | **目标不同时按屏幕空间对准（Screen Space Aim When Targets Differ）** | 混合过程中，跟踪目标的位置会在屏幕空间而非世界空间中进行插值。 |
| | **继承位置（Inherit Position）** | 当该相机生效（live）时，若条件允许，会强制其初始位置与 Unity 相机的当前位置保持一致。 |
| | **忽略目标（IgnoreTarget）** | 混合旋转时不考虑跟踪目标，仅采用球面插值（spherical interpolation）。 |


### 术语补充说明
- **deprecated**：已废弃（指该组件不再被官方推荐使用，未来可能从版本中移除，建议优先采用替代方案）；
- **non-cinemachine camera**：非 Cinemachine 相机（指未添加 Cinemachine 相关核心组件、由 Unity 原生相机系统控制的相机）；
- **blends**：混合过渡（Cinemachine 中不同相机之间切换时的平滑过渡效果，区别于“瞬间切换（cut）”）；
- **spherical interpolation**：球面插值（一种在 3D 空间中平滑计算旋转或位置过渡的算法，确保过渡路径符合球面几何特性，避免万向节锁等问题）。