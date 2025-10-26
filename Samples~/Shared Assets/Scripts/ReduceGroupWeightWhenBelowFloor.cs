using System;
using UnityEngine;

namespace Unity.Cinemachine.Samples
{
    [RequireComponent(typeof(CinemachineTargetGroup))]
    public class ReduceGroupWeightWhenBelowFloor : MonoBehaviour
    {
        [Tooltip("计算 LoseSightAtRange 的基准平台")]
        public Transform Floor;

        [Tooltip("当变换位于地板上方时，其在目标组中的权重为1。当变换位于地板下方时，" +
            "其权重会根据变换与地板之间的距离逐渐降低，并在 LoseSightAtRange 处降至0。" +
            "如果将此值设置为0，则当变换位于地板下方时会立即被移除。")]
        [Range(0, 30)]
        public float LoseSightAtRange = 20;

        CinemachineTargetGroup m_TargetGroup;

        void Awake()
        {
            m_TargetGroup = GetComponent<CinemachineTargetGroup>();
        }

        void Update()
        {
            // iterate through each target in the targetGroup
            var floor = Floor.position.y;
            for (int i = 0; i < m_TargetGroup.Targets.Count; ++i)
            {
                var target = m_TargetGroup.Targets[i];

                // calculate the distance between target and the Floor along the Y axis
                var distanceBelow = floor - target.Object.position.y;

                // weight goes to 0 if it's farther below than LoseSightAtRange
                var weight = Mathf.Clamp(1 - distanceBelow / Mathf.Max(0.001f, LoseSightAtRange), 0, 1);
                target.Weight = weight;
            }
        }
    }
}
