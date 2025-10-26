# 为 Cinemachine 相机添加程序化行为
为 Cinemachine 相机配置[程序化行为](concept-procedural-motion.md)，操作步骤如下：
- 添加用于控制相机位置与旋转的行为
- 指定一个游戏对象（GameObject）作为跟踪目标，实现自动跟随与瞄准
- 添加默认噪声效果，模拟真实物理相机的抖动
- 了解如何通过扩展（Extension）实现高级相机行为


> [!NOTE]
> 执行此操作需提前完成以下准备：
> * 已创建至少一个[被动 Cinemachine 相机](setup-cinemachine-environment.md)
> * 场景中已包含可作为 Cinemachine 相机跟踪目标的游戏对象


## 一、添加位置控制与旋转控制行为
1. 在层级面板（Hierarchy）中，选中 Cinemachine 相机对应的游戏对象。
2. 在检查器面板（Inspector）的**Cinemachine Camera**组件中，将**Position Control**（位置控制）属性设为**Follow**（跟随）。  
   Unity 会自动为该游戏对象添加[Cinemachine Follow](CinemachineFollow.md)组件。
3. 仍在**Cinemachine Camera**组件中，将**Rotation Control**（旋转控制）属性设为**Rotation Composer**（旋转合成器）。  
   Unity 会自动为该游戏对象添加[Cinemachine Rotation Composer](CinemachineRotationComposer.md)组件。


> [!NOTE]
> 直接通过编辑器菜单创建“跟随相机”（**GameObject** > **Cinemachine** > **Targeted Cameras** > **Follow Camera**），也能得到相同结果。本文此步骤的目的是演示如何为已有的 Cinemachine 相机添加行为。

若需查看所有可用的“位置控制”与“旋转控制”行为及详细说明，可参考[Cinemachine Camera 组件参考文档](CinemachineCamera.md)。


> [!WARNING]
> 一个 Cinemachine 相机游戏对象，同一时间只能选择一种“位置控制”行为和一种“旋转控制”行为。若先编辑某行为组件的属性，再在 Cinemachine Camera 组件中切换到其他行为，之前的编辑内容会丢失。


## 二、指定跟踪目标游戏对象
上一步选择的行为需要一个跟踪目标才能生效，具体操作如下：
1. 在检查器面板的**Cinemachine Camera**组件中，设置**Tracking Target**（跟踪目标）属性，指定需要跟踪的游戏对象。
2. 在场景中移动该跟踪目标游戏对象，测试 Cinemachine 相机的行为。  
   此时，Cinemachine 相机会根据“跟随行为”（Follow）始终保持与目标的相对位置，并根据“旋转合成器行为”（Rotation Composer）旋转镜头，确保目标始终在视野内。


## 三、添加相机抖动噪声
若需模拟真实物理相机的抖动效果，可添加噪声，操作步骤如下：
1. 在检查器面板的**Cinemachine Camera**组件中，将**Noise**（噪声）属性设为**Basic Multi Channel Perlin**（基础多通道柏林噪声）。  
   Unity 会自动为该游戏对象添加[Cinemachine Basic Multi Channel Perlin](CinemachineBasicMultiChannelPerlin.md)组件。
2. 在“Cinemachine Basic Multi Channel Perlin”组件中，点击**Noise Profile**（噪声配置文件）右侧的配置按钮。
3. 在弹出的面板中，选择**Presets**（预设）下的任意一个[噪声设置资源](CinemachineNoiseProfiles.md)。
4. 进入播放模式（Play mode）查看所选噪声配置对相机的影响，之后退出播放模式即可。


> [!WARNING]
> 若先编辑“Cinemachine Basic Multi Channel Perlin”组件的属性，再在 Cinemachine Camera 组件中切换“噪声”类型，之前的编辑内容会丢失。

若需进一步调整噪声行为，可参考[Cinemachine Basic Multi Channel Perlin 组件参考文档](CinemachineBasicMultiChannelPerlin.md)和[噪声设置资源参考文档](CinemachineNoiseProfiles.md)。


## 四、添加 Cinemachine 相机扩展
若需实现特定或高级的相机行为，可给 Cinemachine 相机添加扩展（Extension），操作如下：
1. 在检查器面板的**Cinemachine Camera**组件中，点击**Add Extension**（添加扩展）右侧的**(select)**（请选择）。
2. 在弹出的列表中选择所需的扩展类型。  
   Unity 会自动为该游戏对象添加对应的扩展组件。


> [!NOTE]
> 同一个 Cinemachine 相机游戏对象可添加多个扩展。若需移除扩展，操作方式与移除其他[游戏对象组件](https://docs.unity3d.com/Manual/UsingComponents.html)一致。

若需查看所有可用的 Cinemachine 相机扩展及详细说明，可参考[参考文档](Reference.md)。


## 后续操作建议
完成上述配置后，可尝试以下操作：
- [创建多个 Cinemachine 相机，并管理相机间的切换过渡](setup-multiple-cameras.md)
- [通过 Timeline 管理 Cinemachine 相机的编排镜头序列](setup-timeline.md)

