# 搭建基础 Cinemachine 环境

为你的 Unity 项目配置搭建可正常运行的 Cinemachine 环境所需的[最基础元素](concept-essential-elements.md)，步骤如下：

* 创建一个被动式 Cinemachine 相机（默认无特定行为），
* 确保 Unity 相机中包含 Cinemachine 控制器（Cinemachine Brain），
* 调整 Cinemachine 相机的属性，并观察其对 Unity 相机的影响。


> [!注意]
> 你的场景中必须只包含一个 Unity 相机——即带有 [Camera 组件](https://docs.unity3d.com/Manual/class-Camera.html) 的游戏对象（GameObject）。


## 添加 Cinemachine 相机

1. 在场景视图（Scene view）中，通过 [场景导航操作](https://docs.unity3d.com/Manual/SceneViewNavigation.html) 调整到你希望用 Cinemachine 相机构图的视角。

2. 在 Unity 菜单中，选择 **游戏对象（GameObject）> Cinemachine > Cinemachine 相机（Cinemachine Camera）**。

   Unity 会添加一个新的游戏对象，该对象包含：
   * 一个 [Cinemachine 相机组件](CinemachineCamera.md)，
   * 一个与场景视图相机最新位置和朝向匹配的变换组件（Transform）。


## 确认 Cinemachine 控制器已存在

当你在场景中创建第一个 Cinemachine 相机时，Unity 会自动向 Unity 相机添加一个 Cinemachine 控制器（Cinemachine Brain），除非该 Unity 相机已包含此组件。如需确认，请按以下步骤操作：

1. 在层级窗口（Hierarchy）中，选中你的 Unity 相机——即带有 Camera 组件的游戏对象。

2. 在检视面板（Inspector）中，确认该游戏对象包含 [Cinemachine 控制器组件](CinemachineBrain.md)。


> [!注意]
> 如有需要，你可以像添加其他组件一样，手动将 Cinemachine 控制器组件添加到 Unity 相机游戏对象上，但请记住：场景中只能有一个 Unity 相机带有 Cinemachine 控制器。

> [!注意]
> 一旦添加了 Cinemachine 控制器，Unity 相机的变换（Transform）和镜头（Lens）设置会被锁定，无法在 Camera 组件的检视面板中直接修改。你只能通过修改 Cinemachine 相机的对应属性来调整这些相机参数。


## 调整 Cinemachine 相机的属性

1. 打开游戏视图（Game view）。  
   游戏视图会根据当前 Cinemachine 相机的设置，通过 Unity 相机的镜头显示场景内容。

2. 在层级窗口中，选中 Cinemachine 相机游戏对象。

3. 在检视面板中，根据需求调整以下属性以精确构图：
   * 在 **变换（Transform）** 组件中，调整 **位置（Position）** 和 **旋转（Rotation）**。
   * 在 **Cinemachine 相机（Cinemachine Camera）** 组件中，调整 **镜头（Lens）** 属性。


> [!注意]
> 此 Cinemachine 相机是你刚创建的唯一相机，且默认处于启用状态。因此，你会发现 Unity 相机会自动继承你对该 Cinemachine 相机所做的任何修改。


## 后续步骤

以下是你接下来可能需要执行的操作：

* [创建多台 Cinemachine 相机并管理它们之间的过渡效果](setup-multiple-cameras.md)。
* [创建带有程序化行为的 Cinemachine 相机：例如跟随角色的相机](setup-procedural-behavior.md)。
* [使用时间线（Timeline）管理 Cinemachine 相机镜头的编排序列](setup-timeline.md)。


## 其他资源

* [以第一人称视角预览和制作 Cinemachine 相机](preview-and-author-in-first-person.md)