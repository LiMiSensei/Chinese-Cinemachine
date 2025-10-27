using UnityEngine;
using UnityEditor;

// 自定义红色Header属性
namespace Unity.Cinemachine
{
    public class RedHeaderAttribute : PropertyAttribute
    {
        public readonly string header;

        public RedHeaderAttribute(string header)
        {
            this.header = header;
        }
    }

    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(RedHeaderAttribute))]
    public class RedHeaderDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            RedHeaderAttribute redHeader = attribute as RedHeaderAttribute;
            
            // 设置红色样式
            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.normal.textColor = Color.red;
            style.fontStyle = FontStyle.Bold;
            
            // 计算位置并绘制
            Rect headerRect = new Rect(position.x, position.y, position.width, 20);
            EditorGUI.LabelField(headerRect, redHeader.header, style);
        }
        
        public override float GetHeight()
        {
            return 20;
        }
    }
    #endif
}

