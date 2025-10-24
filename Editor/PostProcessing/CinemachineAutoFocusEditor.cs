using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;

namespace Unity.Cinemachine.Editor
{
    [CustomEditor(typeof(CinemachineAutoFocus))]
    class CinemachineAutoFocusEditor : UnityEditor.Editor
    {
        CinemachineAutoFocus Target => target as CinemachineAutoFocus;

#if CINEMACHINE_HDRP
        const string k_ComputeShaderName = "CinemachineFocusDistanceCompute";
#endif

        public override VisualElement CreateInspectorGUI()
        {
            var ux = new VisualElement();
            this.AddMissingCmCameraHelpBox(ux);
#if CINEMACHINE_HDRP
            ux.AddChild(new HelpBox(
                "注意：对焦控制需要一个激活的Volume（体积），该体积需包含景深（Depth Of Field）覆盖设置，"
                + "且需启用对焦模式（Focus Mode）并设置为物理相机（Physical Camera），"
                + "同时启用对焦距离模式（Focus Distance Mode）并设置为相机（Camera）",
                HelpBoxMessageType.Info));
#else
            ux.AddChild(new HelpBox(
                "注意：对焦控制仅会设置相机（Camera）中的对焦距离（focusDistance）字段。除非相机已设置为物理模式"
                    + "且已安装用于处理该数值的组件，否则此操作不会产生可见效果。",
                HelpBoxMessageType.Info));

            var hdrpOnlyMessage = ux.AddChild(
                new HelpBox("屏幕中心模式（ScreenCenter mode）仅在高清渲染管线（HDRP）项目中有效。", HelpBoxMessageType.Warning));
#endif
            var focusTargetProp = serializedObject.FindProperty(() => Target.FocusTarget);
            ux.Add(new PropertyField(focusTargetProp));
            var customTarget = ux.AddChild(new PropertyField(serializedObject.FindProperty(() => Target.CustomTarget)));
            var offset = ux.AddChild(new PropertyField(serializedObject.FindProperty(() => Target.FocusDepthOffset)));
            var damping = ux.AddChild(new PropertyField(serializedObject.FindProperty(() => Target.Damping)));
#if CINEMACHINE_HDRP
            var radius = ux.AddChild(new PropertyField(serializedObject.FindProperty(() => Target.AutoDetectionRadius)));

            var computeShaderProp = serializedObject.FindProperty("m_ComputeShader");
            var shaderDisplay = ux.AddChild(new PropertyField(computeShaderProp));
            shaderDisplay.SetEnabled(false);

           var importHelp = ux.AddChild(new HelpBox(
                        $"{k_ComputeShaderName}着色器需要导入到项目中。默认情况下，它会被导入到Assets根目录。"
                        + "导入后，您可以将其移至其他位置，但请勿重命名。",
                        HelpBoxMessageType.Warning));
            importHelp.AddButton("Import\nShader", () =>
            {
                // Check if it's already imported, just in case
                var shader = FindShader();
                if (shader == null)
                {
                    // Import the asset from the package
                    var shaderAssetPath = $"Assets/{k_ComputeShaderName}.compute";
                    FileUtil.CopyFileOrDirectory(
                        $"{CinemachineCore.kPackageRoot}/Runtime/PostProcessing/HDRP Resources~/{k_ComputeShaderName}.compute",
                        shaderAssetPath);
                    AssetDatabase.ImportAsset(shaderAssetPath);
                    shader = AssetDatabase.LoadAssetAtPath<ComputeShader>(shaderAssetPath);
                }
                AssignShaderToTarget(shader);
            });
#endif

            ux.TrackPropertyWithInitialCallback(focusTargetProp, (p) =>
            {
                if (p.IsDeletedObject())
                    return;
                var mode = (CinemachineAutoFocus.FocusTrackingMode)p.intValue;
                customTarget.SetVisible(mode == CinemachineAutoFocus.FocusTrackingMode.CustomTarget);
                offset.SetVisible(mode != CinemachineAutoFocus.FocusTrackingMode.None);
                damping.SetVisible(mode != CinemachineAutoFocus.FocusTrackingMode.None);
#if CINEMACHINE_HDRP
                radius.SetVisible(mode == CinemachineAutoFocus.FocusTrackingMode.ScreenCenter);
                shaderDisplay.SetVisible(mode == CinemachineAutoFocus.FocusTrackingMode.ScreenCenter);
                bool importHelpVisible = false;
                if (mode == CinemachineAutoFocus.FocusTrackingMode.ScreenCenter && computeShaderProp.objectReferenceValue == null)
                {
                    var shader = FindShader(); // slow!!!!
                    if (shader != null)
                        AssignShaderToTarget(shader);
                    else
                        importHelpVisible = true;
                }
                importHelp.SetVisible(importHelpVisible);
#else
                hdrpOnlyMessage.SetVisible(mode == CinemachineAutoFocus.FocusTrackingMode.ScreenCenter);
#endif
            });

#if CINEMACHINE_HDRP
            // Make the import box disappear after import
            ux.TrackPropertyWithInitialCallback(computeShaderProp, (p) =>
            {
                if (p.IsDeletedObject())
                    return;
                var mode = (CinemachineAutoFocus.FocusTrackingMode)focusTargetProp.intValue;
                importHelp.SetVisible(mode == CinemachineAutoFocus.FocusTrackingMode.ScreenCenter
                    && p.objectReferenceValue == null);
            });
#endif

            return ux;
        }

#if CINEMACHINE_HDRP
        static ComputeShader FindShader()
        {
            var guids = AssetDatabase.FindAssets($"{k_ComputeShaderName}, t:ComputeShader", new [] { "Assets" });
            if (guids.Length > 0)
                return AssetDatabase.LoadAssetAtPath<ComputeShader>(
                    AssetDatabase.GUIDToAssetPath(guids[0]));
            return null;
        }

        void AssignShaderToTarget(ComputeShader shader)
        {
            for (int i = 0; i < targets.Length; ++i)
            {
                var t = new SerializedObject(targets[i]);
                t.FindProperty("m_ComputeShader").objectReferenceValue = shader;
                t.ApplyModifiedProperties();
            }
        }
#endif
    }
}
