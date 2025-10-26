# Cinemachine 混合器设置资源（Cinemachine Blender Settings asset）

在[Cinemachine 控制器组件（Cinemachine Brain component）](CinemachineBrain.md)中使用 Cinemachine 混合器设置资源，可针对特定的 Cinemachine 相机对定义**自定义混合（Custom Blends）** 效果（覆盖“默认混合（Default Blend）”设置）。

有关混合效果的更多信息，请参阅[相机控制与过渡（Camera control and transitions）](concept-camera-control-transitions.md)。

![Cinemachine 控制器中的自定义混合列表](images/CinemachineCustomBlends.png)

“源相机（From）”和“目标相机（To）”设置基于相机名称，而非引用关系。这意味着 Cinemachine 会通过匹配相机名称来找到对应的相机，而非与特定游戏对象（GameObject）关联。可通过内置下拉菜单从当前场景中选择 Cinemachine 相机，也可直接在文本框中输入相机名称。若输入的名称与当前场景中的任何 Cinemachine 相机都不匹配，该字段将以黄色高亮显示。

使用保留名称 **\*\*ANY CAMERA\*\***（任意相机），可实现从任意 Cinemachine 相机发起混合，或混合到任意 Cinemachine 相机。

当 Cinemachine 开始从一个 Cinemachine 相机过渡到另一个 Cinemachine 相机时，会在此资源中查找与即将发生的过渡匹配的条目，并应用该混合定义：
- 若未找到匹配条目，则将应用 Cinemachine 控制器的“默认混合（DefaultBlend）”设置。
- 若自定义混合资源中有多个条目与即将发生的过渡匹配，Cinemachine 将选择特异性最强的条目。例如，当从 vcam1 混合到 vcam2 时，若自定义混合资源中同时存在“vcam1 到任意相机（vcam1-to-AnyCamera）”和“vcam1 到 vcam2（vcam1-to-vcam2）”的条目，则会应用“vcam1 到 vcam2”的条目。
- 若自定义混合资源中有多个条目与即将发生的过渡匹配且特异性相同，则会应用第一个找到的条目。


## 属性（Properties）：

| **属性** || **功能** |
|:---|:---|:---|
| **源相机（From）** || 发起混合的 Cinemachine 相机名称。使用 **\*\*ANY CAMERA\*\*** 可从任意 Cinemachine 相机发起混合。此属性仅适用于自定义混合。 |
| **目标相机（To）** || 混合到的 Cinemachine 相机名称。使用 **\*\*ANY CAMERA\*\*** 可混合到任意 Cinemachine 相机。此属性仅适用于自定义混合。 |
| **默认混合样式（Style Default Blend）** || 混合曲线的形状（即过渡动画的变化规律）。 |
| | **切换（Cut）** | 零时长混合（即瞬间切换，无过渡过程）。 |
| | **淡入淡出（Ease In Out）** | S 形曲线，过渡过程柔和平滑。 |
| | **淡入（Ease In）** | 从源相机线性退出，平滑淡入到目标相机。 |
| | **淡出（Ease Out）** | 从源相机平滑淡出，线性过渡到目标相机。 |
| | **强淡入（Hard In）** | 从源相机平滑淡出，加速进入目标相机。 |
| | **强淡出（Hard Out）** | 从源相机加速退出，平滑淡入到目标相机。 |
| | **线性（Linear）** | 线性混合，过渡效果呈机械感。 |
| | **自定义（Custom）** | 自定义混合曲线，允许手动绘制过渡曲线。 |
| **时间（Time）** || 混合过程的持续时间（单位：秒）。 |