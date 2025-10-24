#if !CINEMACHINE_NO_CM2_SUPPORT
using UnityEditor;
using System.Collections.Generic;

namespace Unity.Cinemachine.Editor
{
    [System.Obsolete]
    [CustomEditor(typeof(CinemachineTransposer))]
    [CanEditMultipleObjects]
    class CinemachineTransposerEditor : BaseEditor<CinemachineTransposer>
    {
        /// <summary>Get the property names to exclude in the inspector.</summary>
        /// <param name="excluded">Add the names to this list</param>
        protected override void GetExcludedPropertiesInInspector(List<string> excluded)
        {
            base.GetExcludedPropertiesInInspector(excluded);

            switch (Target.m_BindingMode)
            {
                default:
                case TargetTracking.BindingMode.LockToTarget:
                    if (Target.m_AngularDampingMode == TargetTracking.AngularDampingMode.Euler)
                        excluded.Add(FieldPath(x => x.m_AngularDamping));
                    else
                    {
                        excluded.Add(FieldPath(x => x.m_PitchDamping));
                        excluded.Add(FieldPath(x => x.m_YawDamping));
                        excluded.Add(FieldPath(x => x.m_RollDamping));
                    }
                    break;
                case TargetTracking.BindingMode.LockToTargetNoRoll:
                    excluded.Add(FieldPath(x => x.m_RollDamping));
                    excluded.Add(FieldPath(x => x.m_AngularDamping));
                    excluded.Add(FieldPath(x => x.m_AngularDampingMode));
                    break;
                case TargetTracking.BindingMode.LockToTargetWithWorldUp:
                    excluded.Add(FieldPath(x => x.m_PitchDamping));
                    excluded.Add(FieldPath(x => x.m_RollDamping));
                    excluded.Add(FieldPath(x => x.m_AngularDamping));
                    excluded.Add(FieldPath(x => x.m_AngularDampingMode));
                    break;
                case TargetTracking.BindingMode.LockToTargetOnAssign:
                case TargetTracking.BindingMode.WorldSpace:
                    excluded.Add(FieldPath(x => x.m_PitchDamping));
                    excluded.Add(FieldPath(x => x.m_YawDamping));
                    excluded.Add(FieldPath(x => x.m_RollDamping));
                    excluded.Add(FieldPath(x => x.m_AngularDamping));
                    excluded.Add(FieldPath(x => x.m_AngularDampingMode));
                    break;
                case TargetTracking.BindingMode.LazyFollow:
                    excluded.Add(FieldPath(x => x.m_XDamping));
                    excluded.Add(FieldPath(x => x.m_PitchDamping));
                    excluded.Add(FieldPath(x => x.m_YawDamping));
                    excluded.Add(FieldPath(x => x.m_RollDamping));
                    excluded.Add(FieldPath(x => x.m_AngularDamping));
                    excluded.Add(FieldPath(x => x.m_AngularDampingMode));
                    break;
            }
            if (Target.HideOffsetInInspector)
                excluded.Add(FieldPath(x => x.m_FollowOffset));
        }

        public override void OnInspectorGUI()
        {
            BeginInspector();
            bool needWarning = false;
            for (int i = 0; !needWarning && i < targets.Length; ++i)
                needWarning = (targets[i] as CinemachineTransposer).FollowTarget == null;
            if (needWarning)
                EditorGUILayout.HelpBox(
                    "移位器（Transposer）需要一个跟踪目标（Tracking Target）。如果不需要跟踪目标，请将位置控制（Position Control）设置为 “无（None）”。",
                    MessageType.Warning);
            DrawRemainingPropertiesInInspector();
        }
    }
}
#endif
