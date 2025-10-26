# 随跟随目标旋转（Rotate With Follow Target）

此 Cinemachine 相机的**旋转控制（Rotation Control）** 行为会匹配**跟踪（Tracking）** 目标的旋转角度。当与 [位置控制（Position Control）](CinemachineCamera.md#set-procedural-components-and-add-extension) 中的 [强制锁定到目标（Hard Lock To Target）](CinemachineHardLockToTarget.md) 行为配合使用时，最终可实现让 Cinemachine 相机匹配控制用游戏对象（control GameObject）的运动路径和旋转角度。


### 属性（Properties）

| 属性 | 说明 |
| --- | --- |
| **阻尼（Damping）** | 为相机的响应添加延迟效果。数值表示相机大致需要多长时间（以秒为单位）才能追上目标的旋转。 |