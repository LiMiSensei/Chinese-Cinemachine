using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Unity.Cinemachine.Editor
{
    [CustomEditor(typeof(CinemachineBrainEvents))]
    [CanEditMultipleObjects]
    class CinemachineBrainEventsEditor : UnityEditor.Editor
    {
        CinemachineBrainEvents Target => target as CinemachineBrainEvents;

        public override VisualElement CreateInspectorGUI()
        {
            var ux = new VisualElement();
            ux.Add(new PropertyField(serializedObject.FindProperty(() => Target.Brain),"大脑"));
            ux.Add(new PropertyField(serializedObject.FindProperty(() => Target.CameraActivatedEvent),"相机激活事件"));
            ux.Add(new PropertyField(serializedObject.FindProperty(() => Target.CameraDeactivatedEvent),"相机停用事件"));
            ux.Add(new PropertyField(serializedObject.FindProperty(() => Target.BlendCreatedEvent),"混合创建事件"));
            ux.Add(new PropertyField(serializedObject.FindProperty(() => Target.BlendFinishedEvent),"混合完成事件"));
            ux.Add(new PropertyField(serializedObject.FindProperty(() => Target.CameraCutEvent),"相机切换事件"));
            ux.AddChild(new PropertyField(serializedObject.FindProperty(() => Target.BrainUpdatedEvent),"大脑更新事件"));
            return ux;
        }
    }
}
