using System;
using UnityEngine;

namespace Unity.Cinemachine
{
    /// <summary>
    /// Evaluates shot quality in the Finalize stage based on LookAt target occlusion and distance.
    /// </summary>
    [AddComponentMenu("Cinemachine/Procedural/Extensions/Cinemachine Shot Quality Evaluator")]
    [SaveDuringPlay]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequiredTarget(RequiredTargetAttribute.RequiredTargets.Tracking)]
    [HelpURL(Documentation.BaseURL + "manual/CinemachineShotQualityEvaluator.html")]
    public class CinemachineShotQualityEvaluator : CinemachineExtension, IShotQualityEvaluator
    {
        /// <summary>这些图层上的物体将被检测</summary>
        [Tooltip("这些图层上的物体将被检测")]
        public LayerMask OcclusionLayers = 1;

        /// <summary>带有此标签的障碍物将被忽略。建议将此字段设置为目标的标签</summary>
        [TagField]
        [Tooltip("带有此标签的障碍物将被忽略。建议将此字段设置为目标的标签")]
        public string IgnoreTag = string.Empty;

        /// <summary>比这个距离更接近目标的障碍物将被忽略</summary>
        [Tooltip("比这个距离更接近目标的障碍物将被忽略")]
        public float MinimumDistanceFromTarget = 0.2f;

        /// <summary>
        /// 用于检查遮挡的球体投射半径
        /// </summary>
        [Tooltip("用于检查遮挡的球体投射半径")]
        public float CameraRadius;

        /// <summary>镜头质量评估设置</summary>
        [Serializable]
        public struct DistanceEvaluationSettings
        {
            /// <summary>如果启用，将根据目标距离评估镜头质量</summary>
            [Tooltip("如果启用，将根据目标距离评估镜头质量")]
            public bool Enabled;

            /// <summary>如果大于零，当目标距离相机此距离时，将获得最大质量提升</summary>
            [Tooltip("如果大于零，当目标距离相机此距离时，将获得最大质量提升")]
            public float OptimalDistance;

            /// <summary>目标比此距离更接近相机的镜头不会获得质量提升</summary>
            [Tooltip("目标比此距离更接近相机的镜头不会获得质量提升")]
            [Delayed]
            public float NearLimit;

            /// <summary>目标比此距离更远离相机的镜头不会获得质量提升</summary>
            [Tooltip("目标比此距离更远离相机的镜头不会获得质量提升")]
            public float FarLimit;

            /// <summary>高质量镜头将按其正常质量的此比例获得提升</summary>
            [Tooltip("高质量镜头将按其正常质量的此比例获得提升")]
            public float MaxQualityBoost;


            internal static DistanceEvaluationSettings Default => new () { NearLimit = 5, FarLimit = 30, OptimalDistance = 10, MaxQualityBoost = 0.2f };
        }
        /// <summary>If enabled, will evaluate shot quality based on target distance and occlusion</summary>
        [FoldoutWithEnabledButton]
        public DistanceEvaluationSettings DistanceEvaluation = DistanceEvaluationSettings.Default;

        void OnValidate()
        {
            CameraRadius = Mathf.Max(0, CameraRadius);
            MinimumDistanceFromTarget = Mathf.Max(0.01f, MinimumDistanceFromTarget);
            CameraRadius = Mathf.Max(0, CameraRadius);
            DistanceEvaluation.NearLimit = Mathf.Max(0.1f, DistanceEvaluation.NearLimit);
            DistanceEvaluation.FarLimit = Mathf.Max(DistanceEvaluation.NearLimit, DistanceEvaluation.FarLimit);
            DistanceEvaluation.OptimalDistance = Mathf.Clamp(
                DistanceEvaluation.OptimalDistance, DistanceEvaluation.NearLimit, DistanceEvaluation.FarLimit);
        }

        private void Reset()
        {
            OcclusionLayers = 1;
            IgnoreTag = string.Empty;
            MinimumDistanceFromTarget = 0.2f;
            CameraRadius = 0;
            DistanceEvaluation = DistanceEvaluationSettings.Default;
        }

        /// <inheritdoc />
        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage == CinemachineCore.Stage.Finalize && state.HasLookAt())
            {
                var targetObscured = state.IsTargetOffscreen() || IsTargetObscured(state);
                if (targetObscured)
                    state.ShotQuality *= 0.2f;

                if (DistanceEvaluation.Enabled)
                {
                    float nearnessBoost = 0;
                    if (DistanceEvaluation.OptimalDistance > 0)
                    {
                        var distance = Vector3.Magnitude(state.ReferenceLookAt - state.GetFinalPosition());
                        if (distance <= DistanceEvaluation.OptimalDistance)
                        {
                            if (distance >= DistanceEvaluation.NearLimit)
                                nearnessBoost = DistanceEvaluation.MaxQualityBoost * (distance - DistanceEvaluation.NearLimit)
                                    / (DistanceEvaluation.OptimalDistance - DistanceEvaluation.NearLimit);
                        }
                        else
                        {
                            distance -= DistanceEvaluation.OptimalDistance;
                            if (distance < DistanceEvaluation.FarLimit)
                                nearnessBoost = DistanceEvaluation.MaxQualityBoost * (1f - (distance / DistanceEvaluation.FarLimit));
                        }
                        state.ShotQuality *= (1f + nearnessBoost);
                    }
                }
            }
        }

        bool IsTargetObscured(CameraState state)
        {
#if CINEMACHINE_PHYSICS
            var lookAtPos = state.ReferenceLookAt;
            var pos = state.GetCorrectedPosition();
            var dir = lookAtPos - pos;
            var distance = dir.magnitude;
            if (distance < Mathf.Max(MinimumDistanceFromTarget, Epsilon))
                return true;
            var ray = new Ray(pos, dir.normalized);
            return RuntimeUtility.SphereCastIgnoreTag(
                    ray, CameraRadius, out _, distance - MinimumDistanceFromTarget, OcclusionLayers, IgnoreTag);
#else
            return false;
#endif
        }
    }
}
