using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Cinemachine.Editor
{
    [CustomEditor(typeof(CinemachinePanTilt))]
    [CanEditMultipleObjects]
    class CinemachinePanTiltEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var ux = new VisualElement();
            this.AddMissingCmCameraHelpBox(ux);
            var prop = serializedObject.GetIterator();
            if (prop.NextVisible(true))
                InspectorUtility.AddRemainingProperties(ux, prop);

            ux.AddSpace();
            this.AddInputControllerHelp(ux, "平移倾斜（PanTilt）没有输入轴控制器行为。");
            return ux;
        }
    }
}
