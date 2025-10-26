# 设置多台 Cinemachine 相机与过渡效果

搭建包含多台 Cinemachine 相机的 Cinemachine 环境，并管理[相机控制与过渡效果](concept-camera-control-transitions.md)，步骤如下：

* 创建多台具有不同属性的 Cinemachine 相机，
* 在 Cinemachine 控制器（Cinemachine Brain）中管理 Cinemachine 相机的过渡效果，
* 在播放模式（Play mode）下测试激活相机（Live Camera）的触发机制与过渡效果。


> [!注意]
> 你的场景中必须只包含一个 Unity 相机——即带有 [Camera 组件](https://docs.unity3d.com/Manual/class-Camera.html) 的游戏对象（GameObject）。


## 添加 Cinemachine 相机

1. 在场景视图（Scene view）中，通过 [场景导航操作](https://docs.unity3d.com/Manual/SceneViewNavigation.html) 调整到你希望用某一台 Cinemachine 相机构图的视角。

2. 在 Unity 菜单中，选择 **游戏对象（GameObject）> Cinemachine > Cinemachine 相机（Cinemachine Camera）**。

   Unity 会添加一个新的游戏对象，该对象包含一个 [Cinemachine 相机组件](CinemachineCamera.md)，以及一个与场景视图相机最新位置和朝向匹配的变换组件（Transform）。

   此时，你还可以通过[之前的方法](setup-cinemachine-environment.md#verify-the-cinemachine-brain-presence)确认 Unity 相机已包含 [Cinemachine 控制器组件](CinemachineBrain.md)。

3. 继续导航场景，并以相同方式创建更多台 Cinemachine 相机，且为每台相机设置不同的位置和旋转角度。

4. 为每台 Cinemachine 相机命名，以便后续能轻松识别它们。


## 管理 Cinemachine 相机之间的过渡效果

1. 在层级窗口（Hierarchy）中，选中你的 Unity 相机——即带有 Camera 组件的游戏对象。

2. 在检视面板（Inspector）的 [Cinemachine 控制器组件](CinemachineBrain.md) 中，进行以下任一操作：
   * 选择一个**默认混合过渡（Default Blend）**，用于所有 Cinemachine 相机之间的过渡，或者
   * 创建并指定一个定义了**自定义混合过渡（Custom Blends）** 的资源，用于特定成对 Cinemachine 相机之间的过渡。


## 在播放模式下测试过渡效果

1. 进入 [播放模式](https://docs.unity3d.com/Manual/GameView.html)。

2. 依次更改每台 Cinemachine 相机游戏对象的 [激活状态](https://docs.unity3d.com/Manual/class-GameObject.html)，观察它们是否会按照你在 Cinemachine 控制器中的设置，在彼此之间进行混合过渡。

3. 退出播放模式。


## 后续步骤

以下是你接下来可能需要执行的操作：

* [创建带有程序化行为的 Cinemachine 相机：例如跟随角色的相机](setup-procedural-behavior.md)。
* [使用时间线（Timeline）管理 Cinemachine 相机镜头的编排序列](setup-timeline.md)。


## 其他资源

* [以第一人称视角预览和制作 Cinemachine 相机](preview-and-author-in-first-person.md)