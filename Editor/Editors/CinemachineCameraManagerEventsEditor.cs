using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Unity.Cinemachine.Editor
{
    [CustomEditor(typeof(CinemachineCameraManagerEvents))]
    [CanEditMultipleObjects]
    class CinemachineCameraManagerEventsEditor : UnityEditor.Editor
    {
        CinemachineCameraManagerEvents Target => target as CinemachineCameraManagerEvents;

        public override VisualElement CreateInspectorGUI()
        {
            var ux = new VisualElement();
            ux.Add(new PropertyField(serializedObject.FindProperty(() => Target.CameraManager),"相机管理器"));
            ux.Add(new PropertyField(serializedObject.FindProperty(() => Target.CameraActivatedEvent),"相机激活事件"));
            ux.Add(new PropertyField(serializedObject.FindProperty(() => Target.CameraDeactivatedEvent),"相机停用事件"));
            ux.Add(new PropertyField(serializedObject.FindProperty(() => Target.BlendCreatedEvent),"混合创建事件"));
            ux.Add(new PropertyField(serializedObject.FindProperty(() => Target.BlendFinishedEvent),"混合完成事件"));
            ux.Add(new PropertyField(serializedObject.FindProperty(() => Target.CameraCutEvent),"相机切换事件"));
            return ux;
        }
    }
}
