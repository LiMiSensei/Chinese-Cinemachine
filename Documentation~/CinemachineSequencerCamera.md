# Cinemachine 序列相机（Cinemachine Sequencer Camera）

**Cinemachine 序列相机**组件用于执行其子级 Cinemachine 相机（CinemachineCamera）之间的一系列混合过渡（blend）或瞬间切换（cut）。

当序列相机被激活时，它会执行其指令列表：激活列表中的第一个子级 Cinemachine 相机，保持指定时长后，切换（cut）或混合过渡（blend）到下一个子级相机，依此类推。序列相机会保持最后一个子级 Cinemachine 相机的激活状态，直到 Cinemachine 控制器（Cinemachine Brain）或时间线（Timeline）将序列相机停用。如果启用了“循环（Loop）”标志，序列相机将回到列表中的第一个相机并继续执行序列。

**提示**：对于简单的自动序列，可使用序列相机替代[时间线（Timeline）](concept-timeline.md)，其使用方式与普通 Cinemachine 相机相同。


## 属性（Properties）：

| **属性** || **功能** |
|:---|:---|:---|
| **单独激活（Solo）** || 切换该 Cinemachine 相机是否暂时处于激活状态。使用此属性可在[游戏视图（Game view）](https://docs.unity3d.com/Manual/GameView.html)中获得即时视觉反馈，以便调整相机设置。 |
| **游戏视图辅助线（Game View Guides）** || 切换游戏视图中构图辅助线的可见性。当“跟踪目标（Tracking Target）”指定了某个游戏对象，且该 Cinemachine 相机具有屏幕构图行为（如位置构图器或旋转构图器）时，这些辅助线会生效。此设置对所有 Cinemachine 相机通用。 |
| **运行时保存（Save During Play）** || 勾选后，可[在运行模式下应用修改](CinemachineSavingDuringPlay.md)。使用此功能可微调 Cinemachine 相机，无需记住需要复制粘贴哪些属性。此设置对所有 Cinemachine 相机通用。 |
| **自定义输出（Custom Output）** || 此设置控制 Cinemachine 控制器（CinemachineBrain）如何使用该 Cinemachine 相机的输出。启用后可使用优先级（Priorities）或自定义 Cinemachine 输出通道。 |
|| **通道（Channel）** | 控制哪个 Cinemachine 控制器会被该相机驱动。当场景中有多个 Cinemachine 控制器时（例如实现分屏效果时），需要设置此属性。 |
|| **优先级（Priority）** | 当不受时间线控制时，用于控制多个激活的 Cinemachine 相机中哪个应处于激活状态。默认优先级为 0，可通过此属性指定自定义优先级值——值越高，优先级越高，也允许使用负值。Cinemachine 控制器会从所有已激活且优先级等于或高于当前激活相机的 Cinemachine 相机中，选择下一个激活的相机。此属性在时间线中使用 Cinemachine 相机时无效。 |
| **待机更新（Standby Update）** || 控制当 Cinemachine 相机未处于激活状态时的更新频率。可通过此属性优化性能。 |
|  | **从不（Never）** | 仅当 Cinemachine 相机处于激活状态时才更新。如果要在镜头评估场景中使用该相机，不要设置此值。 |
|  | **始终（Always）** | 即使 Cinemachine 相机未处于激活状态，也每帧更新。 |
|  | **轮询（Round Robin）** | 偶尔更新 Cinemachine 相机，更新频率取决于处于待机状态的其他 Cinemachine 相机数量。 |
| **默认目标（Default Target）** || 若启用，当子级 Cinemachine 相机未指定自己的“跟踪目标”时，将使用此目标作为备用。 |
| **显示调试文本（Show Debug Text）** || 若启用，当前状态信息将显示在游戏视图中。 |
| **循环（Loop）** || 启用后，子级 Cinemachine 相机会无限循环切换，而不是停留在列表中的最后一个 Cinemachine 相机。 |
| **指令（Instructions）** || 指定序列中子级相机的指令集。 |