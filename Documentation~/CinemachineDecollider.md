# Cinemachine 防碰撞器（Cinemachine Decollider）

**Cinemachine 防碰撞器**是用于[Cinemachine 相机（Camera）](CinemachineCamera.md)的一款[扩展组件](concept-procedural-motion.md#extensions)。它会对 Cinemachine 相机的最终位置进行后处理，将相机从发生碰撞的物体中“拉离”。尽管碰撞会朝着相机目标的方向解决，但该组件不会尝试保持相机到目标的视线畅通。若需实现视线畅通功能，请使用[Cinemachine 去遮挡器（CinemachineDeoccluder）](CinemachineDeoccluder.md)。

防碰撞器整合了两种算法：
1. **地形解析（Terrain Resolution）**：从相机上方朝正下方发射一条射线。若射线击中相机上方的碰撞体，且该碰撞体属于指定的“地形图层（Terrain Layers）”，则相机会向上移动至射线击中的位置，使相机处于该碰撞体的顶部。
2. **障碍物解析（Obstacle Resolution）**：检测指定图层中与相机重叠的障碍物，并将相机朝着目标方向移出障碍物区域。若相机的球形范围未实际与物体相交，则相机不会移动。

若某一图层同时存在于“地形解析图层遮罩”和“障碍物防碰撞图层遮罩”中，则该图层仅会被地形算法处理，不会被障碍物防碰撞算法处理。

防碰撞器使用[物理射线投射器（Physics Raycaster）](https://docs.unity3d.com/Manual/script-PhysicsRaycaster.html)，因此 Cinemachine 防碰撞器要求潜在障碍物必须具备[碰撞体（collider）](https://docs.unity3d.com/Manual/CollidersOverview.html)体积。这一要求会带来一定的性能消耗；若该消耗对游戏性能影响较大，可考虑通过其他方式实现此功能。


## 属性（Properties）：

| **属性** | **功能** |
|:---|:---|
| **相机半径（Camera Radius）** | 相机与任何障碍物或地形之间需保持的距离。为获得最佳效果，建议将此值设置得较小；若需防止相机与障碍物近边缘发生“穿模”，可增大此值。 |
| **防碰撞（Decollision）** | 启用后，会尝试将相机从相交的物体中推出。 |
| **障碍物图层（Obstacle Layers）** | 会被检测的障碍物所在的图层。 |
| **使用跟随目标（Use Follow Target）** | 启用后，防碰撞器会将相机朝“跟随目标（Follow Target）”方向移动，而非“看向目标（LookAt Target）”方向。 |
| **Y 轴偏移（Y Offset）** | 启用“使用跟随目标”后，跟随目标的 Y 轴位置会在其局部垂直方向上偏移此数值。 |
| **地形解析（Terrain Resolution）** | 启用后，会尝试将相机放置在地形图层的顶部。 |
| **地形图层（Terrain Layers）** | 会被检测的地形碰撞体所在的图层。 |
| **最大射线投射距离（Maximum Raycast）** | 指定用于检测地形碰撞体的射线投射的最大长度。 |
| **阻尼（Damping）** | 当不再需要调整相机位置时，相机恢复到正常位置的速度。数值越小，相机响应越灵敏；数值越大，相机响应越缓慢。 |
| **平滑时间（Smoothing Time）** | 相机在离目标最近位置需保持的最短时间（单位：秒）。在障碍物较多的环境中，可通过此属性减少相机的过度移动。 |