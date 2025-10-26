# Cinemachine 去遮挡器（Cinemachine Deoccluder）

**Cinemachine 去遮挡器**是用于 Cinemachine 相机（CinemachineCamera）的一款[扩展组件](concept-procedural-motion.md#extensions)。它会对 Cinemachine 相机的最终位置进行后处理，力求保持相机与自身“看向目标（Look At Target）”之间的视线畅通，具体实现方式是让相机远离遮挡视野的游戏对象。

为 Cinemachine 相机添加 Cinemachine 去遮挡器扩展组件，可实现以下任一功能：
- 将相机推离场景中遮挡视野的障碍物；
- 将相机放置在遮挡物前方（当该遮挡物处于 Cinemachine 相机与“看向目标”之间时）；
- 评估镜头质量（Shot Quality）。镜头质量是对以下因素的综合衡量：Cinemachine 相机与理想位置的距离、相机与目标的距离，以及遮挡目标视野的障碍物。其他模块（包括[清晰视角相机（Clear Shot）](CinemachineClearShot.md)）会用到镜头质量这一参数。

去遮挡器使用[物理射线投射（Physics Raycaster）](https://docs.unity3d.com/ScriptReference/Physics.Raycast.html)功能，因此 Cinemachine 去遮挡器要求潜在的障碍物必须具备[碰撞体（collider）](https://docs.unity3d.com/Manual/CollidersOverview.html)体积。这一要求会带来一定的性能消耗；若该消耗对游戏性能影响较大，可考虑通过其他方式实现此功能。


## 属性（Properties）：

| **属性** || **功能** |
|:---|:---|:---|
| **碰撞检测图层（Collide Against）** || Cinemachine 去遮挡器会将这些图层中的游戏对象视为潜在障碍物，忽略未选中图层中的游戏对象。 |
| **与目标的最小距离（Minimum Distance From Target）** || 忽略与目标支点距离小于此值的障碍物。 |
| **避开障碍物（Avoid Obstacles）** || 启用后，当目标被障碍物遮挡时，去遮挡器会移动场景中的相机。可通过“距离限制（Distance Limit）”“相机半径（Camera Radius）”“策略（Strategy）”“平滑时间（Smoothing Time）”“最小遮挡时间（Minimum Occlusion Time）”“阻尼（Damping）”和“遮挡时阻尼（DampingWhenOccluded）”等属性，调整避开障碍物的具体方式。若禁用此选项，Cinemachine 去遮挡器仍会根据障碍物评估镜头质量，但不会尝试移动相机以优化镜头效果。 |
| **距离限制（Distance Limit）** || 检查相机到目标的视线是否畅通时，射线投射的最大距离。输入 0 则使用相机到目标的当前实际距离。仅当“避开障碍物”勾选时可用。 |
| **使用跟随目标（Use Follow Target）** || 启用后，去遮挡器会将相机朝“跟随目标（Follow Target）”方向移动，而非“看向目标（LookAt Target）”方向。 |
| **Y 轴偏移（Y Offset）** || 启用“使用跟随目标”后，跟随目标的 Y 轴位置会在其局部垂直方向上偏移此数值。 |
| **相机半径（Camera Radius）** || 相机与任何障碍物之间需保持的距离。为获得最佳效果，建议将此值设置得较小；若因相机视野（FOV）过大而导致能看到障碍物内部，可增大此值。仅当“避开障碍物”勾选时可用。 |
| **策略（Strategy）** || 去遮挡器用于保持目标视线畅通的方式。仅当“避开障碍物”勾选时可用。 |
| | **向前拉相机（Pull Camera Forward）** | 沿相机 Z 轴正方向移动相机，直至相机位于离目标最近的障碍物前方。 |
| | **保持相机高度（Preserve Camera Height）** | 移动相机至其他视角，同时尽量保持相机的原始高度。 |
| | **保持相机距离（Preserve Camera Distance）** | 移动相机至其他视角，同时尽量保持相机与目标的原始距离。 |
| **平滑时间（Smoothing Time）** || 相机在离目标最近位置需保持的最短时间（单位：秒）。在障碍物较多的环境中，可通过此属性减少相机的过度移动。仅当“避开障碍物”勾选时可用。 |
| **阻尼（Damping）** || 当遮挡消失后，相机恢复到正常位置的速度。数值越小，相机响应越灵敏；数值越大，相机响应越缓慢。仅当“避开障碍物”勾选时可用。 |
| **遮挡时阻尼（Damping When Occluded）** || 相机移动以避开障碍物的速度。数值越小，相机响应越灵敏；数值越大，相机响应越缓慢。仅当“避开障碍物”勾选时可用。 |
| **透明图层（Transparent Layers）** || 这些图层中的对象永远不会遮挡目标的视野。 |
| **最小遮挡时间（Minimum Occlusion Time）** || 仅当遮挡持续时间至少达到此值时，才会执行避开障碍物的操作。 |
| **最大处理次数（Maximum Effort）** || 处理障碍物碰撞检测的最大次数上限。数值越高，可能对性能影响越大。在大多数环境中，4 次碰撞检测已足够。 |
| **忽略标签（Ignore Tag）** || 带有此标签的障碍物会被忽略。建议将此字段设为目标的标签。 |
| **镜头质量评估（Shot Quality Evaluation）** || 启用后，当目标距离接近最优距离时，镜头会获得更高的质量分数。 |
| | **最优距离（Optimal Distance）** | 当目标与相机的距离接近此值时，镜头将获得最大的质量提升。 |
| | **近限（Near Limit）** | 当目标距离小于最优距离时，质量提升会逐渐降低；当达到近限时，质量不再获得提升。 |
| | **远限（Far Limit）** | 当目标距离大于最优距离时，质量提升会逐渐降低；当达到远限时，质量不再获得提升。 |
| | **最大质量提升（Max Quality Boost）** | 当目标处于最优距离时，镜头可获得的质量提升幅度，以默认质量的比例表示。例如，若此值为 0.5，镜头质量将乘以 1.5（即提升 50%）。 |