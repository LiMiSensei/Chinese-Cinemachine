using System;
using UnityEngine;
using UnityEngine.Splines;

namespace Unity.Cinemachine.Samples
{
    [Serializable]
    public class RandomizedDollySpeed : SplineAutoDolly.ISplineAutoDolly
    {
        [Tooltip("矿车的最低行驶速度")]
        public float MinSpeed = 2;
        [Tooltip("矿车的最高行驶速度")]
        public float MaxSpeed = 10;
        [Tooltip("矿车改变速度的快慢程度")]
        public float Acceleration = 1;

        float m_Speed;
        float m_TargetSpeed;

        void SplineAutoDolly.ISplineAutoDolly.Validate() => MaxSpeed = Mathf.Max(MaxSpeed, MinSpeed);
        void SplineAutoDolly.ISplineAutoDolly.Reset() => m_Speed = m_TargetSpeed = (MinSpeed + MaxSpeed) / 2;
        bool SplineAutoDolly.ISplineAutoDolly.RequiresTrackingTarget => false;

        public float GetSplinePosition(
            MonoBehaviour sender, Transform target,
            SplineContainer spline, float currentPosition,
            PathIndexUnit positionUnits, float deltaTime)
        {
            if (Application.isPlaying && deltaTime > 0)
            {
                if (Mathf.Abs(m_Speed - m_TargetSpeed) < 0.01f)
                    m_TargetSpeed = UnityEngine.Random.Range(MinSpeed, MaxSpeed);
                if (m_Speed < m_TargetSpeed)
                    m_Speed = Mathf.Min(m_TargetSpeed, m_Speed + Acceleration * deltaTime);
                if (m_Speed > m_TargetSpeed)
                    m_Speed = Mathf.Max(m_TargetSpeed, m_Speed - Acceleration * deltaTime);

                return currentPosition + m_Speed * deltaTime;
            }
            return currentPosition;
        }
    }
}
