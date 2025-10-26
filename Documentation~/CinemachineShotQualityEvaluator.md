# Cinemachine 镜头质量评估器（Cinemachine Shot Quality Evaluator）

**Cinemachine 镜头质量评估器**是用于 [Cinemachine 相机（Camera）](CinemachineCamera.md) 的一款[扩展组件](concept-procedural-motion.md#extensions)。它会对 Cinemachine 相机的最终位置进行后处理，基于“看向目标（LookAt target）”的可见性（可选条件：以及相机与目标的距离）来评估镜头质量。

[Cinemachine 遮挡排除器（CinemachineDeoccluder）](CinemachineDeoccluder.md) 已内置此功能。若你希望在不使用遮挡排除器的情况下评估镜头质量，可使用本扩展组件。

你可以通过创建一个实现 `IShotQualityEvaluator` 接口的类，来自定义镜头质量评估器。

镜头质量评估器会使用 [物理射线投射器（Physics Raycaster）](https://docs.unity3d.com/Manual/script-PhysicsRaycaster.html)，因此，Cinemachine 镜头质量评估器要求潜在的遮挡物必须带有 [碰撞体（collider）](https://docs.unity3d.com/Manual/CollidersOverview.html) 体积。这一要求会带来一定的性能消耗；若该性能消耗对你的游戏而言过高，可考虑通过其他方式实现此功能。


## 属性（Properties）：

| **属性** | **功能** |
|:---|:---|
| **遮挡层（Occlusion Layers）** | 这些图层上的对象会被检测为遮挡物。 |
| **忽略标签（Ignore Tag）** | 带有此标签的障碍物会被忽略。建议将此字段设置为目标的标签。 |
| **与目标的最小距离（Minimum Distance From Target）** | 距离目标小于此值的障碍物会被忽略。 |
| **相机半径（Camera Radius）** | 用于检测遮挡物的球形投射（spherecast）半径。 |
| **距离评估（Distance Evaluation）** | 若启用，将基于目标距离评估镜头质量。 |
| **最佳距离（Optimal Distance）** | 若值大于 0，当目标与相机的距离为此值时，镜头质量将获得最大提升。 |
| **近距限制（Near Limit）** | 目标与相机距离小于此值的镜头，不会获得质量提升。 |
| **远距限制（Far Limit）** | 目标与相机距离大于此值的镜头，不会获得质量提升。 |
| **最大质量提升（Max Quality Boost）** | 高质量镜头的质量将在此基础上，额外提升其常规质量的指定比例（即此字段数值为提升比例）。 |