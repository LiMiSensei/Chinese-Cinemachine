# 分屏与多 Unity 相机（Split screen and multiple Unity Cameras）

根据设计，[Cinemachine 相机（CinemachineCameras）](CinemachineCamera.md) 并不直接与 [Cinemachine 控制器（CinemachineBrains）](CinemachineBrain.md) 关联。相反，控制器会动态识别场景中处于激活状态的 Cinemachine 相机，这使得这些相机可通过预制体实例化或场景加载的方式创建并生效。默认情况下，若场景中存在多个 Cinemachine 控制器，它们会识别到相同的 Cinemachine 相机，因此会显示相同的画面。若要将特定的 Cinemachine 相机分配给特定的控制器，需使用 **Cinemachine 通道（Cinemachine Channels）**，其工作原理与 Unity 图层（Layers）类似。


首先，将你的 Cinemachine 相机设置为输出到目标通道：

![Cinemachine 通道（相机端）](images/CinemachineChannels-camera.png)


接下来，将该通道添加到 Cinemachine 控制器的“通道遮罩（Channel mask）”中。遮罩中可同时包含多个通道，Cinemachine 控制器仅会使用那些输出到“通道遮罩”内通道的 Cinemachine 相机，其他 Cinemachine 相机会被忽略。

![Cinemachine 通道（控制器端）](images/CinemachineChannels-brain.png)