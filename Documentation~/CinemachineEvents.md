# Cinemachine 事件（Cinemachine Events）

每当相机被激活、停用，以及混合过渡（blend）开始、结束时，Cinemachine 都会生成事件。此外，当发生相机切换（camera cut）——即活跃的 Cinemachine 相机在无混合过渡的情况下切换——时，也会生成事件。

Cinemachine 发送事件时，会通过 CinemachineCore 全局发送。脚本可为此类事件添加监听器，并根据事件执行相应操作。监听器会接收所有相机触发的事件。

事件会在所有管理混合过渡的场景中生成。这包括在最高层级处理混合过渡的 Cinemachine 控制器（CinemachineBrain），也适用于自身管理子相机间混合过渡的 Cinemachine 管理器相机（Cinemachine Manager Cameras）。

有时，用户可能希望事件仅针对特定相机发送，以便脚本仅根据该特定相机的活动接收通知，无需额外编写事件筛选代码。Cinemachine 提供了以下行为组件来执行此筛选逻辑：
- 若要捕获与特定 Cinemachine 相机（CinemachineCamera）或管理器相机（ManagerCamera）的激活、停用相关的事件，可向该相机添加[Cinemachine 相机事件（Cinemachine Camera Events）](CinemachineCameraEvents.md)行为组件。
- 若要捕获管理器相机（ManagerCamera）生成的事件，可向该管理器相机添加[Cinemachine 相机管理器事件（Cinemachine Camera Manager Events）](CinemachineCameraManagerEvents.md)行为组件。
- 若要捕获特定 Cinemachine 控制器（CinemachineBrain）生成的事件，可向该控制器添加[Cinemachine 控制器事件（Cinemachine Brain Events）](CinemachineBrainEvents.md)行为组件。