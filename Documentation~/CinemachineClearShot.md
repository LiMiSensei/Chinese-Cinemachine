# Cinemachine 清晰视角相机（Cinemachine Clear Shot Camera）

**清晰视角相机（ClearShot Camera）** 组件会从其子级 Cinemachine 相机中选择能呈现目标最佳视角的相机。使用清晰清晰视角（Clear Shot）"可以为场景设置复杂的多相机覆盖，确保目标始终处于清晰可见。

这是一个非常强大的工具。带有[Cinemachine 去遮挡器（Cinemachine Deoccluder）](CinemachineDeoccluder.md)或其他视角质量评估扩展的子级 Cinemachine 相机会分析视角的目标遮挡情况、最佳目标距离等。清晰视角相机利用这些信息选择最佳的子级相机并激活。

**提示**：若要为所有子级 Cinemachine 相机使用同一个[Cinemachine 去遮挡器](CinemachineDeoccluder.md)，可将 Cinemachine 去遮挡器扩展添加到清晰视角相机的游戏对象上，而非每个子级 Cinemachine 相机。此去遮挡器扩展将应用于所有子级相机，效果等同于每个子级相机都自带该去遮挡器。

如果多个子级相机的视角质量相同，清晰视角相机会选择优先级最高的那个。

你也可以在清晰视角相机的子级之间定义自定义混合效果。


## 属性（Properties）：

| **属性** || **功能** |
|:---|:---|:---|
| **单独激活（Solo）** || 切换该 Cinemachine 相机是否临时生效。使用此属性可在[游戏视图（Game view）](https://docs.unity3d.com/Manual/GameView.html)中立即获得视觉反馈，以便调整该相机。 |
| **游戏视图辅助线（Game View Guides）** || 切换游戏视图中构图辅助线的显示状态。当“跟踪目标（Tracking Target）”指定了游戏对象，且该 Cinemachine 相机具有屏幕构图行为（如“位置合成器”或“旋转合成器”）时，这些辅助线可用。所有 Cinemachine 相机共享此设置。 |
| **运行时保存（Save During Play）** || 勾选后可[在运行模式下应用更改](CinemachineSavingDuringPlay.md)。使用此功能可微调 Cinemachine 相机，无需记忆需复制粘贴的属性。所有 Cinemachine 相机共享此设置。 |
| **自定义输出（Custom Output）** || 此设置控制 Cinemachine 控制器（CinemachineBrain）如何使用该 Cinemachine 相机的输出。启用此选项可使用优先级（Priority）或自定义 Cinemachine 输出通道。 |
|| **通道（Channel）** | 控制该相机驱动哪个 Cinemachine 控制器。当场景中存在多个 Cinemachine 控制器时（例如实现分屏功能时），此属性必不可少。 |
|| **优先级（Priority）** | 在非时间线（Timeline）控制的情况下，用于控制多个激活的 Cinemachine 相机中哪个应生效。默认优先级为 0，可通过此属性指定自定义优先级值，数值越大表示优先级越高，也允许使用负值。Cinemachine 控制器会从所有已激活且优先级大于或等于当前生效相机优先级的 Cinemachine 相机中，选择下一个生效的相机。在时间线中使用该相机时，此属性无效。 |
| **待机更新（Standby Update）** || 控制该 Cinemachine 相机未生效（非 Live 状态）时的更新频率，可通过此属性进行性能优化。 |
|  | **从不（Never）** | 仅当该相机生效时才更新。若在镜头评估场景中使用该相机，请勿设置此值。 |
|  | **始终（Always）** | 即使该相机未生效，也会每帧更新。 |
|  | **循环轮询（Round Robin）** | 偶尔更新该相机，更新频率取决于处于待机状态的其他 Cinemachine 相机数量。 |
| **默认目标（Default Target）** || 若启用，当子级 Cinemachine 相机未指定自己的“跟踪目标（Tracking Target）”时，将使用此目标作为备用。 |
| **显示调试文本（Show Debug Text）** || 若启用，当前状态信息将显示在游戏视图中。 |
| **延迟激活（Activate After）** || 等待指定秒数后再激活新的子级相机。 |
| **最短持续时间（Min Duration）** || 激活的相机必须至少保持激活状态指定秒数，除非有更高优先级的相机需要激活。 |
| **随机选择（Randomize Choice）** || 若勾选，当多个相机的视角质量相同时，将随机选择一个；若未勾选，则根据子级 Cinemachine 相机的排列顺序及其优先级进行选择。 |
| **默认混合（Default Blend）** || 当未明确定义两个 Cinemachine 相机之间的混合方式时使用的默认混合效果。 |