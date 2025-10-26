# 示例与教程

除了本文档外，还提供了多个示例场景和视频教程，以演示如何在实际场景中使用 Cinemachine 的功能。


## 示例场景

Cinemachine 包包含示例场景，你可以[将其导入项目](samples-import.md)，以在不同的使用场景中了解 Cinemachine 的众多功能。


### 2D 示例

导入 2D 示例集后，以下场景将出现在 `Assets/Samples/Cinemachine/<版本号>/2D Samples` 文件夹中。

| 示例场景 | 用例和关键功能 |
| :--- | :--- |
| **2D 格斗游戏（2D Fighters）** | <ul> <li>设置可根据游戏中剩余玩家动态构图的相机。</li> <li>模拟沿预设路径移动的玩家。</li> </ul> |
| **2D 平台游戏（2D Platformer）** | <ul> <li>设置可根据玩家状态切换相机的自定义相机组。</li> <li>设置当玩家进入房间时，能完整呈现 boss 房间的相机。</li> <li>限制相机视野，防止其显示游戏地图外的内容。</li> </ul> |
| **相机磁铁（Camera Magnets）** | <ul> <li>设置“磁铁”，使其根据玩家的距离吸引相机的注意力。</li> <li>限制相机视野，防止其显示游戏地图外的内容。</li> </ul> |


### 3D 示例

导入 3D 示例集后，以下场景将出现在 `Assets/Samples/Cinemachine/<版本号>/3D Samples` 文件夹中。

| 示例场景 | 用例和关键功能 |
| :--- | :--- |
| **控制器更新模式（Brain Update Modes）** | <ul> <li>防止动画角色在相机视图中出现抖动。</li> <li>根据被拍摄角色的移动方式，理解 Cinemachine 控制器更新方法（Cinemachine Brain Update Method）的效果。</li> </ul> |
| **清晰视角（Clear Shot）** | <ul> <li>设置一组固定和移动的相机，从不同视角瞄准同一玩家。</li> <li>基于遮挡情况自动选择最佳镜头（当当前相机失去玩家视野时）。</li> </ul> |
| **过场动画（Cutscene）** | <ul> <li>在时间线（Timeline）中设置包含相机混合过渡和资源动画的过场动画。</li> <li>触发过场动画，使其从游戏相机淡入、播放，然后再淡回游戏相机。</li> </ul> |
| **提前看向目标的自定义混合（Early LookAt Custom Blend）** | <ul> <li>编写自定义相机混合算法。</li> <li>挂钩到 Cinemachine 的混合创建过程，以覆盖默认混合算法。</li> </ul> |
| **自由飞行（Fly around）** | <ul> <li>设置具有基本高度和速度控制的第一人称自由飞行相机。</li> </ul> |
| **自由视角去遮挡（FreeLook Deoccluder）** | <ul> <li>设置可处理墙壁遮挡、确保玩家始终在视野内的自由视角相机。</li> <li>设置场景和相机，将特定对象视为透明并在遮挡评估中忽略它们。</li> </ul> |
| **球面自由视角（FreeLook on Spherical Surface）** | <ul> <li>设置可自动重新定向、跟随能在任意表面行走的玩家的自由视角相机。</li> <li>设置相机以“延迟”或“主动”方式跟随角色。</li> </ul> |
| **脉冲波（Impulse Wave）** | <ul> <li>设置相机和场景中的对象，使其对脉冲波做出反应。</li> <li>从固定震源触发脉冲。</li> <li>当玩家跳跃时触发脉冲。</li> </ul> |
| **锁定目标（Lock-on Target）** | <ul> <li>设置简单的第三人称自由视角相机，使其看向玩家并可通过鼠标绕玩家旋转。</li> <li>设置当玩家进入触发区域时，看向玩家并锁定 boss 角色的相机。</li> </ul> |
| **混合相机（Mixing Camera）** | <ul> <li>设置可根据汽车速度持续混合多个相机的相机组。</li> <li>设置当汽车进入双坡道特技区域时激活的固定相机，从侧面拍摄汽车跳跃画面。</li> </ul> |
| **透视到正交的自定义混合（Perspective To Ortho Custom Blend）** | <ul> <li>编写自定义相机混合算法。</li> <li>挂钩到 Cinemachine 的混合创建过程，以覆盖默认混合算法。</li> <li>创建平滑的透视到正交视图混合过渡（Cinemachine 原生不支持此功能）。</li> </ul> |
| **传送门（Portals）** | <ul> <li>无缝传送玩家及其自由视角相机。</li> </ul> |
| **赛跑（Running Race）** | <ul> <li>设置一组清晰视角相机，每台相机跟随不同的跑步者。</li> <li>自定义清晰视角质量评估，确保赛跑领先者始终位于相机视图中心。</li> <li>设置可按需呈现所有跑步者的相机。</li> <li>模拟沿预设路径移动的玩家。</li> </ul> |
| **分屏赛车（Split Screen Car）** | <ul> <li>以分屏配置显示两辆赛车。</li> </ul> |
| **带瞄准模式的第三人称（Third Person With Aim Mode）** | <ul> <li>设置跟随玩家移动、跳跃和冲刺的相机。</li> <li>设置可根据玩家瞄准控制器移动和瞄准的专用相机，并使用动态十字准星。</li> <li>为相机添加噪声以模拟手持效果，并在玩家瞄准和发射投射物时忽略此效果以保持精度。</li> </ul> |
| **带跟拍跑的第三人称（Third Person With Roadie Run）** | <ul> <li>设置跟随玩家移动、跳跃、瞄准和发射投射物的相机，并使用动态十字准星。</li> <li>设置玩家发射投射物时的相机后坐力效果。</li> <li>设置当玩家冲刺时自动激活的专用跟拍跑相机，该相机具有增强的噪声效果且无十字准星。</li> </ul> |


### 输入系统示例

导入输入系统示例集后，以下场景将出现在 `Assets/Samples/Cinemachine/<版本号>/Input System Samples` 文件夹中。

| 示例场景 | 用例和关键功能 |
| :--- | :--- |
| **分屏多人游戏（Split Screen Multiplayer）** | <ul> <li>编写与输入系统包的 Player Input 组件交互的自定义 Cinemachine 输入处理器。</li> <li>在分屏中动态添加多名玩家，每位玩家都有自己的自由视角相机系统。</li> </ul> |


### 简单玩家控制器

上面列出的多个示例都使用了 Cinemachine 的[简单玩家控制器（Simple Player Controller）](SimplePlayerController.md)，这是一个基础但多功能的玩家控制器，你可以在自己的项目中使用。它是一组脚本，你可以通过组合和配置这些脚本来创建角色控制器，用于不同场景以实现多种角色移动类型。


## 教程

> [!注意]
> 本节链接的视频是使用旧版本的 Cinemachine 制作的。你可能会注意到界面和某些元素的命名有一些变化，但所有解释的概念和展示的功能仍然适用于最新版本的 Cinemachine。

Unity 官方 YouTube 频道提供了[“使用 Cinemachine”系列视频教程](https://www.youtube.com/playlist?list=PLX2vGYjWbI0TQpl4JdfEDNO1xK_I34y8P)。了解各种 Cinemachine 用例，并观看相应项目设置所产生的即时效果。

| 视频 | 说明 |
| :--- | :--- |
| [入门（Getting Started）](https://www.youtube.com/watch?v=x6Q5sKXjZOM&list=PLX2vGYjWbI0TQpl4JdfEDNO1xK_I34y8P) | 让相机聚焦于一个变换组件（Transform），并在其在场景中移动时跟随它。 |
| [跟踪与移动（Track & Dolly）](https://www.youtube.com/watch?v=q1fkx94vHtg&list=PLX2vGYjWbI0TQpl4JdfEDNO1xK_I34y8P) | 通过为 Cinemachine 相机设置移动路径来跟踪目标。 |
| [状态驱动相机（State Driven Cameras）](https://www.youtube.com/watch?v=2X00qXErxIM&list=PLX2vGYjWbI0TQpl4JdfEDNO1xK_I34y8P) | 将 Cinemachine 相机链接到动画状态，并在场景中进一步自定义其行为。 |
| [自由视角（Free Look）](https://www.youtube.com/watch?v=X33t13gOBFw&list=PLX2vGYjWbI0TQpl4JdfEDNO1xK_I34y8P) | 创建可接收玩家输入的轨道相机系统，并在不同轨道阶段保持对相机构图的控制。 |
| [清晰视角（Clear Shot）](https://www.youtube.com/watch?v=I9w-agFYZ3I&list=PLX2vGYjWbI0TQpl4JdfEDNO1xK_I34y8P) | 当被跟踪目标被遮挡时，在场景中的多个 Cinemachine 相机之间动态切换。 |
| [后期处理（Post Processing）](https://www.youtube.com/watch?v=jFqOEvrVZeE&list=PLX2vGYjWbI0TQpl4JdfEDNO1xK_I34y8P) | 通过添加后期处理栈，Cinemachine 允许在相机或场景的不同效果之间轻松混合过渡。 |
| [Cinemachine 2D](https://www.youtube.com/watch?v=mWqX8GxeCBk&list=PLX2vGYjWbI0TQpl4JdfEDNO1xK_I34y8P) | 在 2D 项目中，将 Cinemachine 构图工具与正交虚拟相机配合使用。 |