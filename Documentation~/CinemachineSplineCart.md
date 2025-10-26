# Cinemachine 样条轨道小车（Cinemachine Spline Cart）
**Cinemachine 样条轨道小车**是一款组件，可将其所在游戏对象的变换（transform）约束到**样条线（Spline）** 上。可使用它让游戏对象沿路径动画移动，或作为 Cinemachine 相机（CinemachineCamera）的跟踪目标。


## 属性（Properties）：

| **属性** || **功能** |
|:---|:---|:---|
| **样条线（Spline）** || 小车要跟随的样条线。 |
| **更新方式（Update Method）** || 当速度非零时，小车的移动更新时机。常规更新可使用 **Update（更新）** 或 **LateUpdate（延迟更新）**；若需与物理模块同步更新，可使用 **Fixed Update（固定更新）**。 |
| **位置单位（Position Units）** || “位置（Position）”属性所使用的计量单位。  |
| | **节点（Knot）** | 以样条线上的节点为单位。值为 0 表示样条线的第一个节点，值为 1 表示第二个节点，以此类推；非整数值表示节点之间的点。 |
| | **距离（Distance）** | 以常规距离单位（如米、单位长度等）表示的沿样条线距离，值为 0 表示样条线的起点。 |
| | **归一化（Normalized）** | 值为 0 表示样条线的起点，值为 1 表示样条线的终点（即无论样条线实际长度如何，均将其整体长度视为 1 个单位）。 |
| **速度（Speed）** || 小车以该速度移动，数值的解读方式与“位置单位（Position Units）”的设置一致。 |
| **位置（Position）** || 小车在样条线上的放置位置。可直接为该属性制作动画；若速度非零，其值将根据“更新方式（Update Method）”的设定，在指定时机自动更新。数值的解读方式与“位置单位（Position Units）”的设置一致。 |
| **自动移动（Automatic Dolly）** || 控制是否启用沿样条线的自动移动功能。 |
| **方式（Method）** || 控制自动移动的实现方式。可通过编写自定义的 SplineAutoDolly.ISplineAutoDolly 类，为此功能扩展自定义逻辑。 |
| | **无（None）** | 不启用自动移动。需通过设置“路径位置（PathPosition）”属性，手动控制 Cinemachine 相机在样条线上的位置。 |
| | **固定速度（Fixed Speed）** | 相机以设定的固定速度沿路径移动。 |
| | **到目标的最近点（Nearest Point To Target）** | 将相机定位到样条线上与“跟踪目标（Tracking Target）”位置最近的点。此模式要求 Cinemachine 相机必须设置跟踪目标；也可指定与最近点的偏移量，用于微调相机位置。 |