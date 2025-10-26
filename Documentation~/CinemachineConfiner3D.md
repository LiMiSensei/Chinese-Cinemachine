# Cinemachine 3D 限制器（Cinemachine Confiner 3D）

使用 **Cinemachine 3D 限制器**这一[扩展组件](concept-procedural-motion.md#extensions)，可将相机位置限制在指定的体积范围内。

在 3D 场景中，相机会被限制在设定的体积区域内移动。

| **属性** || **功能** |
|:---|:---|:---|
| **边界体积（Bounding Volume）** || 用于限制相机的 3D 体积区域。 |
| **减速距离（Slowing Distance）** || 边界体积边缘处减速区域的大小。当相机向边缘移动且与边缘的距离处于此范围内时，相机会逐渐减速，直至抵达边缘后停止。 |