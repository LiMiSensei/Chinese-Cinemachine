using UnityEngine;
using System;
using UnityEngine.Serialization;
using System.Collections.Generic;

namespace Unity.Cinemachine
{
    /// <summary>
    /// 表示可作为虚拟相机目标的对象的接口。
    /// 它具有变换、边界框和边界球。
    /// </summary>
    public interface ICinemachineTargetGroup
    {
        /// <summary>
        /// 如果对象未被删除，则返回 true。
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// 获取 MonoBehaviour 的变换
        /// </summary>
        Transform Transform { get; }

        /// <summary>
        /// 组的轴对齐边界框，使用目标位置和半径计算
        /// </summary>
        Bounds BoundingBox { get; }

        /// <summary>
        /// 组的边界球，使用目标位置和半径计算
        /// </summary>
        BoundingSphere Sphere { get; }

        /// <summary>
        /// 如果组中没有非零权重的成员，则返回 true
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>组的轴对齐边界框，在特定参考系中</summary>
        /// <param name="observer">计算边界框所处的参考系</param>
        /// <param name="includeBehind">如果为 true，将包含观察者后方（z 轴负方向）的成员</param>
        /// <returns>所需参考系中组的轴对齐边界框</returns>
        Bounds GetViewSpaceBoundingBox(Matrix4x4 observer, bool includeBehind);

        /// <summary>
        /// 从特定视角获取组的局部空间角度边界。
        /// 同时返回成员的 z 深度范围。
        /// 观察者后方（z 轴负方向）的成员将被忽略。
        /// </summary>
        /// <param name="observer">计算的视角，返回值将处于该空间中</param>
        /// <param name="minAngles">成员屏幕角度的下限（度）</param>
        /// <param name="maxAngles">成员屏幕角度的上限（度）</param>
        /// <param name="zRange">成员相对于观察者的最小和最大深度值</param>
        void GetViewSpaceAngularBounds(
            Matrix4x4 observer, out Vector2 minAngles, out Vector2 maxAngles, out Vector2 zRange);
    }

    /// <summary>定义一组目标对象，每个对象都有半径和权重。
    /// 权重用于计算目标组的平均位置。
    /// 组中权重较高的成员将占更大比重。
    /// 边界框的计算会考虑成员的位置、权重和半径。
    /// </summary>
    [AddComponentMenu("Cinemachine/Helpers/Cinemachine Target Group")]
    [SaveDuringPlay]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [HelpURL(Documentation.BaseURL + "manual/CinemachineTargetGroup.html")]
    public class CinemachineTargetGroup : MonoBehaviour, ICinemachineTargetGroup
    {
        /// <summary>保存表示组成员的信息</summary>
        [Serializable] public class Target
        {
            /// <summary>目标对象。该对象的位置和旋转将根据其权重贡献给组的位置和旋转平均值</summary>
            [Tooltip("目标对象。该对象的位置和旋转将根据其权重贡献给组的位置和旋转平均值")]
            [FormerlySerializedAs("target")]
            public Transform Object;

            /// <summary>计算平均值时目标的权重。不能为负数</summary>
            [Tooltip("计算平均值时目标的权重。不能为负数")]
            [FormerlySerializedAs("weight")]
            public float Weight = 1;

            /// <summary>目标的半径，用于计算边界框。不能为负数</summary>
            [Tooltip("目标的半径，用于计算边界框。不能为负数")]
            [FormerlySerializedAs("radius")]
            public float Radius = 0.5f;
        }

        /// <summary>组位置的计算方式</summary>
        public enum PositionModes
        {
            ///<summary>组位置将是组的轴对齐边界框的中心</summary>
            GroupCenter,
            /// <summary>组位置将是成员位置的加权平均值</summary>
            GroupAverage
        }

       /// <summary>组的位置计算方式</summary>
        [Tooltip("组的位置计算方式。选择GroupCenter表示使用边界框的中心，"
            + "选择GroupAverage表示使用成员位置的加权平均值。")]
        [FormerlySerializedAs("m_PositionMode")]
        public PositionModes PositionMode = PositionModes.GroupCenter;

        /// <summary>组的旋转计算方式</summary>
        public enum RotationModes
        {
            /// <summary>在组的变换中手动设置</summary>
            Manual,
            /// <summary>成员旋转的加权平均值</summary>
            GroupAverage
        }

        /// <summary>组的旋转计算方式</summary>
        [Tooltip("组的旋转计算方式。选择Manual表示使用组变换中的值，"
            + "选择GroupAverage表示使用成员旋转的加权平均值。")]
        [FormerlySerializedAs("m_RotationMode")]
        public RotationModes RotationMode = RotationModes.Manual;

        /// <summary>此枚举定义了可用的更新方法选项</summary>
        public enum UpdateMethods
        {
            /// <summary>在正常的MonoBehaviour Update中更新</summary>
            Update,
            /// <summary>与物理模块同步，在FixedUpdate中更新</summary>
            FixedUpdate,
            /// <summary>在MonoBehaviour LateUpdate中更新</summary>
            LateUpdate
        };

        /// <summary>何时根据组成员的位置更新组的变换</summary>
        [Tooltip("何时根据组成员的位置更新组的变换")]
        [FormerlySerializedAs("m_UpdateMethod")]
        public UpdateMethods UpdateMethod = UpdateMethods.LateUpdate;

        /// <summary>目标对象及其权重和半径，这些将贡献给组的位置、旋转和尺寸的平均值</summary>
        [NoSaveDuringPlay]
        [Tooltip("目标对象及其权重和半径，这些将贡献给组的位置、旋转和尺寸的平均值")]
        public List<Target> Targets = new ();



        float m_MaxWeight;
        float m_WeightSum;
        Vector3 m_AveragePos;
        Bounds m_BoundingBox;
        BoundingSphere m_BoundingSphere;
        int m_LastUpdateFrame = -1;

        // 有效成员的缓存，这样我们就不必一直检查 activeInHierarchy
        List<int> m_ValidMembers = new ();
        List<bool> m_MemberValidity = new ();

        void OnValidate()
        {
            var count = Targets.Count;
            for (int i = 0; i < count; ++i)
            {
                Targets[i].Weight = Mathf.Max(0, Targets[i].Weight);
                Targets[i].Radius = Mathf.Max(0, Targets[i].Radius);
            }
        }

        void Reset()
        {
            PositionMode = PositionModes.GroupCenter;
            RotationMode = RotationModes.Manual;
            UpdateMethod = UpdateMethods.LateUpdate;
            Targets.Clear();
        }

        //============================================
        // 遗留支持

        [HideInInspector, SerializeField, NoSaveDuringPlay, FormerlySerializedAs("m_Targets")]
        Target[] m_LegacyTargets;

        void Awake()
        {
            if (m_LegacyTargets != null && m_LegacyTargets.Length > 0)
                Targets.AddRange(m_LegacyTargets);
            m_LegacyTargets = null;
        }

        /// <summary>已过时的目标列表</summary>
        [Obsolete("m_Targets 已过时。请使用 Targets 代替")]
        public Target[] m_Targets
        {
            get => Targets.ToArray();
            set { Targets.Clear(); Targets.AddRange(value); }
        }

        //============================================

        /// <summary>
        /// 获取 MonoBehaviour 的变换
        /// </summary>
        public Transform Transform => transform;

        /// <inheritdoc />
        public bool IsValid => this != null;

        /// <summary>组的轴对齐边界框，使用目标位置和半径计算</summary>
        public Bounds BoundingBox
        {
            get
            {
                if (m_LastUpdateFrame != CinemachineCore.CurrentUpdateFrame)
                    DoUpdate();
                return m_BoundingBox;
            }
            private set => m_BoundingBox = value;
        }

        /// <summary>组的边界球，使用目标位置和半径计算</summary>
        public BoundingSphere Sphere
        {
            get
            {
                if (m_LastUpdateFrame != CinemachineCore.CurrentUpdateFrame)
                    DoUpdate();
                return m_BoundingSphere;
            }
            private set => m_BoundingSphere = value;
        }

        /// <summary>如果没有权重 > 0 的成员，则返回 true。这返回缓存的成员状态，仅在调用 DoUpdate() 后有效。
        /// 如果在该调用后添加或删除成员，在下次更新前，这不一定返回正确的信息。</summary>
        public bool IsEmpty
        {
            get
            {
                if (m_LastUpdateFrame != CinemachineCore.CurrentUpdateFrame)
                    DoUpdate();
                return m_ValidMembers.Count == 0;
            }
        }

        /// <summary>向组中添加成员</summary>
        /// <param name="t">要添加的成员</param>
        /// <param name="weight">新成员的权重</param>
        /// <param name="radius">新成员的半径</param>
        public void AddMember(Transform t, float weight, float radius)
        {
            Targets.Add(new Target { Object = t, Weight = weight, Radius = radius });
        }

        /// <summary>从组中移除成员</summary>
        /// <param name="t">要移除的成员</param>
        public void RemoveMember(Transform t)
        {
            int index = FindMember(t);
            if (index >= 0)
                Targets.RemoveAt(index);
        }

        /// <summary>查找成员在组中的索引。</summary>
        /// <param name="t">要查找的成员</param>
        /// <returns>成员索引，如果不是成员则返回 -1</returns>
        public int FindMember(Transform t)
        {
            var count = Targets.Count;
            for (int i = 0; i < count; ++i)
                if (Targets[i].Object == t)
                    return i;
            return -1;
        }

        /// <summary>
        /// 获取组成员的边界球，已考虑权重。
        /// 当成员的权重趋近于 0 时，位置将插值到组的平均位置。
        /// 注意，此结果仅在调用 DoUpdate 后有效。如果在该调用后添加或删除成员，
        /// 或成员更改了权重或激活状态，在下次更新前，这不一定返回正确的信息。
        /// </summary>
        /// <param name="index">成员索引</param>
        /// <returns>加权边界球</returns>
        public BoundingSphere GetWeightedBoundsForMember(int index)
        {
            if (m_LastUpdateFrame != CinemachineCore.CurrentUpdateFrame)
                DoUpdate();
            if (!IndexIsValid(index) || !m_MemberValidity[index])
                return Sphere;
            return WeightedMemberBoundsForValidMember(Targets[index], m_AveragePos, m_MaxWeight);
        }

        /// <summary>组的轴对齐边界框，在特定参考系中。
        /// 注意，此结果仅在调用 DoUpdate 后有效。如果在该调用后添加或删除成员，
        /// 或成员更改了权重或激活状态，在下次更新前，这不一定返回正确的信息。</summary>
        /// <param name="observer">计算边界框所处的参考系</param>
        /// <param name="includeBehind">如果为 true，将包含观察者后方（z 轴负方向）的成员</param>
        /// <returns>所需参考系中组的轴对齐边界框</returns>
        public Bounds GetViewSpaceBoundingBox(Matrix4x4 observer, bool includeBehind)
        {
            if (m_LastUpdateFrame != CinemachineCore.CurrentUpdateFrame)
                DoUpdate();
            var inverseView = observer;
            if (!Matrix4x4.Inverse3DAffine(observer, ref inverseView))
                inverseView = observer.inverse;
            var b = new Bounds(inverseView.MultiplyPoint3x4(m_AveragePos), Vector3.zero);
            if (CachedCountIsValid)
            {
                bool gotOne = false;
                var unit = 2 * Vector3.one;
                var count = m_ValidMembers.Count;
                for (int i = 0; i < count; ++i)
                {
                    var s = WeightedMemberBoundsForValidMember(Targets[m_ValidMembers[i]], m_AveragePos, m_MaxWeight);
                    s.position = inverseView.MultiplyPoint3x4(s.position);
                    if (s.position.z > 0 || includeBehind)
                    {
                        if (gotOne)
                            b.Encapsulate(new Bounds(s.position, s.radius * unit));
                        else
                            b = new Bounds(s.position, s.radius * unit);
                        gotOne = true;
                    }
                }
            }
            return b;
        }

        bool CachedCountIsValid => m_MemberValidity.Count == Targets.Count;
        bool IndexIsValid(int index) => index >= 0 && index < Targets.Count && CachedCountIsValid;

        static BoundingSphere WeightedMemberBoundsForValidMember(Target t, Vector3 avgPos, float maxWeight)
        {
            var pos = t.Object == null ? avgPos : TargetPositionCache.GetTargetPosition(t.Object);
            var w = Mathf.Max(0, t.Weight);
            if (maxWeight > UnityVectorExtensions.Epsilon && w < maxWeight)
                w /= maxWeight;
            else
                w = 1;
            return new BoundingSphere(Vector3.Lerp(avgPos, pos, w), t.Radius * w);
        }

        /// <summary>
        /// 立即根据成员的变换更新组的变换。
        /// 通常这会由 Update() 或 LateUpdate() 自动调用。
        /// </summary>
        public void DoUpdate()
        {
            m_LastUpdateFrame = CinemachineCore.CurrentUpdateFrame;

            UpdateMemberValidity();
            m_AveragePos = CalculateAveragePosition();
            BoundingBox = CalculateBoundingBox();
            m_BoundingSphere = CalculateBoundingSphere();

            switch (PositionMode)
            {
                case PositionModes.GroupCenter:
                    transform.position = Sphere.position;
                    break;
                case PositionModes.GroupAverage:
                    transform.position = m_AveragePos;
                    break;
            }

            switch (RotationMode)
            {
                case RotationModes.Manual:
                    break;
                case RotationModes.GroupAverage:
                    transform.rotation = CalculateAverageOrientation();
                    break;
            }
        }

        void UpdateMemberValidity()
        {
            Targets ??= new (); // 防止用户将其设为 null
            var count = Targets.Count;
            m_ValidMembers.Clear();
            m_ValidMembers.Capacity = Mathf.Max(m_ValidMembers.Capacity, count);
            m_MemberValidity.Clear();
            m_MemberValidity.Capacity = Mathf.Max(m_MemberValidity.Capacity, count);
            m_WeightSum = m_MaxWeight = 0;
            for (int i = 0; i < count; ++i)
            {
                m_MemberValidity.Add(Targets[i].Object != null
                        && Targets[i].Weight > UnityVectorExtensions.Epsilon
                        && Targets[i].Object.gameObject.activeInHierarchy);
                if (m_MemberValidity[i])
                {
                    m_ValidMembers.Add(i);
                    m_MaxWeight = Mathf.Max(m_MaxWeight, Targets[i].Weight);
                    m_WeightSum += Targets[i].Weight;
                }
            }
        }

        // 假设已调用 UpdateMemberValidity()
        Vector3 CalculateAveragePosition()
        {
            if (m_WeightSum < UnityVectorExtensions.Epsilon)
                return transform.position;

            var pos = Vector3.zero;
            var count = m_ValidMembers.Count;
            for (int i = 0; i < count; ++i)
            {
                var targetIndex = m_ValidMembers[i];
                var weight = Targets[targetIndex].Weight;
                pos += TargetPositionCache.GetTargetPosition(Targets[targetIndex].Object) * weight;
            }
            return pos / m_WeightSum;
        }

        // 假设已调用 UpdateMemberValidity()
        Bounds CalculateBoundingBox()
        {
            if (m_MaxWeight < UnityVectorExtensions.Epsilon)
                return m_BoundingBox;
            var b = new Bounds(m_AveragePos, Vector3.zero);
            var count = m_ValidMembers.Count;
            for (int i = 0; i < count; ++i)
            {
                var s = WeightedMemberBoundsForValidMember(Targets[m_ValidMembers[i]], m_AveragePos, m_MaxWeight);
                b.Encapsulate(new Bounds(s.position, s.radius * 2 * Vector3.one));
            }
            return b;
        }

        /// <summary>
        /// 使用 Ritter 算法计算近似边界球。
        /// 假设已调用 UpdateMemberValidity()。
        /// </summary>
        /// <param name="maxWeight">组中成员的最大权重</param>
        /// <returns>近似边界球，会稍大一些。</returns>
        BoundingSphere CalculateBoundingSphere()
        {
            var count = m_ValidMembers.Count;
            if (count == 0 || m_MaxWeight < UnityVectorExtensions.Epsilon)
                return m_BoundingSphere;

            var sphere = WeightedMemberBoundsForValidMember(Targets[m_ValidMembers[0]], m_AveragePos, m_MaxWeight);
            for (int i = 1; i < count; ++i)
            {
                var s = WeightedMemberBoundsForValidMember(Targets[m_ValidMembers[i]], m_AveragePos, m_MaxWeight);
                var distance = (s.position - sphere.position).magnitude + s.radius;
                if (distance > sphere.radius)
                {
                    // 点在当前球外：更新
                    sphere.radius = (sphere.radius + distance) * 0.5f;
                    sphere.position = (sphere.radius * sphere.position + (distance - sphere.radius) * s.position) / distance;
                }
            }
            return sphere;
        }

        // 假设已调用 UpdateMemberValidity()
        Quaternion CalculateAverageOrientation()
        {
            if (m_WeightSum > 0.001f)
            {
                var averageForward = Vector3.zero;
                var averageUp = Vector3.zero;
                var count = m_ValidMembers.Count;
                for (int i = 0; i < count; ++i)
                {
                    var targetIndex = m_ValidMembers[i];
                    var scaledWeight = Targets[targetIndex].Weight / m_WeightSum;
                    var rot = TargetPositionCache.GetTargetRotation(Targets[targetIndex].Object);
                    averageForward += rot * Vector3.forward * scaledWeight;
                    averageUp += rot * Vector3.up * scaledWeight;
                }
                if (averageForward.sqrMagnitude > 0.0001f && averageUp.sqrMagnitude > 0.0001f)
                    return Quaternion.LookRotation(averageForward, averageUp);
            }
            return transform.rotation;
        }

        void FixedUpdate()
        {
            if (UpdateMethod == UpdateMethods.FixedUpdate)
                DoUpdate();
        }

        void Update()
        {
            if (!Application.isPlaying || UpdateMethod == UpdateMethods.Update)
                DoUpdate();
        }

        void LateUpdate()
        {
            if (UpdateMethod == UpdateMethods.LateUpdate)
                DoUpdate();
        }

        /// <summary>
        /// 从特定视角获取组的局部空间角度边界。
        /// 同时返回成员的 z 深度范围。
        /// 注意，此结果仅在调用 DoUpdate 后有效。如果在该调用后添加或删除成员，
        /// 或成员更改了权重或激活状态，在下次更新前，这不一定返回正确的信息。
        /// </summary>
        /// <param name="observer">计算的视角，返回值将处于该空间中</param>
        /// <param name="minAngles">成员屏幕角度的下限（度）</param>
        /// <param name="maxAngles">成员屏幕角度的上限（度）</param>
        /// <param name="zRange">成员相对于观察者的最小和最大深度值</param>
        public void GetViewSpaceAngularBounds(
            Matrix4x4 observer, out Vector2 minAngles, out Vector2 maxAngles, out Vector2 zRange)
        {
            if (m_LastUpdateFrame != CinemachineCore.CurrentUpdateFrame)
                DoUpdate();
            var world2local = observer;
            if (!Matrix4x4.Inverse3DAffine(observer, ref world2local))
                world2local = observer.inverse;

            var r = m_BoundingSphere.radius;
            var b = new Bounds() { center = world2local.MultiplyPoint3x4(m_AveragePos), extents = new Vector3(r, r, r) };
            zRange = new Vector2(b.center.z - r, b.center.z + r);
            if (CachedCountIsValid)
            {
                bool haveOne = false;
                var count = m_ValidMembers.Count;
                for (int i = 0; i < count; ++i)
                {
                    var s = WeightedMemberBoundsForValidMember(Targets[m_ValidMembers[i]], m_AveragePos, m_MaxWeight);
                    var p = world2local.MultiplyPoint3x4(s.position);
                    if (p.z < UnityVectorExtensions.Epsilon)
                        continue; // 在我们后方

                    var rN = s.radius / p.z;
                    var rN2 = new Vector3(rN, rN, 0);
                    var pN = p / p.z;
                    if (!haveOne)
                    {
                        b.center = pN;
                        b.extents = rN2;
                        zRange = new Vector2(p.z, p.z);
                        haveOne = true;
                    }
                    else
                    {
                        b.Encapsulate(pN + rN2);
                        b.Encapsulate(pN - rN2);
                        zRange.x = Mathf.Min(zRange.x, p.z);
                        zRange.y = Mathf.Max(zRange.y, p.z);
                    }
                }
            }
            // 不需要 UnityVectorExtensions.SignedAngle 的高精度版本
            var pMin = b.min;
            var pMax = b.max;
            minAngles = new Vector2(
                Vector3.SignedAngle(Vector3.forward, new Vector3(0, pMax.y, 1), Vector3.right),
                Vector3.SignedAngle(Vector3.forward, new Vector3(pMin.x, 0, 1), Vector3.up));
            maxAngles = new Vector2(
                Vector3.SignedAngle(Vector3.forward, new Vector3(0, pMin.y, 1), Vector3.right),
                Vector3.SignedAngle(Vector3.forward, new Vector3(pMax.x, 0, 1), Vector3.up));
        }
    }
}