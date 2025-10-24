using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Cinemachine.Editor
{
    [CustomEditor(typeof(CinemachinePixelPerfect))]
    [CanEditMultipleObjects]
    class CinemachinePixelPerfectEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var ux = new VisualElement();

#if CINEMACHINE_URP || CINEMACHINE_PIXEL_PERFECT_2_0_3
            this.AddMissingCmCameraHelpBox(ux);

            var infoBox = ux.AddChild(new HelpBox(
                "此组件正在驱动 Unity 相机上的 “像素完美相机”（Pixel Perfect Camera）组件。",
                HelpBoxMessageType.Info));
            var helpBox = ux.AddChild(new HelpBox(
                "此组件要求 Unity 相机上必须有激活的 “像素完美相机”（Pixel Perfect Camera）组件。",
                HelpBoxMessageType.Warning));

            ux.TrackAnyUserActivity(() => 
            {
                var pp = target as CinemachinePixelPerfect;
                bool isValid = pp.HasValidPixelPerfectCamera();
                infoBox.SetVisible(isValid && pp.enabled);
                helpBox.SetVisible(!isValid);
            });
#else
            ux.Add(new HelpBox("此组件仅在 URP（通用渲染管线）项目中有效。", HelpBoxMessageType.Warning));
#endif
            return ux;
        }
    }
}
