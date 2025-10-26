using System;
using UnityEngine;

namespace Unity.Cinemachine.Samples
{
    /// <summary>
    /// An example add-on module for Cinemachine Camera for controlling
    /// the FadeOut shader included in our example package.
    /// </summary>
    [ExecuteAlways]
    public class FadeOutShaderController : CinemachineExtension
    {
        /// <summary>注视目标的半径。</summary>
        [Tooltip("注视目标的半径。")]
        public float LookAtTargetRadius = 1;

        /// <summary>
        /// 范围从摄像机（x值）沿注视方向（y值）定义，在此范围内的
        /// 使用淡出材质的对象将变为透明。
        /// 透明度的强度取决于对象距离摄像机的远近和淡出范围。
        /// </summary>
        [Tooltip("范围从摄像机（x值）沿注视方向（y值）定义，在此范围内的" +
            "使用淡出材质的对象将变为透明。\n透明度的强度取决于" +
            "对象距离摄像机的远近和淡出范围。")]
        [MinMaxRangeSlider(0, 20)]
        public Vector2 FadeOutRange = new (0f, 10f);

        /// <summary>
        /// 如果为true，最大距离将设置为
        /// 此虚拟摄像机与注视目标之间的距离减去注视目标半径。
        /// </summary>
        [Tooltip("如果为true，最大距离将设置为" +
            "此虚拟摄像机与注视目标之间的距离减去注视目标半径。")]
        public bool MaxDistanceControlledByCamera = true;

        /// <summary>使用淡出着色器的材质。</summary>
        [Tooltip("使用淡出着色器的材质。")]
        public Material FadeOutMaterial;

        static readonly int k_MaxDistanceID = Shader.PropertyToID("_MaxDistance");
        static readonly int k_MinDistanceID = Shader.PropertyToID("_MinDistance");

        /// <summary>Updates FadeOut shader on the specified FadeOutMaterial.</summary>
        /// <param name="vcam">The virtual camera being processed</param>
        /// <param name="stage">The current pipeline stage</param>
        /// <param name="state">The current virtual camera state</param>
        /// <param name="deltaTime">The current applicable deltaTime</param>
        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage == CinemachineCore.Stage.Finalize)
            {
                if (FadeOutMaterial == null ||
                    !FadeOutMaterial.HasProperty(k_MaxDistanceID) ||
                    !FadeOutMaterial.HasProperty(k_MinDistanceID))
                    return;

                if (MaxDistanceControlledByCamera && vcam.LookAt != null)
                    FadeOutRange.y = Vector3.Distance(vcam.transform.position, vcam.LookAt.position) - LookAtTargetRadius;

                FadeOutMaterial.SetFloat(k_MinDistanceID, FadeOutRange.x);
                FadeOutMaterial.SetFloat(k_MaxDistanceID, FadeOutRange.y);
            }
        }

        void OnValidate()
        {
            LookAtTargetRadius = Math.Max(0, LookAtTargetRadius);
            FadeOutRange.x = Math.Max(0, FadeOutRange.x);
            FadeOutRange.y = Math.Max(0, FadeOutRange.y);
            FadeOutRange.y = Math.Max(FadeOutRange.x, FadeOutRange.y);
        }
    }
}
