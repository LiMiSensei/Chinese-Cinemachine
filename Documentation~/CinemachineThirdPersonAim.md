# Cinemachine 第三人称瞄准扩展（Cinemachine Third Person Aim Extension）

此扩展是 Cinemachine 相机中 [第三人称跟随组件（ThirdPersonFollow component）](CinemachineThirdPersonFollow.md) 的补充组件，其用途是检测相机正瞄准的对象。

为实现这一功能，该扩展会从相机位置沿其前向轴投射一条射线，检测与该射线相交的第一个对象。随后，相交点会被赋值到 Cinemachine 相机的 `state.ReferenceLookAt` 属性中。对于需要知晓相机“瞄准点”的算法（例如混合过渡算法）而言，此相交点将被视为相机的瞄准目标。

此外，若启用了**噪声抵消（Noise Cancellation）** 功能，即使相机启用了手持噪声效果，也可通过此扩展将目标稳定在屏幕中心。旋转噪声会被抵消，但如果相机存在位置噪声（且噪声偏移量非零），该位置噪声会被保留，同时系统会修正瞄准方向，以确保目标在屏幕上保持稳定。

> [!注意]
> **第三人称带瞄准模式（ThirdPersonWithAimMode）** [示例场景（sample scene）](samples-tutorials.md) 提供了此扩展的使用示例。


## 属性（Properties）：

| **属性** | **功能** |
|:---|:---|
| **瞄准碰撞筛选器（Aim Collision Filter）** | 仅检测这些图层（Layer）上的对象。 |
| **忽略标签（Ignore Tag）** | 带有此标签（Tag）的对象会被忽略。建议将此字段设置为目标对象的标签。 |
| **瞄准距离（Aim Distance）** | 对象检测射线的投射距离。 |
| **噪声抵消（Noise Cancellation）** | 启用后，当相机存在手持噪声效果时，瞄准目标会在屏幕上保持稳定。此功能仅在噪声组件（Noise component）中的“轴心偏移（Pivot Offset）”非零时生效。 |


## 视差问题（Parallax issues）

通常情况下，玩家发射原点（子弹发射的位置）与相机位置之间存在偏移。若玩家沿自身前向轴（在第三人称装备中，该轴始终与相机前向轴平行）发射子弹，子弹会因上述偏移而始终无法命中目标。在大多数第三人称场景中，合理的处理方式是**忽略这种偏差**，默认相机瞄准的目标就是玩家的瞄准目标，并直接向该目标点发射即可。

但在某些情况下，可能存在某个对象遮挡了玩家对目标的视线，却未遮挡相机对目标的视线。这种情况下，玩家发射的子弹会击中该遮挡对象，而非相机瞄准的目标。Cinemachine 会检测这种情况，并计算出玩家实际发射时会命中的点。该命中点可通过 API（`CinemachineThirdPersonAim.AimTarget`）获取。