# Cinemachine 相机管理器事件（Cinemachine Camera Manager Events）

当 Cinemachine 相机被激活时，会通过 CinemachineCore 发送全局事件。脚本可为此类事件添加监听器，并根据事件执行相应操作。监听器会接收所有相机和所有控制器（Brain）触发的事件。

有时，用户可能希望事件仅针对特定的 Cinemachine 相机管理器（Cinemachine Camera Manager）发送，以便脚本仅根据该特定对象的活动接收通知，无需额外编写事件筛选代码。Cinemachine 控制器事件（Cinemachine Brain Events）组件正是为满足这一需求而设计的。

该组件会暴露基于目标对象活动触发的事件。当目标对象触发这些事件时，你添加的所有监听器都会被调用。可在“相机管理器（Camera Manager）”字段中明确指定目标对象；若将该字段设为 null，并将此脚本直接添加到带有 Cinemachine 控制器（Cinemachine Brain）组件的对象上，也可实现同样效果。

若你需要针对特定 Cinemachine 相机触发的事件，请参阅[Cinemachine 相机事件（Cinemachine Camera Events）](CinemachineCameraEvents.md)。

若你需要针对特定 Cinemachine 控制器触发的事件，请参阅[Cinemachine 控制器事件（Cinemachine Brain Events）](CinemachineBrainEvents.md)。


## 属性（Properties）：

| **属性** | **功能** |
|:---|:---|
| **相机管理器（Camera Manager）** | 发送事件的 Cinemachine 相机管理器（CinemachineCameraManager）。若该字段为 null，且当前游戏对象（GameObject）带有 Cinemachine 相机管理器组件，则会使用该组件。 |
| **相机激活事件（Camera Activated Event）** | 当相机开始生效（变为活跃状态）时，在混合过渡（blend）开始时调用。参数包括：控制器（brain）、新激活的相机（incoming camera）。“切换（cut）”操作被视为时长为 0 的混合过渡。 |
| **相机停用事件（Camera Deactivated Event）** | 每当某个 Cinemachine 相机停止生效（不再活跃）时触发。若涉及混合过渡，则在混合过渡的最后一帧之后触发。 |
| **混合过渡创建事件（Blend Created Event）** | 每当创建新的 Cinemachine 混合过渡时触发。事件处理器可修改混合过渡的设置（但无法修改相机本身）。注意：时间线（timeline）混合过渡不会发送此事件，因为时间线混合过渡应 100% 由时间线控制。若需修改时间线混合过渡的算法，可为 CinemachineCore.GetCustomBlender 添加事件处理器。 |
| **混合过渡完成事件（Blend Finished Event）** | 每当某个 Cinemachine 相机完成混合过渡并完全生效时触发。若混合过渡时长为 0（即切换操作），则不会触发此事件。 |
| **相机切换事件（Camera Cut Event）** | 当发生时长为 0 的混合过渡（即切换操作）时调用。 |