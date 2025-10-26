# Cinemachine 相机事件

当 Cinemachine 相机被激活时，全局事件会通过 CinemachineCore 发送。脚本可以为这些事件添加监听器，并根据事件执行操作。监听器会接收所有相机的事件。

有时，我们希望仅针对特定相机发送事件，这样脚本就可以根据该特定相机的活动收到通知，而无需编写用于过滤事件的代码。Cinemachine 相机事件组件正好满足了这一需求。

如果将其添加到 CinemachineCamera 上，它会公开基于该相机活动触发的事件。当该相机发生相关事件时，你添加的任何监听器都会被调用。

如果需要针对特定 CinemachineBrain 的事件，请参阅 [Cinemachine 大脑事件](CinemachineBrainEvents.md)。

如果需要针对特定 CinemachineCameraManager 的事件，请参阅 [Cinemachine 相机管理器事件](CinemachineCameraManagerEvents.md)。

## 属性：

| **属性：** | **功能：** |
|:---|:---|
| __事件目标__ | 这是正在监视其事件的对象。如果为空，且当前游戏对象具有 CinemachineVirtualCameraBase 组件，则会使用该组件。 |
| __相机激活事件__ | 在混合开始时调用，当相机变为激活状态时触发。参数为：brain（大脑）、incoming camera（切入相机）。硬切（cut）被视为长度为零的混合。 |
| __相机停用事件__ | 当 Cinemachine 相机停止激活时触发此事件。如果涉及混合，则该事件会在混合的最后一帧之后触发。 |
| __混合创建事件__ | 每当创建新的 Cinemachine 混合时，会触发此事件。处理程序可以修改混合的设置（但不能修改相机）。注意：时间线混合不会发送 BlendCreatedEvents，因为它们应完全由时间线控制。要修改时间线混合的混合算法，可以为 CinemachineCore.GetCustomBlender 安装处理程序。 |
| __混合完成事件__ | 每当 Cinemachine 相机完成混合切入时，会触发此事件。如果混合长度为零，则不会触发。 |