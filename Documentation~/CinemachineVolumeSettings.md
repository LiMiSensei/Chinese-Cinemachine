# Cinemachine 体积设置扩展（Cinemachine Volume Settings Extension）

使用 Cinemachine 体积设置[扩展组件](concept-procedural-motion.md#extensions)，可将 HDRP（高清渲染管线）/URP（通用渲染管线）的 VolumeSettings（体积设置）配置文件附加到 Cinemachine 相机（CinemachineCamera）上。

Cinemachine 体积设置扩展组件会持有一个 Volume Settings 配置文件资源，当对应的 Cinemachine 相机激活时，该配置文件会应用到相机上。若此相机正与另一台 Cinemachine 相机进行混合过渡（blending），则混合权重也会同步应用到体积设置的效果上。


## 为 Cinemachine 相机添加 Volume Settings 配置文件
1. 在[场景视图（Scene view）](https://docs.unity3d.com/Manual/UsingTheSceneView.html)或[层级窗口（Hierarchy）](https://docs.unity3d.com/Manual/Hierarchy.html)中，选中你的 Cinemachine 相机。
2. 在[检视面板（Inspector）](https://docs.unity3d.com/Manual/UsingTheInspector.html)中，选择 **添加扩展（Add Extension）> CinemachineVolumeSettings**，然后配置该配置文件资源（Profile asset），设置你希望此 Cinemachine 相机激活时生效的效果。


> [!注意]
> 在某些情况下，尤其是在与空白配置文件（empty profile）之间进行混合过渡时，效果可能会出现突然变化或跳变（pop）。若遇到此问题，最佳解决方案是：通过添加带有默认设置的效果，避免与空白配置文件进行混合过渡。若此方法不可行，可在项目的脚本定义符号（scripting defines）中添加 `CINEMACHINE_TRANSPARENT_POST_PROCESSING_BLENDS`。但需注意，此操作会产生副作用——后期处理混合效果的中心区域会变得更透明，可能会显示出其后方的全局效果。


## 属性（Properties）：

| **属性** || **功能** |
|:---||:---|
| **配置文件（Profile）** || 当此 Cinemachine 相机激活时，需要启用的 Volume Settings 配置文件。 |
| **焦点跟踪目标（Focus Tracks Target）** || 此属性已过时（obsolete），请使用**焦点跟踪（Focus Tracking）** 属性。 |
| **焦点跟踪（Focus Tracking）** || 若配置文件包含相应的覆盖设置（override），会将基础对焦距离（base focus distance）设置为所选目标到相机的距离。随后，**焦点偏移（Focus Offset）** 字段会对此距离进行调整。 |
|| **无（None）** | 不启用焦点跟踪 |
|| **看向目标（Look At Target）** | 焦点偏移相对于“看向目标（LookAt target）”计算 |
|| **跟随目标（Follow Target）** | 焦点偏移相对于“跟随目标（Follow target）”计算 |
|| **自定义目标（Custom Target）** | 焦点偏移相对于“自定义目标（Custom target）”计算 |
|| **相机（Camera）** | 焦点偏移相对于相机自身计算 |
| **焦点目标（Focus Target）** || 当**焦点跟踪目标（Focus Tracks Target）** 设为“自定义目标（Custom Target）”时，所使用的目标对象。 |
| **焦点偏移（Focus Offset）** || 当**焦点跟踪（Focus Tracking）** 未设为“无（None）”时生效。用于调整焦点的最清晰点（sharpest point）与焦点目标位置之间的偏移量。 |
| **权重（Weight）** || 当相机完全完成混合过渡（fully blended in）时，将创建的动态体积（dynamic volume）的权重。此权重会随相机的混合过程在 0 到设定值之间过渡。 |