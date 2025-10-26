# Cinemachine 基础多通道柏林噪声组件（Cinemachine Basic Multi Channel Perlin component）

在 Cinemachine 相机游戏对象（Cinemachine Camera GameObject）中添加“基础多通道柏林噪声组件”，可通过柏林噪声（Perlin noise）运动模拟相机抖动效果。柏林噪声是一种生成具有自然运动特性的伪随机运动的技术。

“基础多通道柏林噪声组件”会应用一个[噪声配置资源（Noise Profile Asset）](CinemachineNoiseProfiles.md)，该资源定义了随时间变化的噪声运动规律。Cinemachine 内置了若干噪声配置资源，你可以对这些资源进行编辑，也可以创建自定义的噪声配置资源。

<!--- 请更新下方截图，确保截图展示该组件的实际外观 --->

<!---
![选择“基础多通道柏林噪声组件”为相机添加噪声效果](images/CinemachineBasicMultiChannelPerlin.png)
--->

## 属性（Properties）

| **属性** | **功能** |
|:---|:---|
| **噪声配置（Noise Profile）** | 要使用的噪声配置资源。 |
| **振幅增益（Amplitude Gain）** | 应用于噪声配置中定义的振幅的增益系数。设为 1 时，将使用噪声配置中定义的原始振幅；设为 0 时，噪声效果将被静音。提示：可对该属性设置动画，实现噪声效果的渐强或渐弱。 |
| **频率增益（Frequency Gain）** | 应用于噪声配置中定义的频率的系数。设为 1 时，将使用噪声配置中定义的原始频率；增大该值可使相机抖动更频繁。提示：可对该属性设置动画，实现噪声效果的渐强或渐弱。 |
| **轴心偏移（Pivot Offset）** | 当相机旋转时，应用旋转噪声时会将相机轴心按指定的 X、Y、Z 轴距离进行偏移。此设置可生成与旋转噪声对应的位置变化，让抖动效果更自然。 |