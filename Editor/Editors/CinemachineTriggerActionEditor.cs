#if CINEMACHINE_PHYSICS || CINEMACHINE_PHYSICS_2D
using UnityEngine;
using UnityEditor;
using UnityEngine.Playables;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Unity.Cinemachine.Editor
{
    [CustomEditor(typeof(CinemachineTriggerAction))]
    class CinemachineTriggerActionEditor : UnityEditor.Editor
    {
        CinemachineTriggerAction Target => target as CinemachineTriggerAction;

        public override VisualElement CreateInspectorGUI()
        {
            var ux = new VisualElement();

            ux.Add(new PropertyField(serializedObject.FindProperty(() => Target.LayerMask)));
            ux.Add(new PropertyField(serializedObject.FindProperty(() => Target.WithTag)));
            ux.Add(new PropertyField(serializedObject.FindProperty(() => Target.WithoutTag)));

            var row = ux.AddChild(InspectorUtility.PropertyRow(serializedObject.FindProperty(() => Target.SkipFirst), out _));
            var repeatingProp = serializedObject.FindProperty(() => Target.Repeating);
            row.Contents.AddChild(new Toggle { text = repeatingProp.displayName, style = { marginLeft = 5 }}).BindProperty(repeatingProp);

            AddActionSettings(ux, serializedObject.FindProperty(() => Target.OnObjectEnter));
            AddActionSettings(ux, serializedObject.FindProperty(() => Target.OnObjectExit));

            return ux;
        }

        void AddActionSettings(VisualElement ux, SerializedProperty property)
        {
            CinemachineTriggerAction.ActionSettings def = new(); // to access name strings

            ux.AddSpace();
            var foldout = ux.AddChild(new Foldout { text = $"<b>{property.displayName}</b>" });
            foldout.BindProperty(property);

            var actionProp = property.FindPropertyRelative(() => def.Action);
            foldout.Add(new PropertyField(actionProp));

            var targetProp = property.FindPropertyRelative(() => def.Target);
            var target = foldout.AddChild(new PropertyField(targetProp));

            var boostAmount = foldout.AddChild(new PropertyField(property.FindPropertyRelative(() => def.BoostAmount)));

#if CINEMACHINE_TIMELINE
            var timelineRow = foldout.AddChild(InspectorUtility.PropertyRow(property.FindPropertyRelative(() => def.StartTime), out _));
            timelineRow.Contents.AddChild(new Label { text = " s", tooltip = "Seconds", style = { alignSelf = Align.Center }});
            timelineRow.Contents.AddChild(new PropertyField(
                property.FindPropertyRelative(() => def.Mode), "") { style = { flexGrow = 1 }});
#endif
            var row = foldout.AddChild(new InspectorUtility.LeftRightRow());
            var helpMessage = row.Right.AddChild(new HelpBox("帮助文本", HelpBoxMessageType.Warning));
            foldout.Add(new PropertyField(property.FindPropertyRelative(() => def.Event)) { style = { marginTop = 5 }});

            foldout.TrackAnyUserActivity(() =>
            {
                if (Target == null)
                    return; // object deleted
                var targetObject = targetProp.objectReferenceValue;
                var action = (CinemachineTriggerAction.ActionSettings.ActionModes)actionProp.intValue;

                bool isEventOnly = action == CinemachineTriggerAction.ActionSettings.ActionModes.EventOnly;
                target.SetVisible(!isEventOnly);

                string helpText = isEventOnly || targetObject != null
                    ? string.Empty : "由于目标无效，将不执行任何操作";

                boostAmount.SetVisible(action == CinemachineTriggerAction.ActionSettings.ActionModes.PriorityBoost);
#if CINEMACHINE_TIMELINE
                bool isTimeline = action == CinemachineTriggerAction.ActionSettings.ActionModes.Play
                    || action == CinemachineTriggerAction.ActionSettings.ActionModes.Stop;
                timelineRow.SetVisible(isTimeline);
                if (isTimeline && !HasComponent<Animator>(targetObject) && !HasComponent<PlayableDirector>(targetObject))
                    helpText = "目标必须包含 PlayableDirector（可播放导演）或 Animator（动画器）组件才能执行播放 / 停止操作";
#endif
                if (string.IsNullOrEmpty(helpText))
                {
                    switch (action)
                    {
                        default: break;
                        case CinemachineTriggerAction.ActionSettings.ActionModes.PriorityBoost:
                            if (!HasComponent<CinemachineVirtualCameraBase>(targetObject))
                                helpText = "目标必须是 Cinemachine 相机才能提升优先级";
                            break;
                        case CinemachineTriggerAction.ActionSettings.ActionModes.Enable:
                        case CinemachineTriggerAction.ActionSettings.ActionModes.Disable:
                            if (targetObject is not Behaviour)
                                helpText = "目标必须是 Behaviour（行为）组件才能执行启用 / 禁用操作";
                            break;
                    }
                }
                helpMessage.text = helpText;
                helpMessage.SetVisible(!string.IsNullOrEmpty(helpText));
            });

            static bool HasComponent<T>(Object obj) where T : Component
            {
                if (obj is T)
                    return true;
                var go = obj as GameObject;
                if (go == null && obj is Component c)
                    go = c.gameObject;
                if (go != null)
                    return go.TryGetComponent<T>(out _);
                return false;
            }
        }
    }
}
#endif
