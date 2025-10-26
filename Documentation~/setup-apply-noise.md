# 应用噪声以模拟相机抖动

要为 Cinemachine 相机添加噪声行为，请按以下步骤操作：

1. 在“层级窗口（Hierarchy）”中，选中你的 Cinemachine 相机。

2. 在“检视面板（Inspector）”的 Cinemachine 相机组件中，选择“噪声（Noise）”，然后选择“基础多通道柏林噪声（Basic Multi Channel Perlin）”。

   此操作会为 Cinemachine 相机添加一个噪声行为组件。

3. 在“基础多通道柏林噪声（Basic Multi Channel Perlin）”组件的“噪声配置文件（Noise Profile）”选项下，选择一个现有的噪声配置文件资源，或[创建自定义配置文件](CinemachineNoiseProfiles.md)。

4. 使用“振幅增益（Amplitude Gain）”和“频率增益（Frequency Gain）”微调噪声效果。


噪声通常用于实现类似手持相机的持续抖动效果。若需实现突发抖动（例如响应爆炸等事件的抖动），建议使用“脉冲（Impulse）”功能[脉冲（Impulse）](CinemachineImpulse.md)，而非噪声功能。