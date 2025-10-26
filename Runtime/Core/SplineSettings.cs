using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Unity.Cinemachine
{
    /// <summary>Interface for behaviours that reference a Spline</summary>
    public interface ISplineReferencer
    {
        /// <summary>Get a reference to the SplineSettings struct contained in this object.</summary>
        /// <value>A reference to the embedded SplineSettings struct</value>
        public ref SplineSettings SplineSettings { get; }
    }

    /// <summary>
    /// This structure holds the spline reference and the position and position units.
    /// </summary>
    [Serializable]
    public struct SplineSettings
    {
        /// <summary>位置将应用到的样条容器。</summary>
        [Tooltip("位置将应用到的样条容器。")]
        public SplineContainer Spline;

        /// <summary>沿样条的位置。对应样条上给定点的实际值将取决于单位类型。</summary>
        [NoSaveDuringPlay]
        [Tooltip("沿样条的位置。对应样条上给定点的实际值将取决于单位类型。")]
        public float Position;

        /// <summary>如何解释样条位置：
        /// - 距离：值范围从0（样条起点）到样条长度（样条终点）。
        /// - 归一化：值范围从0（样条起点）到1（样条终点）。
        /// - 节点：值由节点索引和一个表示在特定节点索引与下一个节点之间归一化插值的分数值定义。</summary>
        [Tooltip("如何解释样条位置：\n"
            + "- <b>距离</b>：值范围从0（样条起点）到样条长度（样条终点）。\n"
            + "- <b>归一化</b>：值范围从0（样条起点）到1（样条终点）。\n"
            + "- <b>节点</b>：值由节点索引和一个表示在特定节点索引与下一个节点之间归一化插值的分数值定义。\n")]
        public PathIndexUnit Units;

        /// <summary>
        /// Change the units of the position, preserving the position on the spline.
        /// The value of Position may change in order to preserve the position on the spline.
        /// </summary>
        /// <param name="newUnits">The new units to use</param>
        public void ChangeUnitPreservePosition(PathIndexUnit newUnits)
        {
            if (Spline.IsValid() && newUnits != Units)
                Position = GetCachedSpline().ConvertIndexUnit(Position, Units, newUnits);
            Units = newUnits;
        }

        CachedScaledSpline m_CachedSpline;
        int m_CachedFrame;

        /// <summary>
        /// Computing spline length dynamically is costly.  This method computes the length on the first call
        /// and caches it for subsequent calls.
        ///
        /// While we can auto-detect changes to the transform and some changes to the spline's knots, it would be
        /// too costly to continually check for subtle changes to the spline's control points.  Therefore, if such
        /// subtle changes are made to the spline's control points at runtime, client is responsible
        /// for calling InvalidateCache().
        /// </summary>
        /// <returns>Cached version of the spline with transform incorporated</returns>
        internal CachedScaledSpline GetCachedSpline()
        {
            if (!Spline.IsValid())
                InvalidateCache();
            else
            {
                // Only check crude validity once per frame, to keep things efficient
                if (m_CachedSpline == null || (Time.frameCount != m_CachedFrame && !m_CachedSpline.IsCrudelyValid(Spline.Spline, Spline.transform)))
                {
                    InvalidateCache();
                    m_CachedSpline = new CachedScaledSpline(Spline.Spline, Spline.transform);
                }
#if UNITY_EDITOR
                // Deep check only in editor and if not playing
                else if (!Application.isPlaying && Time.frameCount != m_CachedFrame && !m_CachedSpline.KnotsAreValid(Spline.Spline, Spline.transform))
                {
                    InvalidateCache();
                    m_CachedSpline = new CachedScaledSpline(Spline.Spline, Spline.transform);
                }
#endif
                m_CachedFrame = Time.frameCount;
            }
            return m_CachedSpline;
        }

        /// <summary>
        /// While we can auto-detect changes to the transform and some changes to the spline's knots, it would be
        /// too costly to continually check for subtle changes to the spline's control points.  Therefore, if such
        /// subtle changes are made to the spline's control points at runtime, client is responsible
        /// for calling InvalidateCache().
        /// </summary>
        public void InvalidateCache()
        {
            m_CachedSpline?.Dispose();
            m_CachedSpline = null;
        }
    }


    /// <summary>
    /// In order to properly handle the spline scale, we need to cache a spline that incorporates the scale
    /// natively.  This class does that.
    /// Be sure to call Dispose() before discarding this object, otherwise there will be memory leaks.
    /// </summary>
    internal class CachedScaledSpline : ISpline, IDisposable
    {
        NativeSpline m_NativeSpline;
        readonly Spline m_CachedSource;
        //readonly float m_CachedRawLength;
        readonly Vector3 m_CachedScale;
        bool m_IsAllocated;

        /// <summary>Construct a CachedScaledSpline</summary>
        /// <param name="spline">The spline to cache</param>
        /// <param name="transform">The transform to use for the scale, or null</param>
        /// <param name="allocator">The allocator to use for the native spline</param>
        public CachedScaledSpline(Spline spline, Transform transform, Allocator allocator = Allocator.Persistent)
        {
            var scale = transform != null ? transform.lossyScale : Vector3.one;
            m_CachedSource = spline;
            m_NativeSpline = new NativeSpline(spline, Matrix4x4.Scale(scale), allocator);
            //m_CachedRawLength = spline.GetLength();
            m_CachedScale = scale;
            m_IsAllocated = true;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (m_IsAllocated)
                m_NativeSpline.Dispose();
            m_IsAllocated = false;
        }

        /// <summary>Check if the cached spline is still valid, without doing any costly knot checks.</summary>
        /// <param name="spline">The source spline</param>
        /// <param name="transform">The source spline's transform, or null</param>
        /// <returns>True if the spline is crudely unchanged</returns>
        public bool IsCrudelyValid(Spline spline, Transform transform)
        {
            var scale = transform != null ? transform.lossyScale : Vector3.one;
            return spline == m_CachedSource && (m_CachedScale - scale).AlmostZero()
                && m_NativeSpline.Count == m_CachedSource.Count
                //&& Mathf.Abs(m_CachedRawLength - spline.GetLength()) < 0.001f; // this would catch almost everything but is it too expensive?
                ;
        }

        /// <summary>Performs costly knot check to see if the spline's knots have changed.</summary>
        /// <param name="spline">The source spline</param>
        /// <param name="transform">The source spline's transform, or null</param>
        /// <returns>True if the knots have not changed</returns>
        public bool KnotsAreValid(Spline spline, Transform transform)
        {
            if (m_NativeSpline.Count != spline.Count)
                return false;

            var m = Matrix4x4.Scale(transform != null ? transform.lossyScale : Vector3.one);
            var ita = GetEnumerator();
            var itb = spline.GetEnumerator();
            while (ita.MoveNext() && itb.MoveNext())
                if (!ita.Current.Equals(itb.Current.Transform(m)))
                    return false;
            return true;
        }

        /// <inheritdoc/>
        public BezierKnot this[int index] => m_NativeSpline[index];
        /// <inheritdoc/>
        public bool Closed => m_NativeSpline.Closed;
        /// <inheritdoc/>
        public int Count => m_NativeSpline.Count;
        /// <inheritdoc/>
        public BezierCurve GetCurve(int index) => m_NativeSpline.GetCurve(index);
        /// <inheritdoc/>
        public float GetCurveInterpolation(int curveIndex, float curveDistance) => m_NativeSpline.GetCurveInterpolation(curveIndex, curveDistance);
        /// <inheritdoc/>
        public float GetCurveLength(int index) => m_NativeSpline.GetCurveLength(index);
#if CINEMACHINE_SPLINES_2_4
        /// <inheritdoc/>
        public float3 GetCurveUpVector(int index, float t) => m_NativeSpline.GetCurveUpVector(index, t);
#endif
        /// <inheritdoc/>
        public IEnumerator<BezierKnot> GetEnumerator() => m_NativeSpline.GetEnumerator();
        /// <inheritdoc/>
        public float GetLength() => m_NativeSpline.GetLength();
        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => m_NativeSpline.GetEnumerator();
    }
}
