using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using Unity.Cinemachine.Editor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Unity.Cinemachine
{
    [CustomEditor(typeof(CinemachineFreeLookModifier))]
    class CinemachineFreeLookModifierEditor : UnityEditor.Editor
    {
        CinemachineFreeLookModifier Target => target as CinemachineFreeLookModifier;

        public override VisualElement CreateInspectorGUI()
        {
            var ux = new VisualElement();

            var instructionsMsg = ux.AddChild(new HelpBox("此组件为可选组件，若无需使用，可将其移除。"
                + "您添加的修改器将覆盖相机垂直轨道上下两部分的设置。",
                HelpBoxMessageType.Info));

            var invalidSrcMsg = ux.AddChild(
                new HelpBox("由于不存在可修改的目标，此组件将被忽略。\n"
                    + "<b>您可以移除此组件\n</b>可修改的目标包括："
                    + InspectorUtility.GetAssignableBehaviourNames(
                        typeof(CinemachineFreeLookModifier.IModifierValueSource)),
                    HelpBoxMessageType.Warning));

            ux.Add(new PropertyField(serializedObject.FindProperty(nameof(Target.Easing))));

            var controllersProperty = serializedObject.FindProperty(nameof(Target.Modifiers));
            ux.Add(new Label(controllersProperty.displayName) 
                { tooltip = controllersProperty.tooltip , style = { marginLeft = 3, marginTop = 6 }});
            var list = ux.AddChild(new ListView()
            {
                reorderable = false,
                showAddRemoveFooter = true,
                showBorder = true,
                showBoundCollectionSize = false,
                showFoldoutHeader = false,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                style = { marginLeft = 3 }
            });
            list.BindProperty(controllersProperty);

            // Convert the add button to a popup selector
            var button = list.Q<Button>("unity-list-view__add-button");
            var manipulator = new ContextualMenuManipulator((evt) =>
            {
                for (int i = 0; i < ModifierMenuItems.s_ModifierNames.Count; ++i)
                {
                    var type = ModifierMenuItems.s_AllModifiers[i];
                    evt.menu.AppendAction(ModifierMenuItems.s_ModifierNames[i],
                        (action) =>
                        {
                            Undo.RecordObject(Target, "add modifier");
                            var m = (CinemachineFreeLookModifier.Modifier)Activator.CreateInstance(type);
                            m.RefreshCache(Target.ComponentOwner);
                            m.Reset(Target.ComponentOwner);
                            Target.Modifiers.Add(m);
                        },
                        (status) =>
                        {
                            // Enable item if not already present
                            for (int m = 0; m < Target.Modifiers.Count; ++m)
                                if (Target.Modifiers[m] != null && Target.Modifiers[m].GetType() == type)
                                    return DropdownMenuAction.Status.Disabled;
                            return DropdownMenuAction.Status.Normal;
                        });
                }
            });
            manipulator.activators.Clear();
            manipulator.activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
            button.AddManipulator(manipulator);
            button.clickable = null;

            ux.TrackAnyUserActivity(() => 
            {
                if (Target == null)
                    return;
                var hasModifiableSource = Target.HasValueSource();
                var hasModifiers = Target.Modifiers.Count > 0;
                invalidSrcMsg.SetVisible(!hasModifiableSource);
                instructionsMsg.SetVisible(hasModifiableSource && !hasModifiers);
            });

            return ux;
        }

        [CustomPropertyDrawer(typeof(CinemachineFreeLookModifier.Modifier), true)]
        class FreeLookModifierItemPropertyDrawer : PropertyDrawer
        {
            public override VisualElement CreatePropertyGUI(SerializedProperty property)
            {
                if (property.managedReferenceValue is not CinemachineFreeLookModifier.Modifier m)
                    return new Label("invalid item");

                var warningText = "No applicable targets found.  Applicable targets include: "
                    + InspectorUtility.GetAssignableBehaviourNames(m.CachedComponentType);

                var overlay = new VisualElement { style = { flexDirection = FlexDirection.Row, flexGrow = 1 }};
                var warningSymbol = overlay.AddChild(InspectorUtility.MiniHelpIcon(warningText));

                var typeName = ModifierMenuItems.GetModifierName(m.GetType());
                var foldout = new Foldout() { text = typeName, tooltip = property.tooltip };
                foldout.BindProperty(property);

                var noComponentsMsg = foldout.AddChild(new HelpBox(warningText, HelpBoxMessageType.Warning));

                var childProperty = property.Copy();
                var endProperty = childProperty.GetEndProperty();
                childProperty.NextVisible(true);
                while (!SerializedProperty.EqualContents(childProperty, endProperty))
                {
                    foldout.Add(new PropertyField(childProperty));
                    childProperty.NextVisible(false);
                }

                foldout.TrackAnyUserActivity(() =>
                {
                    var showWarning = m != null && !m.HasRequiredComponent;
                    noComponentsMsg.SetVisible(showWarning);
                    warningSymbol.SetVisible(showWarning);
                });

                var ux = new InspectorUtility.FoldoutWithOverlay(foldout, overlay, null);
                ux.style.marginLeft = 12;
                ux.style.marginRight = 6;
                return ux;
            }
        }

        [InitializeOnLoad]
        static class ModifierMenuItems
        {
            public static string GetModifierName(Type type)
            {
                for (int j = 0; j < s_AllModifiers.Count; ++j)
                    if (s_AllModifiers[j] == type)
                        return s_ModifierNames[j];
                return "(none)"; // should never get here
            }

            // These lists are synchronized
            public static List<Type> s_AllModifiers = new ();
            public static List<string> s_ModifierNames = new ();

            // This code dynamically discovers eligible classes and builds the menu data
            static ModifierMenuItems()
            {
                // Get all Modifier types
                var allTypes = ReflectionHelpers.GetTypesDerivedFrom(typeof(CinemachineFreeLookModifier.Modifier),
                    (t) => !t.IsAbstract && t.GetCustomAttribute<ObsoleteAttribute>() == null);

                s_AllModifiers.Clear();
                s_ModifierNames.Clear();
                s_AllModifiers.AddRange(allTypes);
                for (int i = 0; i < s_AllModifiers.Count; ++i)
                {
                    var name = s_AllModifiers[i].Name;
                    var index = name.LastIndexOf("Modifier", StringComparison.OrdinalIgnoreCase);
                    if (index >= 0)
                        name = name.Remove(index);
                    s_ModifierNames.Add(InspectorUtility.NicifyClassName(name));
                }
            }
        }
    }
}

