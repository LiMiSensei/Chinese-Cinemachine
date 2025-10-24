using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cinemachine.Editor
{
    [CustomEditor(typeof(CinemachineClearShot))]
    [CanEditMultipleObjects]
    class CinemachineClearShotEditor : CinemachineVirtualCameraBaseEditor
    {
        CinemachineClearShot Target => target as CinemachineClearShot;
        EvaluatorState m_EvaluatorState;

        static string GetAvailableQualityEvaluatorNames()
        {
            var names = InspectorUtility.GetAssignableBehaviourNames(typeof(IShotQualityEvaluator));
            if (names == InspectorUtility.s_NoneString)
                return "没有可用的镜头质量评估器扩展。"
                    + "这可能是因为物理模块已禁用，"
                    + "而所有镜头质量评估器的实现都依赖于物理射线检测。";
            return "可用的镜头质量评估器如下：" + names;
        }

        protected override void AddInspectorProperties(VisualElement ux)
        {
            ux.AddHeader("全局设置");
            this.AddGlobalControls(ux);

            var helpBox = ux.AddChild(new HelpBox());

            ux.AddHeader("清晰视角");
            ux.Add(new PropertyField(serializedObject.FindProperty(() => Target.DefaultTarget)));
            ux.Add(new PropertyField(serializedObject.FindProperty(() => Target.ActivateAfter),"激活之后"));
            ux.Add(new PropertyField(serializedObject.FindProperty(() => Target.MinDuration),"最短持续时间"));
            ux.Add(new PropertyField(serializedObject.FindProperty(() => Target.RandomizeChoice),"随机选择"));
            ux.Add(new PropertyField(serializedObject.FindProperty(() => Target.DefaultBlend),"默认混合"));
            ux.Add(new PropertyField(serializedObject.FindProperty(() => Target.CustomBlends)));

            ux.TrackAnyUserActivity(() =>
            {
                if (Target == null)
                    return; // object deleted
                m_EvaluatorState = GetEvaluatorState();
                switch (m_EvaluatorState)
                {
                    case EvaluatorState.EvaluatorOnParent:
                    case EvaluatorState.EvaluatorOnAllChildren:
                        helpBox.SetVisible(false);
                        break;
                    case EvaluatorState.NoEvaluator:
                        helpBox.text = "清晰视角（ClearShot）需要镜头质量评估器扩展来对镜头进行评级。"
                            + "您可以向清晰视角（ClearShot）本身添加一个，也可以向每个子相机添加。"
                            + GetAvailableQualityEvaluatorNames();
                        helpBox.messageType = HelpBoxMessageType.Warning;
                        helpBox.SetVisible(true);
                        break;
                    case EvaluatorState.EvaluatorOnSomeChildren:
                        helpBox.text = "部分子相机没有镜头质量评估器扩展。"
                            + "清晰视角（ClearShot）要求所有子相机上都有镜头质量评估器，"
                            + "或者也可以仅在清晰视角（ClearShot）本身添加。"
                            + GetAvailableQualityEvaluatorNames();
                        helpBox.messageType = HelpBoxMessageType.Warning;
                        helpBox.SetVisible(true);
                        break;
                    case EvaluatorState.EvaluatorOnChildrenAndParent:
                        helpBox.text = "清晰视角（ClearShot）相机上有一个镜头质量评估器扩展，且其部分子相机上也有。"
                            + "这种两者皆有的情况是不允许的。"
                            + GetAvailableQualityEvaluatorNames();
                        helpBox.messageType = HelpBoxMessageType.Error;
                        helpBox.SetVisible(true);
                        break;
                }
            });
        }

        enum EvaluatorState
        {
            NoEvaluator,
            EvaluatorOnAllChildren,
            EvaluatorOnSomeChildren,
            EvaluatorOnParent,
            EvaluatorOnChildrenAndParent
        }

        EvaluatorState GetEvaluatorState()
        {
            int numEvaluatorChildren = 0;
            bool colliderOnParent = ObjectHasEvaluator(Target);

            var children = Target.ChildCameras;
            var numChildren = children == null ? 0 : children.Count;
            for (var i = 0; i < numChildren; ++i)
                if (ObjectHasEvaluator(children[i]))
                    ++numEvaluatorChildren;
            if (colliderOnParent)
                return numEvaluatorChildren > 0
                    ? EvaluatorState.EvaluatorOnChildrenAndParent : EvaluatorState.EvaluatorOnParent;
            if (numEvaluatorChildren > 0)
                return numEvaluatorChildren == numChildren
                    ? EvaluatorState.EvaluatorOnAllChildren : EvaluatorState.EvaluatorOnSomeChildren;
            return EvaluatorState.NoEvaluator;
        }

        bool ObjectHasEvaluator(object obj)
        {
            var vcam = obj as CinemachineVirtualCameraBase;
            if (vcam != null && vcam.TryGetComponent<IShotQualityEvaluator>(out var evaluator))
            {
                var b = evaluator as MonoBehaviour;
                return b != null && b.enabled;
            }
            return false;
        }

        string GetChildWarningMessage(object obj)
        {
            if (m_EvaluatorState == EvaluatorState.EvaluatorOnSomeChildren
                || m_EvaluatorState == EvaluatorState.EvaluatorOnChildrenAndParent)
            {
                bool hasEvaluator = ObjectHasEvaluator(obj);
                if (m_EvaluatorState == EvaluatorState.EvaluatorOnSomeChildren && !hasEvaluator)
                    return "This camera has no shot quality evaluator";
                if (m_EvaluatorState == EvaluatorState.EvaluatorOnChildrenAndParent && hasEvaluator)
                    return "There are multiple shot quality evaluators on this camera";
            }
            return "";
        }
    }
}
