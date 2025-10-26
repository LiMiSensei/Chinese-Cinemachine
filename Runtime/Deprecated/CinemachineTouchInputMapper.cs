#if !CINEMACHINE_NO_CM2_SUPPORT
using System;
using UnityEngine;

namespace Unity.Cinemachine
{
    /// <summary>
    /// This is an add-on behaviour that globally maps the touch control
    /// to standard input channels, such as mouse X and mouse Y.
    /// Drop it on any game object in your scene.
    /// </summary>
    [Obsolete]
    [AddComponentMenu("")] // Don't display in add component menu
    public class CinemachineTouchInputMapper : MonoBehaviour
    {
        /// <summary>X轴灵敏度乘数</summary>
        [Tooltip("X轴灵敏度乘数")]
        public float TouchSensitivityX = 10f;

        /// <summary>Y轴灵敏度乘数</summary>
        [Tooltip("Y轴灵敏度乘数")]
        public float TouchSensitivityY = 10f;

        /// <summary>X轴模拟输入的输入通道</summary>
        [Tooltip("X轴模拟输入的输入通道")]
        public string TouchXInputMapTo = "Mouse X";

        /// <summary>Y轴模拟输入的输入通道</summary>
        [Tooltip("Y轴模拟输入的输入通道")]
        public string TouchYInputMapTo = "Mouse Y";

        void Start()
        {
            CinemachineCore.GetInputAxis = GetInputAxis;
        }

        private float GetInputAxis(string axisName)
        {
            if (Input.touchCount > 0)
            {
                if (axisName == TouchXInputMapTo)
                    return Input.touches[0].deltaPosition.x / TouchSensitivityX;
                if (axisName == TouchYInputMapTo)
                    return Input.touches[0].deltaPosition.y / TouchSensitivityY;
            }
            return Input.GetAxis(axisName);
        }
    }
}
#endif
