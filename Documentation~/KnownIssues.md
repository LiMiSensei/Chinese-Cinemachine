# 已知问题

## 累积缓冲区投影矩阵
如果启用了累积缓冲区的“抗锯齿（Anti-aliasing）”选项，且场景中包含 Cinemachine 相机切换（cut），则切换后相机的视野（FOV）会出现异常。

**解决方法**：在每帧中，当 CinemachineBrain 修改相机后，重置投影矩阵。

```csharp
public class FixProjection : MonoBehaviour
{
    void LateUpdate()
    {
        Camera.main.ResetProjectionMatrix();
    }
}
```