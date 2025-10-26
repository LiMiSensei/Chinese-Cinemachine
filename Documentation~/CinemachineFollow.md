# Cinemachine 跟随组件（Cinemachine Follow component）

这款 Cinemachine 相机的**位置控制（Position Control）** 行为，会通过移动 Cinemachine 相机，使其与**跟踪目标（Tracking Target）** 保持固定偏移距离，同时还会应用阻尼效果。

根据“绑定模式（Binding Mode）”的不同设置，固定偏移距离的解读方式也会有所差异。


## 属性（Properties）

| 属性 | 功能 |
| :--- | :--- |
| **[绑定模式（Binding Mode）](#binding-modes)** | 如何解读相机与目标之间的偏移距离，具体包含以下选项：<ul> <li>**世界空间（World Space）**：偏移距离以跟踪目标的原点为基准，在世界空间中进行解读。当目标旋转时，相机位置不会发生变化。</li> <li>**锁定到目标（Lock To Target）**：使 Cinemachine 相机采用跟踪目标的局部坐标系。当目标旋转时，相机将随之移动，以保持偏移距离不变，并维持对目标的相同视角。</li> <li>**锁定到目标（世界上方向）（Lock To Target With World Up）**：使 Cinemachine 相机采用跟踪目标的局部坐标系，但将倾斜角（tilt）和侧滚角（roll）设为 0。此绑定模式仅考虑目标的偏航角（yaw），忽略其他旋转角度。</li> <li>**锁定到目标（无侧滚）（Lock To Target No Roll）**：使 Cinemachine 相机采用跟踪目标的局部坐标系，但将侧滚角（roll）设为 0。</li> <li>**绑定目标时锁定（Lock To Target On Assign）**：在 Cinemachine 相机激活或指定跟踪目标的瞬间，使相机的朝向与跟踪目标的局部坐标系保持一致。此后，该偏移距离在世界空间中保持固定，相机不会随目标旋转而旋转。</li> <li>**惰性跟随（Lazy Follow）**：在相机局部空间中解读偏移距离和阻尼值。此模式模拟人类摄影师跟随目标时的操作逻辑——相机尽量以最小的移动幅度保持与目标的距离，无需关注相机相对于目标的朝向。无论目标朝向如何，相机都会尝试与目标保持固定的距离和高度。</li> </ul> |
| **跟随偏移（Follow Offset）** | Cinemachine 相机相对于目标的期望偏移距离。将 X、Y、Z 轴数值均设为 0 时，相机将位于目标中心点。默认值分别为 0、0、-10，即相机位于目标后方。 |
| **位置阻尼（Position Damping）** | 相机在 X、Y、Z 轴方向上维持偏移距离的响应速度。数值越小，相机响应越灵敏；数值越大，相机响应越缓慢。 |
| **角度阻尼模式（Angular Damping Mode）** | 包含“欧拉角（Euler）”和“四元数（Quaternion）”两种模式。<br> - 欧拉角模式：可分别为俯仰角（Pitch）、侧滚角（Roll）和偏航角（Yaw）设置阻尼值，但可能出现万向节锁问题。<br> - 四元数模式：仅需设置一个阻尼值，且可避免万向节锁问题。 |
| **旋转阻尼（Rotation Damping）** | 当处于“欧拉角角度阻尼模式”时，相机跟踪目标俯仰角、偏航角和侧滚角的响应速度。数值越小，相机响应越灵敏；数值越大，相机响应越缓慢。 |
| **四元数阻尼（Quaternion Damping）** | 当处于“四元数角度阻尼模式”时，相机跟踪目标旋转的响应速度。 |


## 绑定模式（Binding Modes）

[!include[](includes/binding-modes.md)]