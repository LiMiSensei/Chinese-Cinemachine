# 后处理扩展（Post Processing Extension）

使用 Cinemachine 后处理[扩展](concept-procedural-motion.md#extensions)，可将 Postprocessing V2 配置文件附加到 Cinemachine 相机（CinemachineCamera）上。

**注意 1**：Unity 推荐使用 Postprocessing V2 而非 Postprocessing V1。

**注意 2**：在 HDRP 和 URP 7 及更高版本中，PostProcessing 包已被弃用，其功能由 HDRP 和 URP 原生实现。这种情况下，请参阅 **CinemachineVolumeSettings** 扩展。

Cinemachine 后处理扩展包含一个后处理配置文件（Post-Processing Profile）资源，当 Cinemachine 相机激活时，该配置文件会应用于相机。若该相机正在与另一台 Cinemachine 相机进行混合过渡，混合权重也会应用到后处理效果上。


在将后处理配置文件附加到 Cinemachine 相机之前，需要先设置项目以支持后处理功能。

为 Cinemachine 配置使用 Post Processing V2 的项目步骤：

1. [安装](https://docs.unity3d.com/Packages/com.unity.package-manager-ui@latest/index.html) Postprocessing V2 包。

2. 在[场景视图（Scene view）](https://docs.unity3d.com/Manual/UsingTheSceneView.html)中，选中带有 Cinemachine 控制器（Cinemachine Brain）的 Unity 相机。

3. [添加组件](https://docs.unity3d.com/Manual/UsingComponents.html) **Post-Process Layer**（后处理层）。这将使后处理配置文件能够对相机产生影响。


为 Cinemachine 相机添加后处理配置文件的步骤：

4. 在[场景视图（Scene view）](https://docs.unity3d.com/Manual/UsingTheSceneView.html)或[层级窗口（Hierarchy）](https://docs.unity3d.com/Manual/Hierarchy.html)中，选中你的 Cinemachine 相机。

5. 在[检视面板（Inspector）](https://docs.unity3d.com/Manual/UsingTheInspector.html)中，选择 **Add Extension > CinemachinePostProcessing**（添加扩展 > Cinemachine 后处理），然后配置该配置文件资源，使其包含你希望该 Cinemachine 相机激活时应用的效果。


> [!注意]
> 在某些情况下，尤其是在与空配置文件进行混合过渡时，效果可能会突然变化或出现跳变。若出现这种情况，最佳解决方案是通过添加具有默认设置的效果，避免与空配置文件进行混合过渡。如果不可行，你可以在项目的脚本定义中添加 `CINEMACHINE_TRANSPARENT_POST_PROCESSING_BLENDS`。但这样做的副作用是，后处理混合过渡的中心会更透明，可能会显示出其背后的全局效果。


## 属性（Properties）：

| **属性** || **功能** |
|:---||:---|
| **配置文件（Profile）** || 当该 Cinemachine 相机激活时，要应用的后处理配置文件。 |
| **焦点跟踪目标（Focus Tracks Target）** || 此属性已过时，请使用 **焦点跟踪（Focus Tracking）**。 |
| **焦点跟踪（Focus Tracking）** || 若配置文件有相应的覆盖设置，会将基础对焦距离设为所选目标到相机的距离。**焦点偏移（Focus Offset）** 字段会进一步调整该距离。 |
|| **无（None）** | 不进行焦点跟踪。 |
|| **看向目标（Look At Target）** | 焦点偏移相对于“看向目标（LookAt target）”。 |
|| **跟随目标（Follow Target）** | 焦点偏移相对于“跟随目标（Follow target）”。 |
|| **自定义目标（Custom Target）** | 焦点偏移相对于“自定义目标（Custom target）”。 |
|| **相机（Camera）** | 焦点偏移相对于相机。 |
| **焦点目标（Focus Target）** || 当 **焦点跟踪（Focus Tracks Target）** 设为“自定义目标（Custom Target）”时，所使用的目标。 |
| **焦点偏移（Focus Offset）** || 当 **焦点跟踪（Focus Tracking）** 未设为“无（None）”时使用。用于调整离焦目标位置的最清晰点距离。 |
| **权重（Weight）** || 当相机完全混合到位时，将创建的动态体积的权重。该权重会随相机的混合过渡在 0 和设定值之间变化。 |