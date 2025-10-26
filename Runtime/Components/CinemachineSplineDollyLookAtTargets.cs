using System;
using UnityEngine;
using UnityEngine.Splines;

namespace Unity.Cinemachine
{
    /// <summary>
    /// CinemachineSplineDollyLookAtTargets is a component that allows the camera to look at
    /// specific points in the world as it moves along a spline.
    /// </summary>
    [ExecuteAlways, SaveDuringPlay]
    [CameraPipeline(CinemachineCore.Stage.Aim)]
    [AddComponentMenu("Cinemachine/Procedural/Rotation Control/Cinemachine Spline Dolly LookAt Targets")]
    [DisallowMultipleComponent]
    [HelpURL(Documentation.BaseURL + "manual/CinemachineSplineDollyLookAtTargets.html")]
    public class CinemachineSplineDollyLookAtTargets : CinemachineComponentBase
    {
        /// <summary>LookAt targets for the camera at specific positions on the Spline</summary>
        [Serializable]
        public struct Item
        {
            
            /// <summary>The target object to look at.  It may be None, in which case the Offset will specify a point in world spac</summary>
            [Tooltip("要注视的目标对象。它可以为“无”（None），这种情况下，偏移量（Offset）将指定世界空间中的一个点。")]
            public Transform LookAt;

            /// <summary>The offset (in local coords) from the LookAt target's origin.  If LookAt target is None, this will specify a world-space point</summary>
            [Tooltip("相对于注视目标原点的偏移量（位于局部坐标系中）。如果注视目标为“无”（None），则此偏移量将指定世界空间中的一个点。")]
            public Vector3 Offset;

            /// <summary>Easing value for the Bezier curve. 0 is linear, 1 is smooth.</summary>
            [Tooltip("控制对此数据点的缓入和缓出方式。值为0时，将在注视点（LookAt points）之间进行线性插值；值为1时，会减缓旋转速度并短暂停顿以注视目标。")]
            [Range(0, 1)]
            public float Easing;

            /// <summary>Get/set the LookAt point in world space.</summary>
            public Vector3 WorldLookAt
            {
                readonly get => LookAt == null ? Offset : LookAt.TransformPoint(Offset);
                set => Offset = LookAt == null ? value : LookAt.InverseTransformPoint(value);
            }
        }

        /// <summary>Interpolator for the Targets</summary>
        internal struct LerpItem : IInterpolator<Item>
        {
            public Item Interpolate(Item a, Item b, float t)
            {
                var t2 = t * t;
                var d = 1f - t;
                t = 3f * d * d * t * Mathf.Lerp(0.3333f, 0, a.Easing) + 3f * d * t2 * Mathf.Lerp(0.6666f, 1, b.Easing) + t * t2;
                return new Item { Offset = Vector3.Lerp(a.WorldLookAt, b.WorldLookAt, t) };
            }
        }

        /// <summary>LookAt targets for the camera at specific positions on the Spline</summary>
        /// <remarks>It is not recommended to modify the data array at runtime, because the infrastructure
        /// expects the array to be in strictly increasing order of distance along the spline.  If you do change
        /// the array at runtime, you must take care to keep it in this order, or the results will be unpredictable.
        /// </remarks>
        [Tooltip("相机在样条线（Spline）特定位置上的注视目标（LookAt targets）。")]
        public SplineData<Item> Targets = new () { DefaultValue = new Item { Easing = 1 } };

        void Reset() => Targets = new SplineData<Item> { DefaultValue = new Item { Easing = 1 } };

        /// <inheritdoc/>
        public override bool IsValid => enabled && Targets != null && GetGetSplineAndDolly(out _, out _);

        /// <inheritdoc/>
        public override CinemachineCore.Stage Stage => CinemachineCore.Stage.Aim;

        /// <inheritdoc/>
        public override void MutateCameraState(ref CameraState state, float deltaTime)
        {
            if (!GetGetSplineAndDolly(out _, out var dolly))
                return;

            var spline = dolly.SplineSettings.GetCachedSpline();
            var item = Targets.Evaluate(spline, dolly.CameraPosition, dolly.PositionUnits, new LerpItem());
            var dir = item.WorldLookAt - state.RawPosition;
            if (dir.sqrMagnitude > UnityVectorExtensions.Epsilon)
            {
                var up = state.ReferenceUp;
                if (Vector3.Cross(dir, up).sqrMagnitude < UnityVectorExtensions.Epsilon)
                {
                    // Look direction is parallel to the up vector
                    up = state.RawOrientation * Vector3.back;
                    if (Vector3.Cross(dir, up).sqrMagnitude < UnityVectorExtensions.Epsilon)
                        up = state.RawOrientation * Vector3.left;
                }
                state.RawOrientation = Quaternion.LookRotation(dir, up);
            }
            state.ReferenceLookAt = item.Offset;
        }

        /// <summary>
        /// API for the inspector: Get the spline and the required CinemachineTrackDolly component.
        /// </summary>
        /// <param name="spline">The spline being augmented</param>
        /// <param name="dolly">The associated CinemachineTrackDolly component</param>
        /// <returns></returns>
        internal bool GetGetSplineAndDolly(out SplineContainer spline, out CinemachineSplineDolly dolly)
        {
            dolly = null;
            if (this != null && TryGetComponent(out dolly))
            {
                spline = dolly.Spline;
                return spline != null && spline.Spline != null;
            }
            spline = null;
            return false;
        }
    }
}
