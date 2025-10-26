using System;
using UnityEngine;

namespace Unity.Cinemachine
{
    /// <summary>
    /// Structure for holding the priority of a camera.
    /// </summary>
    [Serializable]
    public struct PrioritySettings
    {
        /// <summary>
        /// 如果为false，将使用默认优先级0。
        /// 如果为true，则Priority字段有效。
        /// </summary>
        [Tooltip("启用此选项以暴露Priority字段")]
        public bool Enabled;

        /// <summary>启用时的优先级值</summary>
        [Tooltip("要使用的优先级值。0为默认值。优先级最高的摄像机会被优先使用。")]
        [SerializeField] int m_Value;

        /// <summary>Priority to use, if Enabled is true</summary>
        public int Value
        {
            readonly get => Enabled ? m_Value : 0;
            set { m_Value = value; Enabled = true; }
        }

        /// <summary> Implicit conversion to int </summary>
        /// <param name="prioritySettings"> The priority settings to convert. </param>
        /// <returns> The value of the priority settings. </returns>
        public static implicit operator int(PrioritySettings prioritySettings) => prioritySettings.Value;

        /// <summary> Implicit conversion from int </summary>
        /// <param name="priority"> The value to initialize the priority settings with. </param>
        /// <returns> A new priority settings with the given priority. </returns>
        public static implicit operator PrioritySettings(int priority) => new () { Value = priority, Enabled = true };
    }
}
