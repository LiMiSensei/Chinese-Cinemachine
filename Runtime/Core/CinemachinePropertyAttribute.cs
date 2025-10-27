using System;
using UnityEngine;

namespace Unity.Cinemachine
{
    /// <summary>
    /// 应用于旧版输入轴名称规范的属性。用于检查器中的自定义绘制。
    /// </summary>
    public sealed class InputAxisNamePropertyAttribute : PropertyAttribute {}

    /// <summary>
    /// 抑制复杂属性上的顶级折叠
    /// </summary>
    public sealed class HideFoldoutAttribute : PropertyAttribute {}

    /// <summary>如果给定类型的组件不存在，则隐藏此属性</summary>
    public sealed class HideIfNoComponentAttribute : PropertyAttribute
    {
        /// <summary>控制启用状态的字段名称</summary>
        public Type ComponentType;

        /// <summary>构造函数</summary>
        /// <param name="type">要检查的组件类型</param>
        public HideIfNoComponentAttribute(Type type) => ComponentType = type;
    }

    /// <summary>
    /// 绘制一个带有启用切换按钮的折叠，该按钮会遮蔽折叠内的字段
    /// </summary>
    public class FoldoutWithEnabledButtonAttribute : PropertyAttribute
    {
        /// <summary>控制启用状态的字段名称</summary>
        public string EnabledPropertyName;

        /// <summary>构造函数</summary>
        /// <param name="enabledProperty">控制启用状态的字段名称</param>
        public FoldoutWithEnabledButtonAttribute(string enabledProperty = "Enabled")
            => EnabledPropertyName = enabledProperty;
    }

    /// <summary>
    /// 在单行上绘制 FoldoutWithEnabledButtonAttribute
    /// </summary>
    public sealed class EnabledPropertyAttribute : FoldoutWithEnabledButtonAttribute
    {
        /// <summary>禁用时显示在切换按钮右侧的文本</summary>
        public string ToggleDisabledText;

        /// <summary>构造函数</summary>
        /// <param name="enabledProperty">控制启用状态的字段名称</param>
        /// <param name="toggleText">显示在切换按钮右侧的文本</param>
        public EnabledPropertyAttribute(string enabledProperty = "Enabled", string toggleText = "")
            : base(enabledProperty) => ToggleDisabledText = toggleText;
    }

    /// <summary>
    /// 应用于整型或浮点型字段以在检查器中生成滑块。
    /// </summary>
    [Obsolete("请使用 RangeAttribute 代替")]
    public sealed class RangeSliderAttribute : PropertyAttribute
    {
        /// <summary>范围滑块的最小值</summary>
        public float Min;
        /// <summary>范围滑块的最大值</summary>
        public float Max;
        /// <summary>范围滑块属性的构造函数</summary>
        /// <param name="min">范围滑块的最小值</param>
        /// <param name="max">范围滑块的最大值</param>
        public RangeSliderAttribute(float min, float max) { Min = min; Max = max; }
    }

    /// <summary>
    /// 应用于整型或浮点型字段以在检查器中生成最小-最大范围滑块。
    /// </summary>
    public sealed class MinMaxRangeSliderAttribute : PropertyAttribute
    {
        /// <summary>范围滑块的最小值</summary>
        public float Min;
        /// <summary>范围滑块的最大值</summary>
        public float Max;
        /// <summary>范围滑块属性的构造函数</summary>
        /// <param name="min">范围滑块的最小值</param>
        /// <param name="max">范围滑块的最大值</param>
        public MinMaxRangeSliderAttribute(float min, float max) { Min = min; Max = max; }
    }

    /// <summary>
    /// 应用于 LensSetting 属性。
    /// 将导致属性绘制器隐藏 ModeOverride 设置。
    /// </summary>
    public sealed class LensSettingsHideModeOverridePropertyAttribute : PropertyAttribute {}

    /// <summary>用于显示 SensorSize 字段的属性</summary>
    public sealed class SensorSizePropertyAttribute : PropertyAttribute {}

    /// <summary>属性字段是一个标签。</summary>
    public sealed class TagFieldAttribute : PropertyAttribute {}

    /// <summary>
    /// 用于检查器中的自定义绘制。检查器将显示一个包含资源内容的折叠
    /// </summary>
    // GML TODO: 删除此属性
    public sealed class CinemachineEmbeddedAssetPropertyAttribute : PropertyAttribute
    {
        /// <summary>如果为 true，当嵌入资源为空时检查器将显示警告</summary>
        public bool WarnIfNull;

        /// <summary>标准构造函数</summary>
        /// <param name="warnIfNull">如果为 true，当嵌入资源为空时检查器将显示警告</param>
        public CinemachineEmbeddedAssetPropertyAttribute(bool warnIfNull = false) { WarnIfNull = warnIfNull; }
    }

    /// <summary>
    /// 应用于 Vector2 以将 (x, y) 视为 (min, max)。
    /// 用于检查器中的自定义绘制。
    /// </summary>
    public sealed class Vector2AsRangeAttribute : PropertyAttribute {}

    /// <summary>
    /// 为向量的每个浮点字段设置 isDelayed 为 true。
    /// </summary>
    public sealed class DelayedVectorAttribute : PropertyAttribute {}

    /// <summary>
    /// 摄像机管线创作组件使用的属性，用于指示
    /// 它们属于管线的哪个阶段。
    /// </summary>
    public sealed class CameraPipelineAttribute : System.Attribute
    {
        /// <summary>获取此组件在摄像机管线中的定位阶段</summary>
        public CinemachineCore.Stage Stage { get; private set; }

        /// <summary>构造函数：在此定义管线阶段。</summary>
        /// <param name="stage">此组件在摄像机管线中的定位阶段</param>
        public CameraPipelineAttribute(CinemachineCore.Stage stage) { Stage = stage; }
    }

    /// <summary>
    /// 检查器用于显示关于缺失目标的警告的属性。
    /// 这可以用于 CinemachineComponents 和 CinemachineExtensions。
    /// </summary>
    public sealed class RequiredTargetAttribute : System.Attribute
    {
        /// <summary>需要哪些目标的选择</summary>
        public enum RequiredTargets
        {
            /// <summary>不需要特定目标。</summary>
            None,
            /// <summary>管线元素工作需要跟踪目标</summary>
            Tracking,
            /// <summary>管线元素工作需要注视目标</summary>
            LookAt,
            /// <summary>管线元素工作需要注视目标且必须是 ICinemachineTargetGroup</summary>
            GroupLookAt
        };

        /// <summary>获取此组件在摄像机管线中的定位阶段</summary>
        public RequiredTargets RequiredTarget { get; private set; }

        /// <summary>构造函数：在此定义管线阶段。</summary>
        /// <param name="requiredTarget">需要哪些目标</param>
        public RequiredTargetAttribute(RequiredTargets requiredTarget) { RequiredTarget = requiredTarget; }
    }

    /// <summary>
    /// 应用于 CinemachineCameraManagerBase 属性以在
    /// 检查器中生成子摄像机选择器。
    /// </summary>
    public sealed class ChildCameraPropertyAttribute : PropertyAttribute {}

    /// <summary>
    /// 在检查器中绘制嵌入的 BlenderSettings 资源。
    /// </summary>
    public sealed class EmbeddedBlenderSettingsPropertyAttribute : PropertyAttribute {}
}