using UnityEngine;
using System;

namespace Unity.Cinemachine
{
    /// <summary>
    /// Describes the FOV and clip planes for a camera.  This generally mirrors the Unity Camera's
    /// lens settings, and will be used to drive the Unity camera when the vcam is active.
    /// </summary>
    [Serializable]
    public struct LensSettings
    {
        /// <summary>
        /// 这是摄像机的垂直视野角度（单位：度）。显示通常为垂直角度，除非关联的摄像机
        /// 将其视野轴向设置为水平，此时将显示水平角度。内部处理时始终使用垂直角度。
        /// 对于电影制作人员，在超35mm传感器上使用50mm镜头相当于19.6度的视野。
        /// </summary>
        [Tooltip("此设置控制镜头的视野或焦距，具体取决于"
            + "摄像机模式是物理模式还是非物理模式。视野可以是水平或垂直，"
            + "具体取决于摄像机组件中的设置。")]
        public float FieldOfView;

        /// <summary>
        /// 使用正交摄像机时，此值定义摄像机视图的半高度（世界坐标单位）。
        /// </summary>
        [Tooltip("使用正交摄像机时，此值定义摄像机视图的半高度（世界坐标单位）。")]
        public float OrthographicSize;

        /// <summary>
        /// 此镜头设置的近裁剪平面
        /// </summary>
        [Tooltip("此值定义摄像机视锥体可渲染范围的近端区域。"
            + "提高此值将阻止游戏绘制靠近摄像机的物体，这在某些情况下很有用。"
            + "较大的值也会提高阴影分辨率。")]
        public float NearClipPlane;

        /// <summary>
        /// 此镜头设置的远裁剪平面
        /// </summary>
        [Tooltip("此值定义摄像机视锥体可渲染范围的远端区域。通常"
            + "您希望将此值设置为尽可能低，同时不切断所需的远处物体")]
        public float FarClipPlane;

        /// <summary>
        /// 应用于摄像机的倾斜角度（荷兰角）。单位：度
        /// </summary>
        [Tooltip("摄像机的Z轴旋转或倾斜角度，单位：度。")]
        public float Dutch;

        /// <summary>
        /// 此枚举控制摄像机设置的驱动方式。某些设置
        /// 可以根据这些值从主摄像机拉取或推送到主摄像机。
        /// </summary>
        public enum OverrideModes
        {
            /// <summary> 透视/正交、是否为物理摄像机
            /// 不会在Unity摄像机中更改。这是默认设置。</summary>
            None = 0,
            /// <summary>正交投影模式将被推送到Unity摄像机</summary>
            Orthographic,
            /// <summary>透视投影模式将被推送到Unity摄像机</summary>
            Perspective,
            /// <summary>物理建模的透视投影类型将被推送
            /// 到Unity摄像机</summary>
            Physical
        }

        /// <summary>
        /// 允许您选择不同的摄像机模式，当Cinemachine激活此虚拟摄像机时
        /// 将应用到摄像机组件。
        /// </summary>
        [Tooltip("允许您选择不同的摄像机模式，当Cinemachine激活此虚拟摄像机时"
            + "将应用到摄像机组件。")]
        public OverrideModes ModeOverride;

        /// <summary>这些设置仅在启用物理摄像机时使用。</summary>
        [Serializable]
        [Tooltip("这些设置仅在启用物理摄像机时使用")]
        public struct PhysicalSettings
        {
            /// <summary>当宽高比不同时，图像如何适配传感器</summary>
            [Tooltip("当宽高比不同时，图像如何适配传感器")]
            public Camera.GateFitMode GateFit;

            /// <summary>这是图像传感器的实际尺寸（单位：毫米）。</summary>
            [SensorSizeProperty]
            [Tooltip("这是图像传感器的实际尺寸（单位：毫米）")]
            public Vector2 SensorSize;

            /// <summary>相对于胶片背板的门位置</summary>
            [Tooltip("相对于胶片背板的门位置")]
            public Vector2 LensShift;

            /// <summary>距离摄像机镜头焦距最清晰的距离。
            /// 如果将FocusDistanceMode设置为Camera，景深体积覆盖将使用此值</summary>
            [Tooltip("距离摄像机镜头焦距最清晰的距离。景深体积"
                + "覆盖将使用此值，如果将FocusDistanceMode设置为Camera")]
            public float FocusDistance;

            /// <summary>传感器灵敏度（ISO）</summary>
            [Tooltip("传感器灵敏度（ISO）")]
            public int Iso;

            /// <summary>曝光时间，单位：秒</summary>
            [Tooltip("曝光时间，单位：秒")]
            public float ShutterSpeed;

            /// <summary>光圈数值，单位：f-stop</summary>
            [Tooltip("光圈数值，单位：f-stop")]
            [Range(Camera.kMinAperture, Camera.kMaxAperture)]
            public float Aperture;

            /// <summary>光圈叶片数量</summary>
            [Tooltip("光圈叶片数量")]
            [Range(Camera.kMinBladeCount, Camera.kMaxBladeCount)]
            public int BladeCount;

            /// <summary>将光圈范围映射到叶片曲率</summary>
            [Tooltip("将光圈范围映射到叶片曲率")]
            [MinMaxRangeSlider(Camera.kMinAperture, Camera.kMaxAperture)]
            public Vector2 Curvature;

            /// <summary>散景中"猫眼"效果的强度（光学暗角）</summary>
            [Tooltip("散景中\"猫眼\"效果的强度（光学暗角）")]
            [Range(0, 1)]
            public float BarrelClipping;

            /// <summary>拉伸传感器以模拟变形宽银幕效果。正值使
            /// 摄像机垂直扭曲，负值使摄像机水平扭曲</summary>
            [Tooltip("拉伸传感器以模拟变形宽银幕效果。正值使"
                + "摄像机垂直扭曲，负值使摄像机水平扭曲")]
            [Range(-1, 1)]
            public float Anamorphism;

        }

        /// <summary>
        /// The physical settings of the lens, valid only when camera is set to Physical mode.
        /// </summary>
        public PhysicalSettings PhysicalProperties;

        bool m_OrthoFromCamera;
        bool m_PhysicalFromCamera;
        float m_AspectFromCamera;

#if UNITY_EDITOR
        // Needed for knowing how to display FOV (horizontal or vertical)
        // This should really be a global Unity setting, but for now there is no better way than this!
        Camera m_SourceCamera;
        internal bool UseHorizontalFOV
        {
            get
            {
                if (m_SourceCamera == null)
                    return false;
                var p = new UnityEditor.SerializedObject(m_SourceCamera).FindProperty("m_FOVAxisMode");
                return p != null && p.intValue == (int)Camera.FieldOfViewAxis.Horizontal;
            }
        }
#endif

        /// <summary>
        /// This is set every frame by the virtual camera, based on the value found in the
        /// currently associated Unity camera.
        /// Do not set this property.  Instead, use the ModeOverride field to set orthographic mode.
        /// </summary>
        public bool Orthographic => ModeOverride == OverrideModes.Orthographic
            || (ModeOverride == OverrideModes.None && m_OrthoFromCamera);

        /// <summary>
        /// This property will be true if the camera mode is set, either directly or
        /// indirectly, to Physical Camera
        /// Do not set this property.  Instead, use the ModeOverride field to set physical mode.
        /// </summary>
        public bool IsPhysicalCamera => ModeOverride == OverrideModes.Physical
            || (ModeOverride == OverrideModes.None && m_PhysicalFromCamera);

        /// <summary>
        /// For physical cameras, this is the Sensor aspect.
        /// For nonphysical cameras, this is the screen aspect pulled from the camera, if any.
        /// </summary>
        public float Aspect => IsPhysicalCamera
            ? PhysicalProperties.SensorSize.x / PhysicalProperties.SensorSize.y : m_AspectFromCamera;

        /// <summary>Default Lens Settings</summary>
        public static LensSettings Default => new ()
        {
            FieldOfView = 40f,
            OrthographicSize = 10f,
            NearClipPlane = 0.1f,
            FarClipPlane = 5000f,
            Dutch = 0,
            ModeOverride = OverrideModes.None,

            PhysicalProperties = new ()
            {
                SensorSize = new Vector2(21.946f, 16.002f),
                GateFit = Camera.GateFitMode.Horizontal,
                FocusDistance = 10,
                LensShift = Vector2.zero,
                Iso = 200,
                ShutterSpeed = 0.005f,
                Aperture = 16,
                BladeCount = 5,
                Curvature = new Vector2(2, 11),
                BarrelClipping = 0.25f,
                Anamorphism = 0,
            },
            m_AspectFromCamera = 1
        };

        /// <summary>
        /// Creates a new LensSettings, copying the values from the
        /// supplied Camera
        /// </summary>
        /// <param name="fromCamera">The Camera from which the FoV, near
        /// and far clip planes will be copied.</param>
        /// <returns>The LensSettings as extracted from the supplied Camera</returns>
        public static LensSettings FromCamera(Camera fromCamera)
        {
            LensSettings lens = Default;
            if (fromCamera != null)
            {
                lens.PullInheritedPropertiesFromCamera(fromCamera);

                lens.FieldOfView = fromCamera.fieldOfView;
                lens.OrthographicSize = fromCamera.orthographicSize;
                lens.NearClipPlane = fromCamera.nearClipPlane;
                lens.FarClipPlane = fromCamera.farClipPlane;

                if (lens.IsPhysicalCamera)
                {
                    lens.FieldOfView = Camera.FocalLengthToFieldOfView(
                        Mathf.Max(0.01f, fromCamera.focalLength), fromCamera.sensorSize.y);
                    lens.PhysicalProperties.SensorSize = fromCamera.sensorSize;
                    lens.PhysicalProperties.LensShift = fromCamera.lensShift;
                    lens.PhysicalProperties.GateFit = fromCamera.gateFit;
                    lens.PhysicalProperties.FocusDistance = fromCamera.focusDistance;
                    lens.PhysicalProperties.Iso = fromCamera.iso;
                    lens.PhysicalProperties.ShutterSpeed = fromCamera.shutterSpeed;
                    lens.PhysicalProperties.Aperture = fromCamera.aperture;
                    lens.PhysicalProperties.BladeCount = fromCamera.bladeCount;
                    lens.PhysicalProperties.Curvature = fromCamera.curvature;
                    lens.PhysicalProperties.BarrelClipping = fromCamera.barrelClipping;
                    lens.PhysicalProperties.Anamorphism = fromCamera.anamorphism;
                }
            }
            return lens;
        }

        /// <summary>
        /// In the event that there is no camera mode override, camera mode is driven
        /// by the Camera's state.
        /// </summary>
        /// <param name="camera">The Camera from which we will take the info</param>
        public void PullInheritedPropertiesFromCamera(Camera camera)
        {
            if (ModeOverride == OverrideModes.None)
            {
                m_OrthoFromCamera = camera.orthographic;
                m_PhysicalFromCamera = camera.usePhysicalProperties;
            }
            m_AspectFromCamera = camera.aspect;
#if UNITY_EDITOR
            m_SourceCamera = camera; // hack because of missing Unity API to get horizontal or vertical fov mode
#endif
        }

        /// <summary>
        /// Copy the properties controlled by camera mode.  If ModeOverride is None, then
        /// some internal state information must be transferred.
        /// </summary>
        /// <param name="fromLens">The LensSettings from which we will take the info</param>
        public void CopyCameraMode(ref LensSettings fromLens)
        {
            ModeOverride = fromLens.ModeOverride;
            if (ModeOverride == OverrideModes.None)
            {
                m_OrthoFromCamera = fromLens.Orthographic;
                m_PhysicalFromCamera = fromLens.IsPhysicalCamera;
            }
            m_AspectFromCamera = fromLens.m_AspectFromCamera;
        }

        /// <summary>
        /// Linearly blends the fields of two LensSettings and returns the result
        /// </summary>
        /// <param name="lensA">The LensSettings to blend from</param>
        /// <param name="lensB">The LensSettings to blend to</param>
        /// <param name="t">The interpolation value. Internally clamped to the range [0,1]</param>
        /// <returns>Interpolated settings</returns>
        public static LensSettings Lerp(LensSettings lensA, LensSettings lensB, float t)
        {
            t = Mathf.Clamp01(t);
            // non-lerpable settings taken care of here
            if (t < 0.5f)
            {
                var blendedLens = lensA;
                blendedLens.Lerp(lensB, t);
                return blendedLens;
            }
            else
            {
                var blendedLens = lensB;
                blendedLens.Lerp(lensA, 1 - t);
                return blendedLens;
            }
        }

        /// <summary>
        /// Lerp the interpolatable values. Values that can't be interpolated remain intact.
        /// </summary>
        /// <param name="lensB">The lens containing the values to combine with this one</param>
        /// <param name="t">The weight of LensB's values.</param>
        public void Lerp(in LensSettings lensB, float t)
        {
            FarClipPlane = Mathf.Lerp(FarClipPlane, lensB.FarClipPlane, t);
            NearClipPlane = Mathf.Lerp(NearClipPlane, lensB.NearClipPlane, t);
            FieldOfView = Mathf.Lerp(FieldOfView, lensB.FieldOfView, t);
            OrthographicSize = Mathf.Lerp(OrthographicSize, lensB.OrthographicSize, t);
            Dutch = Mathf.Lerp(Dutch, lensB.Dutch, t);
            PhysicalProperties.SensorSize = Vector2.Lerp(PhysicalProperties.SensorSize, lensB.PhysicalProperties.SensorSize, t);
            PhysicalProperties.LensShift = Vector2.Lerp(PhysicalProperties.LensShift, lensB.PhysicalProperties.LensShift, t);
            PhysicalProperties.FocusDistance = Mathf.Lerp(PhysicalProperties.FocusDistance, lensB.PhysicalProperties.FocusDistance, t);
            PhysicalProperties.Iso = Mathf.RoundToInt(Mathf.Lerp((float)PhysicalProperties.Iso, (float)lensB.PhysicalProperties.Iso, t));
            PhysicalProperties.ShutterSpeed = Mathf.Lerp(PhysicalProperties.ShutterSpeed, lensB.PhysicalProperties.ShutterSpeed, t);
            PhysicalProperties.Aperture = Mathf.Lerp(PhysicalProperties.Aperture, lensB.PhysicalProperties.Aperture, t);
            PhysicalProperties.BladeCount = Mathf.RoundToInt(Mathf.Lerp(PhysicalProperties.BladeCount, lensB.PhysicalProperties.BladeCount, t));;
            PhysicalProperties.Curvature = Vector2.Lerp(PhysicalProperties.Curvature, lensB.PhysicalProperties.Curvature, t);
            PhysicalProperties.BarrelClipping = Mathf.Lerp(PhysicalProperties.BarrelClipping, lensB.PhysicalProperties.BarrelClipping, t);
            PhysicalProperties.Anamorphism = Mathf.Lerp(PhysicalProperties.Anamorphism, lensB.PhysicalProperties.Anamorphism, t);
        }

        /// <summary>Make sure lens settings are sane.  Call this from OnValidate().</summary>
        public void Validate()
        {
            FarClipPlane = Mathf.Max(FarClipPlane, NearClipPlane + 0.001f);
            FieldOfView = Mathf.Clamp(FieldOfView, 0.01f, 179f);
            PhysicalProperties.SensorSize.x = Mathf.Max(PhysicalProperties.SensorSize.x, 0.1f);
            PhysicalProperties.SensorSize.y = Mathf.Max(PhysicalProperties.SensorSize.y, 0.1f);
            PhysicalProperties.FocusDistance = Mathf.Max(PhysicalProperties.FocusDistance, 0.01f);
            if (m_AspectFromCamera == 0)
                m_AspectFromCamera = 1;
            PhysicalProperties.ShutterSpeed = Mathf.Max(0, PhysicalProperties.ShutterSpeed);
            PhysicalProperties.Aperture = Mathf.Clamp(PhysicalProperties.Aperture, Camera.kMinAperture, Camera.kMaxAperture);
            PhysicalProperties.BladeCount = Mathf.Clamp(PhysicalProperties.BladeCount, Camera.kMinBladeCount, Camera.kMaxBladeCount);
            PhysicalProperties.BarrelClipping = Mathf.Clamp01(PhysicalProperties.BarrelClipping);
            PhysicalProperties.Curvature.x = Mathf.Clamp(PhysicalProperties.Curvature.x, Camera.kMinAperture, Camera.kMaxAperture);
            PhysicalProperties.Curvature.y = Mathf.Clamp(PhysicalProperties.Curvature.y, PhysicalProperties.Curvature.x, Camera.kMaxAperture);
            PhysicalProperties.Anamorphism = Mathf.Clamp(PhysicalProperties.Anamorphism, -1, 1);
        }

        /// <summary>
        /// Compare two lens settings objects for approximate equality
        /// </summary>
        /// <param name="a">First LensSettings</param>
        /// <param name="b">Second Lens Settings</param>
        /// <returns>True if the two lenses are approximately equal</returns>
        public static bool AreEqual(ref LensSettings a, ref LensSettings b)
        {
            return Mathf.Approximately(a.NearClipPlane, b.NearClipPlane)
                && Mathf.Approximately(a.FarClipPlane, b.FarClipPlane)
                && Mathf.Approximately(a.OrthographicSize, b.OrthographicSize)
                && Mathf.Approximately(a.FieldOfView, b.FieldOfView)
                && Mathf.Approximately(a.Dutch, b.Dutch)
                && Mathf.Approximately(a.PhysicalProperties.LensShift.x, b.PhysicalProperties.LensShift.x)
                && Mathf.Approximately(a.PhysicalProperties.LensShift.y, b.PhysicalProperties.LensShift.y)

                && Mathf.Approximately(a.PhysicalProperties.SensorSize.x, b.PhysicalProperties.SensorSize.x)
                && Mathf.Approximately(a.PhysicalProperties.SensorSize.y, b.PhysicalProperties.SensorSize.y)
                && a.PhysicalProperties.GateFit == b.PhysicalProperties.GateFit
                && Mathf.Approximately(a.PhysicalProperties.FocusDistance, b.PhysicalProperties.FocusDistance)
                && Mathf.Approximately(a.PhysicalProperties.Iso, b.PhysicalProperties.Iso)
                && Mathf.Approximately(a.PhysicalProperties.ShutterSpeed, b.PhysicalProperties.ShutterSpeed)
                && Mathf.Approximately(a.PhysicalProperties.Aperture, b.PhysicalProperties.Aperture)
                && a.PhysicalProperties.BladeCount == b.PhysicalProperties.BladeCount
                && Mathf.Approximately(a.PhysicalProperties.Curvature.x, b.PhysicalProperties.Curvature.x)
                && Mathf.Approximately(a.PhysicalProperties.Curvature.y, b.PhysicalProperties.Curvature.y)
                && Mathf.Approximately(a.PhysicalProperties.BarrelClipping, b.PhysicalProperties.BarrelClipping)
                && Mathf.Approximately(a.PhysicalProperties.Anamorphism, b.PhysicalProperties.Anamorphism)
                ;
        }
    }
}
