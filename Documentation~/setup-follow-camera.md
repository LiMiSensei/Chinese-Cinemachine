# 跟随角色并为其构图

创建并设置一台可自动跟随角色并为角色构图的 Cinemachine 相机。

> [!注意]
> 你的场景中必须包含一个可作为目标的游戏对象（GameObject），以便 Cinemachine 相机跟随它。


## 添加“跟随”型 Cinemachine 相机

1. 在 Unity 菜单中，选择 **游戏对象（GameObject）> Cinemachine > 目标相机（Targeted Cameras）> 跟随相机（Follow Camera）**。

   Unity 会添加一个新的游戏对象，该对象包含：
   * 一个 [Cinemachine 相机组件](CinemachineCamera.md)，
   * 一个 [Cinemachine 跟随组件](CinemachineFollow.md)（负责处理 Cinemachine 相机的“位置控制（Position Control）”行为），
   * 一个 [Cinemachine 旋转构图器组件](CinemachineRotationComposer.md)（负责处理 Cinemachine 相机的“旋转控制（Rotation Control）”行为）。

2. [确认](setup-cinemachine-environment.md#verify-the-cinemachine-brain-presence) Unity 相机已包含 [Cinemachine 控制器组件](CinemachineBrain.md)。

3. 在检视面板（Inspector）的 **Cinemachine 相机（Cinemachine Camera）** 组件中，设置 **跟踪目标（Tracking Target）** 属性，指定需要跟随和瞄准的游戏对象。

   此后，无论该游戏对象在场景中如何移动，Cinemachine 相机都会自动调整 Unity 相机相对于该对象的位置，并旋转相机以瞄准该对象。


> [!注意]
> 如果你在想要跟随的游戏对象上右键点击，然后通过弹出菜单调用 **跟随相机（Follow Camera）** 选项，新相机的“跟踪目标（Tracking Target）”会自动填充为你右键点击的那个对象。


## 调整 Cinemachine 相机的行为

1. 通过检视面板（Inspector）访问 [Cinemachine 相机组件](CinemachineCamera.md) 的属性，进行进一步配置。

2. 调整以下属性（示例）：
   * 跟随偏移（follow offset）
   * 跟随阻尼（follow damping）
   * 屏幕构图（screen composition）
   * 相机重新瞄准时的阻尼（damping used when re-aiming the camera）
   * 镜头设置（Lens settings）