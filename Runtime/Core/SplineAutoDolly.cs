using System;
using UnityEngine;
using UnityEngine.Splines;

namespace Unity.Cinemachine
{
    /// <summary>
    /// This structure holds the object that implements AutoDolly on a spline.
    /// </summary>
    [Serializable]
    public struct SplineAutoDolly
    {
        /// <summary>如果设置，将在样条上启用自动轨道移动</summary>
        [Tooltip("如果设置，将启用沿样条的选定自动轨道移动")]
        public bool Enabled;

        /// <summary>这是实际实现自动轨道移动的对象</summary>
        [SerializeReference]
        public ISplineAutoDolly Method;

        /// <summary>
        /// 程序化样条轨道移动的接口。
        /// 实现此接口以提供选择路径上点的自定义算法。
        /// </summary>
        public interface ISplineAutoDolly
        {
            /// <summary>从OnValidate()调用以验证设置。</summary>
            void Validate();

            /// <summary>调用此方法以重置实现中包含的任何状态信息。</summary>
            void Reset();

            /// <summary>如果此实现需要跟踪目标，则返回true。</summary>
            bool RequiresTrackingTarget { get; }

            /// <summary>
            /// 计算样条上的期望位置。
            /// </summary>
            /// <param name="sender">发起请求的MonoBehaviour。</param>
            /// <param name="target">目标对象（对于不需要目标的算法可能为null）。</param>
            /// <param name="spline">必须在其上找到位置的样条。</param>
            /// <param name="currentPosition">样条上的当前位置。</param>
            /// <param name="positionUnits">样条位置表达的单位。</param>
            /// <param name="deltaTime">当前deltaTime。如果小于0，则应忽略前一帧的数据。</param>
            /// <returns>样条上的期望位置，以positionUnits表示。</returns>
            float GetSplinePosition(
                MonoBehaviour sender, Transform target, SplineContainer spline,
                float currentPosition, PathIndexUnit positionUnits, float deltaTime);
        }

        /// <summary>
        /// 以恒定速度沿样条移动对象的ISplineAutoDolly实现。
        /// </summary>
        [Serializable]
        public class FixedSpeed : ISplineAutoDolly
        {
            /// <summary>移动速度，以当前位置单位每秒表示。</summary>
            [Tooltip("移动速度，以当前位置单位每秒表示。")]
            public float Speed;

            /// <summary>从OnValidate()调用以验证设置。</summary>
            void ISplineAutoDolly.Validate() {}

            /// <summary>此实现不执行任何操作。</summary>
            void ISplineAutoDolly.Reset() {}

            /// <summary>如果此实现需要跟踪目标，则返回true。</summary>
            bool ISplineAutoDolly.RequiresTrackingTarget => false;

            /// <summary>
            /// 计算样条上的期望位置。
            /// </summary>
            /// <param name="sender">发起请求的MonoBehaviour。</param>
            /// <param name="target">目标对象（对于不需要目标的算法可能为null）。</param>
            /// <param name="spline">必须在其上找到位置的样条。</param>
            /// <param name="currentPosition">样条上的当前位置。</param>
            /// <param name="positionUnits">样条位置表达的单位。</param>
            /// <param name="deltaTime">当前deltaTime。如果小于0，则应忽略前一帧的数据。</param>
            /// <returns>样条上的期望位置，以positionUnits表示。</returns>
            float ISplineAutoDolly.GetSplinePosition(
                MonoBehaviour sender, Transform target, SplineContainer spline,
                float currentPosition, PathIndexUnit positionUnits, float deltaTime)
            {
                // 仅在播放时工作
                if (Application.isPlaying && spline.IsValid() && deltaTime > 0)
                    return currentPosition + Speed * deltaTime;
                return currentPosition;
            }
        }

        /// <summary>
        /// 查找样条上离目标最近点的ISplineAutoDolly实现。
        /// 注意这是一个简单的无状态算法，并不适用于所有样条形状。
        /// 例如，如果样条形成弧线且目标位于弧线内部，则最近点
        /// 可能不稳定或未定义。考虑一个完美圆形的样条，
        /// 目标位于圆心的情况。最近点在哪里？
        /// </summary>
        [Serializable]
        public class NearestPointToTarget : ISplineAutoDolly
        {
            /// <summary>
            /// 从样条上最近点到跟随目标的偏移量，以当前位置单位表示。
            /// </summary>
            [Tooltip("从样条上最近点到跟随目标的偏移量，以当前位置单位表示")]
            public float PositionOffset = 0;

            /// <summary>
            /// 影响计算最近点时将样条分割成的段数。
            /// 值越大意味着段更小且更多，这会在增加处理时间的代价下提高准确性。
            /// 在大多数情况下，默认分辨率是合适的。与<see cref="SearchIteration"/>一起使用
            /// 以微调点精度。
            /// 更多信息请参见SplineUtility.GetNearestPoint。
            /// </summary>
            [Tooltip("影响计算最近点时将样条分割成的段数。"
                + "值越大意味着段更小且更多，这会在增加处理时间的代价下提高准确性。"
                + "在大多数情况下，默认值(4)是合适的。与SearchIteration一起使用"
                + "以微调点精度。")]
            public int SearchResolution = 4;

            /// <summary>
            /// 通过使用<see cref="SearchResolution"/>将样条整个长度分割成等距线段来
            /// 计算最近点。连续的迭代将进一步细分最近段，产生更
            /// 准确的结果。在大多数情况下，默认值已足够。
            /// 更多信息请参见SplineUtility.GetNearestPoint。
            /// </summary>
            [Tooltip("通过使用SearchResolution将样条整个长度分割成等距线段来"
                + "计算最近点。连续的迭代将进一步细分最近段，产生更"
                + "准确的结果。在大多数情况下，默认值(2)已足够。")]
            public int SearchIteration = 2;


            /// <summary>Called from OnValidate() to validate the settings.</summary>
            void ISplineAutoDolly.Validate()
            {
                SearchResolution = Mathf.Max(SearchResolution, 1);
                SearchIteration = Mathf.Max(SearchIteration, 1);
            }

            /// <summary>This implementation does nothing.</summary>
            void ISplineAutoDolly.Reset() {}

            /// <summary>Returns true if this implementation requires a tracking target.</summary>
            bool ISplineAutoDolly.RequiresTrackingTarget => true;

            /// <summary>
            /// Compute the desired position on the spline.
            /// </summary>
            /// <param name="sender">The MonoBehaviour that is asking.</param>
            /// <param name="target">The target object (may be null for algorithms that don't require it).</param>
            /// <param name="spline">The spline on which the location must be found.</param>
            /// <param name="currentPosition">The current position on the spline.</param>
            /// <param name="positionUnits">The units in which spline positions are expressed.</param>
            /// <param name="deltaTime">Current deltaTime.  If smaller than 0, then previous frame data should be ignored.</param>
            /// <returns>The desired position on the spline, expressed in positionUnits.</returns>
            float ISplineAutoDolly.GetSplinePosition(
                MonoBehaviour sender, Transform target, SplineContainer spline,
                float currentPosition, PathIndexUnit positionUnits, float deltaTime)
            {
                if (target == null || !spline.IsValid())
                    return currentPosition;

                // Convert target into spline local space, because SplineUtility works in spline local space
                SplineUtility.GetNearestPoint(spline.Spline,
                    spline.transform.InverseTransformPoint(target.position), out _, out var normalizedPos,
                    SearchResolution, SearchIteration);

                // GML hack because SplineUtility.GetNearestPoint is buggy
                normalizedPos = Mathf.Clamp01(normalizedPos);

                var pos = spline.Spline.ConvertIndexUnit(normalizedPos, PathIndexUnit.Normalized, positionUnits);
                return pos + PositionOffset;
            }
        }
    }
}
