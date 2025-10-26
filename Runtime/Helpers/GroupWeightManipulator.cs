using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
    /// <summary>
    /// A class to get around the limitation in timeline that array members can't be animated.
    /// A fixed number of slots are made available, rather than a dynamic array.
    /// If you want to add more slots, just modify this code.
    /// </summary>
    [RequireComponent(typeof(CinemachineTargetGroup))]
    [ExecuteAlways]
    [AddComponentMenu("Cinemachine/Helpers/Cinemachine Group Weight Manipulator")]
    [HelpURL(Documentation.BaseURL + "manual/GroupWeightManipulator.html")]
    public class GroupWeightManipulator : MonoBehaviour
    {
        /// <summary>索引 0 处群组成员的权重</summary>
        [Tooltip("索引 0 处群组成员的权重")]
        [FormerlySerializedAs("m_Weight0")]
        public float Weight0 = 1;
        /// <summary>索引 1 处群组成员的权重</summary>
        [Tooltip("索引 1 处群组成员的权重")]
        [FormerlySerializedAs("m_Weight1")]
        public float Weight1 = 1;
        /// <summary>索引 2 处群组成员的权重</summary>
        [Tooltip("索引 2 处群组成员的权重")]
        [FormerlySerializedAs("m_Weight2")]
        public float Weight2 = 1;
        /// <summary>索引 3 处群组成员的权重</summary>
        [Tooltip("索引 3 处群组成员的权重")]
        [FormerlySerializedAs("m_Weight3")]
        public float Weight3 = 1;
        /// <summary>索引 4 处群组成员的权重</summary>
        [Tooltip("索引 4 处群组成员的权重")]
        [FormerlySerializedAs("m_Weight4")]
        public float Weight4 = 1;
        /// <summary>索引 5 处群组成员的权重</summary>
        [Tooltip("索引 5 处群组成员的权重")]
        [FormerlySerializedAs("m_Weight5")]
        public float Weight5 = 1;
        /// <summary>索引 6 处群组成员的权重</summary>
        [Tooltip("索引 6 处群组成员的权重")]
        [FormerlySerializedAs("m_Weight6")]
        public float Weight6 = 1;
        /// <summary>索引 7 处群组成员的权重</summary>
        [Tooltip("索引 7 处群组成员的权重")]
        [FormerlySerializedAs("m_Weight7")]
        public float Weight7 = 1;

        CinemachineTargetGroup m_Group;
        void Start()
        {
            TryGetComponent(out m_Group);
        }

        void OnValidate()
        {
            Weight0 = Mathf.Max(0, Weight0);
            Weight1 = Mathf.Max(0, Weight1);
            Weight2 = Mathf.Max(0, Weight2);
            Weight3 = Mathf.Max(0, Weight3);
            Weight4 = Mathf.Max(0, Weight4);
            Weight5 = Mathf.Max(0, Weight5);
            Weight6 = Mathf.Max(0, Weight6);
            Weight7 = Mathf.Max(0, Weight7);
        }

        void Update()
        {
            if (m_Group != null)
                UpdateWeights();
        }

        void UpdateWeights()
        {
            var targets = m_Group.Targets;
            int last = targets.Count - 1;
            if (last < 0) return; targets[0].Weight = Weight0;
            if (last < 1) return; targets[1].Weight = Weight1;
            if (last < 2) return; targets[2].Weight = Weight2;
            if (last < 3) return; targets[3].Weight = Weight3;
            if (last < 4) return; targets[4].Weight = Weight4;
            if (last < 5) return; targets[5].Weight = Weight5;
            if (last < 6) return; targets[6].Weight = Weight6;
            if (last < 7) return; targets[7].Weight = Weight7;
        }
    }
}
