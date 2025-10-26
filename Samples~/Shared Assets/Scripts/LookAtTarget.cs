using UnityEngine;

namespace Unity.Cinemachine.Samples
{
    /// <summary>
    /// Orients the GameObject that this script is attached in such a way that it always faces the Target.
    /// </summary>
    [ExecuteAlways]
    public class LookAtTarget : MonoBehaviour
    {
        [Tooltip("要注视的目标。")]
        public Transform Target;

        [Tooltip("将 X 轴旋转锁定为初始值。")]
        public bool LockRotationX;
        [Tooltip("将 Y 轴旋转锁定为初始值。")]
        public bool LockRotationY;
        [Tooltip("将 Z 轴旋转锁定为初始值。")]
        public bool LockRotationZ;

        Vector3 m_Rotation;

        void OnEnable()
        {
            m_Rotation = transform.rotation.eulerAngles;
        }

        void Reset()
        {
            m_Rotation = transform.rotation.eulerAngles;
        }

        void Update()
        {
            if (Target != null)
            {
                var direction = Target.position - transform.position;
                transform.rotation = Quaternion.LookRotation(direction);

                if (LockRotationX || LockRotationY || LockRotationZ)
                {
                    var euler = transform.rotation.eulerAngles;
                    euler.x = LockRotationX ? m_Rotation.x : euler.x;
                    euler.y = LockRotationY ? m_Rotation.y : euler.y;
                    euler.z = LockRotationZ ? m_Rotation.z : euler.z;
                    transform.rotation = Quaternion.Euler(euler);
                }
            }
        }
    }
}
