using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Cinemachine.Editor
{
    [CustomEditor(typeof(CinemachineImpulseListener))]
    [CanEditMultipleObjects]
    class CinemachineImpulseListenerEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var ux = new VisualElement();

            this.AddMissingCmCameraHelpBox(ux);

            if (!(target as CinemachineImpulseListener).TryGetComponent<CinemachineVirtualCameraBase>(out _))
            {
                ux.Add(new HelpBox(
                    "若要为非 Cinemachine 相机的游戏对象添加冲力监听功能，"
                        + "您可以改用 <b>Cinemachine 外部冲力监听器</b> 行为组件。",
                    HelpBoxMessageType.Info));
            }
            else
            {
                ux.Add(new HelpBox(
                    "冲力监听器（Impulse Listener）会响应由任意冲力源（CinemachineImpulseSource）广播的信号。", 
                    HelpBoxMessageType.Info));
            }
            var prop = serializedObject.GetIterator();
            if (prop.NextVisible(true))
                InspectorUtility.AddRemainingProperties(ux, prop);
            return ux;
        }
    }
}
