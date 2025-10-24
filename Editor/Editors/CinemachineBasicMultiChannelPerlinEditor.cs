using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Cinemachine.Editor
{
    [CustomEditor(typeof(CinemachineBasicMultiChannelPerlin))]
    [CanEditMultipleObjects]
    class CinemachineBasicMultiChannelPerlinEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var ux = new VisualElement();
            this.AddMissingCmCameraHelpBox(ux);

            var noProfile = ux.AddChild(new HelpBox(
                "需要一个噪点配置文件。您可以从项目中已定义的噪点设置（NoiseSettings）资源中选择，"
                + "也可以从预设中选择一个。",
                HelpBoxMessageType.Warning));

            var perlin = target as CinemachineBasicMultiChannelPerlin;
            var profileProp = serializedObject.FindProperty(() => perlin.NoiseProfile);
            InspectorUtility.AddRemainingProperties(ux, profileProp);

            var row = ux.AddChild(new InspectorUtility.LeftRightRow());
            row.Right.Add(new Button(() =>
            {
                for (int i = 0; i < targets.Length; ++i)
                    (targets[i] as CinemachineBasicMultiChannelPerlin).ReSeed();
            }) { text = "新随机种子", style = { flexGrow = 0 }});

            ux.TrackPropertyWithInitialCallback(profileProp, (p) => noProfile.SetVisible(p.objectReferenceValue == null));

            return ux;
        }
    }
}
