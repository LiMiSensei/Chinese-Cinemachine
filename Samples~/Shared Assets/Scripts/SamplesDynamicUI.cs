using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Unity.Cinemachine.Samples
{
    /// <summary>
    /// Runtime UI script that can create toggles and buttons.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class SamplesDynamicUI : MonoBehaviour
    {
        [Serializable]
public class Item
{
    [Tooltip("按钮上显示的文本")]
    public string Name = "Text";

    [Serializable]
    public class ToggleSettings
    {
        public bool Enabled;

        [Tooltip("开关的初始值")]
        public bool Value;

        [Tooltip("点击开关按钮时发送的事件")]
        public UnityEvent<bool> OnValueChanged = new();
    }

        [Tooltip("设置为true以创建开关按钮")]
        [FoldoutWithEnabledButton]
        public ToggleSettings IsToggle = new();

        [Tooltip("点击按钮时发送的事件")]
        public UnityEvent OnClick = new();
}

        [Tooltip("要显示的按钮")]
        public List<Item> Buttons = new();

        VisualElement m_Root;
        readonly List<VisualElement> m_DynamicElements = new ();

        void OnEnable()
        {
            var uiDocument = GetComponent<UIDocument>();
            m_Root = uiDocument.rootVisualElement.Q("TogglesAndButtons"); // Should be justified - flex grow 1
            if (m_Root == null)
            {
                Debug.LogError("Cannot find TogglesAndButtons.  Is the source asset set in the UIDocument?");
                return;
            }
            for (int i = 0; i < Buttons.Count; ++i)
            {
                var item = Buttons[i];
                if (item.IsToggle.Enabled)
                {
                    var toggle = new Toggle
                    {
                        label = item.Name,
                        value = item.IsToggle.Value,
                        focusable = false,
                    };
                    toggle.AddToClassList("dynamicToggle");
                    toggle.RegisterValueChangedCallback(e => item.IsToggle.OnValueChanged.Invoke(e.newValue));
                    m_Root.Add(toggle);
                    m_DynamicElements.Add(toggle);
                }
                else
                {
                    var button = new Button
                    {
                        text = item.Name,
                        focusable = false
                    };
                    button.AddToClassList("dynamicButton");
                    button.clickable.clicked += item.OnClick.Invoke;
                    m_Root.Add(button);
                    m_DynamicElements.Add(button);
                }
            }
            m_Root.visible = Buttons.Count > 0;
        }

        void OnDisable()
        {
            for (int i = 0; i < m_DynamicElements.Count; ++i)
                m_Root.Remove(m_DynamicElements[i]);
            m_DynamicElements.Clear();
        }
    }
}
