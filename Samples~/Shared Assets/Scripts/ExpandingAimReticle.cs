using System;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Cinemachine.Samples
{
    /// <summary>
    /// Reticle control for when the aiming is inaccurate. Inaccuracy is shown by pulling apart the aim reticle.
    /// </summary>
    public class ExpandingAimReticle : MonoBehaviour
    {
        [Tooltip("瞄准不准确时准星的最大半径。")]
        [Vector2AsRange]
        public Vector2 RadiusRange;

        [Tooltip("瞄准不准确时准星调整所需的时间。")]
        [Range(0, 1f)]
        public float BlendTime;

        [Tooltip("准星的上部部件。")]
        public Image Top;
        [Tooltip("准星的下部部件。")]
        public Image Bottom;
        [Tooltip("准星的左部部件。")]
        public Image Left;
        [Tooltip("准星的右部部件。")]
        public Image Right;

        [Tooltip("此2D对象将在游戏视图中定位在射线投射命中点上方（如果有），"
            + "如果未检测到命中点，则保持在屏幕中心。"
            + "如果为空，则不会显示屏幕指示器。")]
        public RectTransform AimTargetReticle;

        float m_BlendVelocity;
        float m_CurrentRadius;

        void OnValidate()
        {
            RadiusRange.x = Mathf.Clamp(RadiusRange.x, 0, 100);
            RadiusRange.y = Mathf.Clamp(RadiusRange.y, RadiusRange.x, 100);
        }

        void Reset()
        {
            RadiusRange = new Vector2(2.5f, 40f);
            BlendTime = 0.05f;
        }

        void Update()
        {
            var screenCenterPoint = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            float distanceFromCenter = 0;
            if (AimTargetReticle != null)
            {
                var hitPoint = (Vector2)AimTargetReticle.position;
                distanceFromCenter = (screenCenterPoint - hitPoint).magnitude;
            }

            m_CurrentRadius = Mathf.SmoothDamp(m_CurrentRadius, distanceFromCenter * 2f, ref m_BlendVelocity, BlendTime);
            m_CurrentRadius = Mathf.Clamp(m_CurrentRadius, RadiusRange.x, RadiusRange.y);

            Left.rectTransform.position = screenCenterPoint + (Vector2.left * m_CurrentRadius);
            Right.rectTransform.position = screenCenterPoint + (Vector2.right * m_CurrentRadius);
            Top.rectTransform.position = screenCenterPoint + (Vector2.up * m_CurrentRadius);
            Bottom.rectTransform.position = screenCenterPoint + (Vector2.down * m_CurrentRadius);
        }
    }
}
