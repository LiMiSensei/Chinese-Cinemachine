using UnityEditor;
using UnityEditor.Splines;
using UnityEditor.UIElements;
using UnityEngine.Splines;
using UnityEngine.UIElements;

namespace Unity.Cinemachine.Editor
{
    [CustomEditor(typeof(CinemachineSplineSmoother))]
    class CinemachineSplineSmootherEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var ux = new VisualElement();
            ux.Add(new HelpBox("样条曲线平滑器会调整样条曲线的节点设置以保持平滑度，适用于相机路径。"
                + "请勿不要手动不要手动调整切线或节点模式，它们会被平滑器覆盖。\n"
                + "若要手动调整切线和节点设置，请禁用或移除此行为组件。",
            HelpBoxMessageType.Info));

            var autoSmoothProp = serializedObject.FindProperty(nameof(CinemachineSplineSmoother.AutoSmooth));
            ux.Add(new PropertyField(autoSmoothProp));
            ux.TrackPropertyValue(autoSmoothProp, (p) =>
            {
                if (p.boolValue)
                {
                    var smoother = target as CinemachineSplineSmoother;
                    if (smoother != null && smoother.enabled)
                        SmoothSplineNow(smoother);
                }
            });

            ux.Add(new Button(() => SmoothSplineNow(target as CinemachineSplineSmoother)) { text = "立即平滑样条曲线" });
            return ux;
        }

        static void SmoothSplineNow(CinemachineSplineSmoother smoother)
        {
            if (smoother != null && smoother.TryGetComponent(out SplineContainer container))
            {
                Undo.RecordObject(container, "Smooth Spline");
                smoother.SmoothSplineNow();
            }
        }

        [InitializeOnLoad]
        static class AutoSmoothHookup
        {
            static AutoSmoothHookup()
            {
                CinemachineSplineSmoother.OnEnableCallback += OnSmootherEnable;
                CinemachineSplineSmoother.OnDisableCallback += OnSmootherDisable;
            }

            static void OnSmootherEnable(CinemachineSplineSmoother smoother)
            {
                EditorSplineUtility.AfterSplineWasModified += smoother.OnSplineModified;
                if (smoother.AutoSmooth)
                    SmoothSplineNow(smoother);
            }
            static void OnSmootherDisable(CinemachineSplineSmoother smoother) => EditorSplineUtility.AfterSplineWasModified -= smoother.OnSplineModified;
        }
    }
}
