# Cinemachine 自动对焦

这个 Cinemachine 相机扩展用于控制相机的 focusDistance（对焦距离）属性。它可用于将焦点锁定在特定对象上，或者（在 HDRP 中）自动检测相机前方的物体并对其对焦。

对焦距离仅与物理相机相关，并且必须启用相应的处理才能使其产生可见效果。

截至本文撰写时，只有 HDRP 提供了开箱即用的处理方式来处理相机的对焦距离。在 HDRP 中：
1. 创建一个包含景深（Depth Of Field）覆盖的激活体积（Volume）。
1. 在景深覆盖中，启用对焦模式（Focus Mode）并将其设置为物理相机（Physical Camera）。
1. 在景深覆盖中，启用对焦距离模式（Focus Distance Mode）并设置为相机（Camera）。

通过这些设置，当相机处于该体积范围内时，Cinemachine 自动对焦所设置的对焦距离将产生可见效果。

![自动对焦体积示例](images/CinemachineAutoVocusVolume.png)


## 属性：

| **属性：** || **功能：** |
|:---|:---|:---|
| __对焦目标（FocusTarget）__ || 相机的对焦距离将设置为相机到所选目标的距离。对焦偏移（Focus Offset）字段会进一步调整该距离。 |
| | _无（None）_ | 对焦跟踪已禁用。 |
| | _看向目标（LookAtTarget）_ | 对焦偏移相对于 LookAt 目标。 |
| | _跟随目标（FollowTarget）_ | 对焦偏移相对于 Follow 目标。 |
| | _自定义目标（CustomTarget）_ | 对焦偏移相对于“自定义目标（CustomTarget）”字段中设置的自定义目标。 |
| | _相机（Camera）_ | 对焦偏移相对于相机。使用此设置可通过“对焦深度偏移（FocusDepthOffset）”直接设置对焦距离。 |
| | _屏幕中心（ScreenCenter）_ | 仅 HDRP 可用：焦点将位于屏幕中心深度缓冲区中的任何物体上。 |
| __自定义目标（Custom Target）__ || 当“对焦目标”设置为“自定义目标（CustomTarget）”时使用的目标。 |
| __对焦深度偏移（Focus Depth Offset）__ || 从对焦目标位置沿深度方向偏移最清晰点。 |
| __阻尼（Damping）__ || 该值大致对应对焦调整到新值所需的时间。 |
| __自动检测半径（Auto Detection Radius）__ || 仅 HDRP 可用：当“对焦目标”设置为“屏幕中心（ScreenCenter）”时，屏幕中心自动对焦传感器的半径。值为 1 时将覆盖整个屏幕。建议将此值设置得较小。默认值为 0.02。 |