#if !CINEMACHINE_NO_CM2_SUPPORT
using System;
using UnityEngine;

namespace Unity.Cinemachine
{
    /// <summary>
    /// This is a deprecated component.  Use InputAxis instead.
    /// </summary>
    [Obsolete("AxisBase has been deprecated. Use InputAxis instead.")]
    [Serializable]
    public struct AxisBase
    {
        /// <summary>轴的当前值</summary>
        [NoSaveDuringPlay]
        [Tooltip("轴的当前值。")]
        public float m_Value;

        /// <summary>轴的最小值</summary>
        [Tooltip("轴的最小值")]
        public float m_MinValue;

        /// <summary>轴的最大值</summary>
        [Tooltip("轴的最大值")]
        public float m_MaxValue;

        /// <summary>如果勾选，轴将在最小值/最大值处循环环绕</summary>
        [Tooltip("如果勾选，轴将在最小值/最大值处循环环绕")]
        public bool m_Wrap;

        /// <summary>
        /// 从OnValidate()调用此方法以验证此结构的字段（应用钳制等）。
        /// </summary>
        public void Validate()
        {
            m_MaxValue = Mathf.Clamp(m_MaxValue, m_MinValue, m_MaxValue);
        }
        }

        /// <summary>
        /// 这是一个已弃用的组件。请改用DefaultInputAxisDriver。
        /// </summary>
        [Obsolete("CinemachineInputAxisDriver已被弃用。请改用DefaultInputAxisDriver。")]
        [Serializable]
        public struct CinemachineInputAxisDriver
        {
            /// <summary>在处理前将输入值乘以该系数。控制输入强度</summary>
            [Tooltip("在处理前将输入值乘以该系数。控制输入强度。")]
            public float multiplier;

            /// <summary>加速到更高速度所需的时间（秒）</summary>
            [Tooltip("加速到更高速度所需的时间（秒）")]
            public float accelTime;

            /// <summary>减速到较低速度所需的时间（秒）</summary>
            [Tooltip("减速到较低速度所需的时间（秒）")]
            public float decelTime;

            /// <summary>在Unity输入管理器中指定的此轴的名称。
            /// 设置为空字符串将禁用此轴的自动更新</summary>
            [Tooltip("在Unity输入管理器中指定的此轴的名称。"
                + "设置为空字符串将禁用此轴的自动更新")]
            public string name;

            /// <summary>输入轴的值。值为0表示无输入。您可以从自定义输入系统
            /// 直接驱动此值，或者设置Axis Name并让内部输入管理器驱动该值</summary>
            [NoSaveDuringPlay]
            [Tooltip("输入轴的值。值为0表示无输入。您可以从自定义输入系统"
                + "直接驱动此值，或者设置Axis Name并让内部输入管理器驱动该值")]
            public float inputValue;


        /// Internal state
        private float mCurrentSpeed;
        const float Epsilon =  UnityVectorExtensions.Epsilon;

        /// <summary>Call from OnValidate: Make sure the fields are sensible</summary>
        public void Validate()
        {
            accelTime = Mathf.Max(0, accelTime);
            decelTime = Mathf.Max(0, decelTime);
        }

        /// <summary>Update the axis</summary>
        /// <param name="deltaTime">current deltaTime</param>
        /// <param name="axis">The AxisState to update</param>
        /// <returns>True if the axis value changed due to user input, false otherwise</returns>
        public bool Update(float deltaTime, ref AxisBase axis)
        {
            if (!string.IsNullOrEmpty(name))
            {
                try { inputValue = CinemachineCore.GetInputAxis(name); }
                catch (ArgumentException) {}
                //catch (ArgumentException e) { Debug.LogError(e.ToString()); }
            }

            float input = inputValue * multiplier;
            if (deltaTime < Epsilon)
                mCurrentSpeed = 0;
            else
            {
                float speed = input / deltaTime;
                float dampTime = Mathf.Abs(speed) < Mathf.Abs(mCurrentSpeed) ? decelTime : accelTime;
                speed = mCurrentSpeed + Damper.Damp(speed - mCurrentSpeed, dampTime, deltaTime);
                mCurrentSpeed = speed;

                // Decelerate to the end points of the range if not wrapping
                float range = axis.m_MaxValue - axis.m_MinValue;
                if (!axis.m_Wrap && decelTime > Epsilon && range > Epsilon)
                {
                    float v0 = ClampValue(ref axis, axis.m_Value);
                    float v = ClampValue(ref axis, v0 + speed * deltaTime);
                    float d = (speed > 0) ? axis.m_MaxValue - v : v - axis.m_MinValue;
                    if (d < (0.1f * range) && Mathf.Abs(speed) > Epsilon)
                        speed = Damper.Damp(v - v0, decelTime, deltaTime) / deltaTime;
                }
                input = speed * deltaTime;
            }

            axis.m_Value = ClampValue(ref axis, axis.m_Value + input);
            return Mathf.Abs(inputValue) > Epsilon;
        }


        /// <summary>Support for legacy AxisState struct: update the axis</summary>
        /// <param name="deltaTime">current deltaTime</param>
        /// <param name="axis">The AxisState to update</param>
        /// <returns>True if the axis value changed due to user input, false otherwise</returns>
        public bool Update(float deltaTime, ref AxisState axis)
        {
            var a = new AxisBase
            {
                m_Value = axis.Value,
                m_MinValue = axis.m_MinValue,
                m_MaxValue = axis.m_MaxValue,
                m_Wrap = axis.m_Wrap
            };
            bool changed = Update(deltaTime, ref a);
            axis.Value = a.m_Value;
            return changed;
        }

        float ClampValue(ref AxisBase axis, float v)
        {
            float r = axis.m_MaxValue - axis.m_MinValue;
            if (axis.m_Wrap && r > Epsilon)
            {
                v = (v - axis.m_MinValue) % r;
                v += axis.m_MinValue + ((v < 0) ? r : 0);
            }
            return Mathf.Clamp(v, axis.m_MinValue, axis.m_MaxValue);
        }
    }
}
#endif
