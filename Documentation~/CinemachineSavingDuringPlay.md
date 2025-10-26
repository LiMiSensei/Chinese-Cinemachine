# 运行模式下保存（Saving in Play Mode）

在游戏运行时调整相机设置通常是最便捷的方式，但默认情况下，退出运行模式（Play Mode）时，Unity 不会将你所做的修改保存到场景中。Cinemachine 提供了一项特殊功能，可保留你在运行模式下进行的微调。需要注意的是，该功能不保存结构性修改（如添加或移除行为）；除部分特定属性外，退出运行模式时，Cinemachine 可保留 Cinemachine 相机（CinemachineCamera）中的大部分设置。

退出运行模式后，Cinemachine 会扫描场景，收集 Cinemachine 相机中所有被修改过的属性，并在退出约一秒后保存这些修改。若需撤销这些修改，可使用 **编辑（Edit）> 撤销（Undo）** 命令。

在任意 Cinemachine 相机的[检视面板（Inspector）](https://docs.unity3d.com/Manual/UsingTheInspector.html)中，勾选 **运行时保存（Save During Play）** 即可启用该功能。这是一项全局属性，而非按相机单独设置的属性，因此只需勾选或取消勾选一次即可作用于所有相机。

Cinemachine 组件通过特殊属性 `[SaveDuringPlay]` 实现此功能。若某个字段需排除在保存范围外，只需为该字段添加 `[NoSaveDuringPlay]` 属性即可。

> [!提示]
> 你可以在自定义脚本中使用 `[SaveDuringPlay]` 和 `[NoSaveDuringPlay]` 属性，为脚本赋予相同的保存控制功能。