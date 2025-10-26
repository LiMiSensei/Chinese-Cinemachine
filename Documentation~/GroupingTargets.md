# 跟随并构图多个目标（Follow and Frame a Group）

使用 Cinemachine 目标组（Cinemachine Target Group），可将多个变换组件（Transform）视为单个跟踪目标（Tracking Target）。

该组件还可用作需要知晓目标尺寸的程序化行为的目标，例如 [群组构图（Group Framing）](CinemachineGroupFraming.md) 扩展组件。


## 创建目标组（To create a Target Group）
* 向一个空游戏对象（empty GameObject）添加 **CinemachineTargetGroup 组件**。


## 创建带有目标组的新 Cinemachine 相机（To create a new Cinemachine Camera with a Target Group）
* 在 Unity 菜单中，选择 **游戏对象（GameObject）> Cinemachine > 目标相机（Targeted Cameras）> 目标组相机（Target Group Camera）**。

  Unity 会向场景中添加一个新的 Cinemachine 相机（CinemachineCamera）和一个目标组（Target Group）。该 Cinemachine 相机的**跟踪目标（Tracking Target）** 会指向这个新的目标组。


## 将现有 Cinemachine 相机的目标转换为目标组（To convert an existing CinemachineCamera target to a target group）
* 在 Cinemachine 相机的检视面板（Inspector）中，点击“跟踪目标（Tracking Target）”字段右侧的弹出菜单，选择 **转换为目标组（Convert to TargetGroup）**。此操作会创建一个新的目标组游戏对象，将当前目标添加到该组中，并将 Cinemachine 相机的“跟踪目标”设置为这个新组。


## 填充目标组（To Populate a Target Group）
1. 在 [层级窗口（Hierarchy）](https://docs.unity3d.com/Manual/Hierarchy.html) 中，选中新创建的目标组对象（Target Group object）。
2. 在 [检视面板（Inspector）](https://docs.unity3d.com/Manual/UsingTheInspector.html) 中，点击“+”号向组中添加新项。
3. 在新添加的项中，指定一个游戏对象（可从层级窗口中拖拽赋值），并编辑**权重（Weight）** 和**半径（Radius）** 属性。需注意，务必为目标设置非零权重，否则该目标会被组忽略。
4. 若要向目标组中添加更多游戏对象，重复步骤 2-3 即可。

![包含两个目标的 Cinemachine 目标组](images/CinemachineTargetGroup.png)