# Cinemachine 快速入门

了解 Cinemachine 的工作原理所需的核心信息，并按照说明设置最基础的功能层级，以便在项目中开始使用 Cinemachine。


<!--- 导航 --->

<!--- 一旦我们整理出手册中以使用结果为导向的结构化内容部分，就可以在此处添加一个简短段落，其中至少包含一个链接，供有经验的用户跳过此部分。 --->

<!--- 示例：如果您已了解 Cinemachine 的工作原理，且希望获取实现特定目标的操作指南，或了解更高级的 Cinemachine 概念与工具，请点击此处。 --->


| 章节 | 说明 |
| :--- | :--- |
| [Cinemachine 核心概念](concept.md) | 了解 Cinemachine 的基础元素与核心功能。 |
| [搭建基础 Cinemachine 环境](setup-cinemachine-environment.md) | 为 Unity 项目配置搭建可正常运行的 Cinemachine 环境所需的最基础元素。 |
| [设置多台 Cinemachine 相机与过渡效果](setup-multiple-cameras.md) | 搭建包含多台 Cinemachine 相机的 Cinemachine 环境，并管理相机之间的过渡效果。 |
| [为 Cinemachine 相机添加程序化行为](setup-procedural-behavior.md) | 为 Cinemachine 相机配置位置与旋转控制、目标自动跟踪以及模拟真实相机的抖动效果。同时了解如何为 Cinemachine 相机添加扩展组件。 |
| [设置时间线（Timeline）与 Cinemachine 相机](setup-timeline.md) | 在 Cinemachine 环境中配置时间线，实现 Cinemachine 相机的编排与可预测的镜头序列制作。 |
| [使用便捷工具与快捷键](ui-ref.md) | 利用界面工具与快捷键，定位到符合需求的 Cinemachine 元素进行配置，提升配置操作的便捷性。 |


<!--- 以下内容待整理/重新分配 --->

<!---

创建具有戏剧效果或细腻感的镜头切换（cut）或混合过渡（blend），例如：

* 对于两个角色对话的过场动画，可使用三台 Cinemachine 相机：一台用于拍摄两个角色的中景镜头，另外两台分别用于拍摄每个角色的特写镜头。通过时间线（Timeline）实现音频与 Cinemachine 相机的同步。

* 复制一台已有的 Cinemachine 相机，使两台相机在场景中处于同一位置。为第二台相机修改视野（FOV）或构图设置。当玩家进入触发区域时，Cinemachine 会从第一台相机平滑过渡到第二台相机，以突出动作场景的变化。

--->