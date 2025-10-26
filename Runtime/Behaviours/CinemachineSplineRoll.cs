using System;
using UnityEngine;
using UnityEngine.Splines;

namespace Unity.Cinemachine
{
    /// <summary>
    /// Extension that can be added to a SplineContainer or a CinemachineCamera that uses a SplineContainer, for example a CinemachineCamera
    /// that has SplineDolly as Body component.
    /// - When CinemachineSplineRoll is added to a gameObject that has SplineContainer,
    /// then the roll affects any CinemachineCamera that reads that SplineContainer globally.
    /// - When CinemachineSplineRoll is added to a CinemachineCamera, then roll only affects that CinemachineCamera locally.
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [AddComponentMenu("Cinemachine/Helpers/Cinemachine Spline Roll")]
    [HelpURL(Documentation.BaseURL + "manual/CinemachineSplineRoll.html")]
    [SaveDuringPlay]
    public class CinemachineSplineRoll : MonoBehaviour, ISerializationCallbackReceiver
    {
        /// <summary>Structure to hold roll value for a specific location on the track.</summary>
        [Serializable]
        public struct RollData
        {
            /// <summary>
            /// 在轨道特定位置围绕前向方向的滚动角度（以度为单位）。
            /// 当放置在 SplineContainer 上时，这将是一个全局覆盖，会影响所有使用该样条的虚拟相机。
            /// 当放置在 CinemachineCamera 上时，这将是一个局部覆盖，仅影响该 CinemachineCamera。
            /// </summary>
            [Tooltip("在轨道特定位置围绕前向方向的滚动角度（以度为单位）。\n" +
                "- 当放置在 SplineContainer 上时，这将是一个全局覆盖，会影响所有使用该样条的虚拟相机。\n" +
                "- 当放置在 CinemachineCamera 上时，这将是一个局部覆盖，仅影响该 CinemachineCamera。")]
            public float Value;

            /// <summary> 隐式转换为 float </summary>
            /// <param name="roll"> 要转换的 RollData 设置。 </param>
            /// <returns> RollData 设置的值。 </returns>
            public static implicit operator float(RollData roll) => roll.Value;

            /// <summary> 从 float 隐式转换 </summary>
            /// <param name="roll"> 用于初始化 RollData 设置的值。 </param>
            /// <returns> 具有给定值的新 RollData 设置。 </returns>
            public static implicit operator RollData(float roll) => new () { Value = roll };

        }

        /// <summary>
        /// 启用时，滚动角度会平滑地过渡到数据点值。否则，插值为线性。
        /// </summary>
        [Tooltip("启用时，滚动角度会平滑地过渡到数据点值。否则，插值为线性。")]
        public bool Easing = true;

        /// <summary>
        /// Get the appropriate interpolator for the RollData, depending on the Easing setting
        /// </summary>
        /// <returns>The appropriate interpolator for the RollData, depending on the Easing setting.</returns>
        public IInterpolator<RollData> GetInterpolator() => Easing ? new LerpRollDataWithEasing() : new LerpRollData();

        /// <summary>Interpolator for the RollData, with no easing between data points.</summary>
        public struct LerpRollData : IInterpolator<RollData>
        {
            /// <inheritdoc/>
            public RollData Interpolate(RollData a, RollData b, float t) => new() { Value = Mathf.Lerp(a.Value, b.Value, t) };
        }

        /// <summary>Interpolator for the RollData, with easing between data points</summary>
        public struct LerpRollDataWithEasing : IInterpolator<RollData>
        {
            /// <inheritdoc/>
            public RollData Interpolate(RollData a, RollData b, float t)
            {
                var t2 = t * t;
                var d = 1f - t;
                t = 3f * d * t2 + t * t2;
                return new() { Value = Mathf.Lerp(a.Value, b.Value, t) };
            }
        }

        /// <summary>
        /// Roll (in degrees) around the forward direction for specific location on the track.
        /// When placed on a SplineContainer, this is going to be a global override that affects all vcams using the Spline.
        /// When placed on a CinemachineCamera, this is going to be a local override that only affects that CinemachineCamera.
        /// </summary>
        /// <remarks>
        /// It is not recommended to modify the data array at runtime, because the infrastructure
        /// expects the array to be in strclty increasing order of distance along the spline.  If you do change
        /// the array at runtime, you must take care to keep it in this order, or the results will be unpredictable.
        /// </remarks>
        [HideFoldout]
        public SplineData<RollData> Roll;

#if UNITY_EDITOR
        // Only needed for drawing the gizmo
        internal ISplineContainer SplineContainer
        {
            get
            {
                // In case behaviour was re-parented in the editor, we check every time
                if (TryGetComponent(out ISplineReferencer referencer))
                    return referencer.SplineSettings.Spline == null || referencer.SplineSettings.Spline.Splines.Count == 0
                        ? null : referencer.SplineSettings.Spline;
                if (TryGetComponent(out ISplineContainer container))
                    return container.Splines.Count > 0 ? container : null;
                return null;
            }
        }
#endif


        //============================================================================
        // Legacy streaming support

        [HideInInspector, SerializeField, NoSaveDuringPlay]
        int m_StreamingVersion;

        void PerformLegacyUpgrade(int streamedVersion)
        {
            if (streamedVersion < 20240101)
            {
                // roll values were inverted
                for (int i = 0; i < Roll.Count; ++i)
                {
                    var item = Roll[i];
                    item.Value = -item.Value;
                    Roll[i] = item;
                }
            }
        }
        //============================================================================

        void Reset()
        {
            Roll?.Clear();
            Easing = true;
        }

        void OnEnable() {} // Needed so we can disable it in the editor

        /// <inheritdoc/>
        public void OnBeforeSerialize() {}

        /// <inheritdoc/>
        public void OnAfterDeserialize()
        {
            // Perform legacy upgrade if necessary
            if (m_StreamingVersion < CinemachineCore.kStreamingVersion)
                PerformLegacyUpgrade(m_StreamingVersion);
            m_StreamingVersion = CinemachineCore.kStreamingVersion;
        }

        /// <summary>Cache for clients that use CinemachineSplineRoll</summary>
        internal struct RollCache
        {
            CinemachineSplineRoll m_RollCache;

            public void Refresh(MonoBehaviour owner)
            {
                m_RollCache = null;

                // Check if owner has CinemachineSplineRoll
                if (!owner.TryGetComponent(out m_RollCache) && owner is ISplineReferencer referencer)
                {
                    // Check if the spline has CinemachineSplineRoll
                    var spline = referencer.SplineSettings.Spline;
                    if (spline != null && spline is Component component)
                        component.TryGetComponent(out m_RollCache);
                }
            }

            public CinemachineSplineRoll GetSplineRoll(MonoBehaviour owner)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    Refresh(owner);
#endif
                return m_RollCache;
            }
        }
    }
}
