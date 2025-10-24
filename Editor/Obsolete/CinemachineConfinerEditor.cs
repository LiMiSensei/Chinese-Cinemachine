#if !CINEMACHINE_NO_CM2_SUPPORT
#if CINEMACHINE_PHYSICS || CINEMACHINE_PHYSICS_2D

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace Unity.Cinemachine.Editor
{
    [System.Obsolete]
    [CustomEditor(typeof(CinemachineConfiner))]
    [CanEditMultipleObjects]
    class CinemachineConfinerEditor : BaseEditor<CinemachineConfiner>
    {
        /// <summary>Get the property names to exclude in the inspector.</summary>
        /// <param name="excluded">Add the names to this list</param>
        protected override void GetExcludedPropertiesInInspector(List<string> excluded)
        {
            base.GetExcludedPropertiesInInspector(excluded);
            CinemachineBrain brain = CinemachineCore.FindPotentialTargetBrain(Target.ComponentOwner);
            bool ortho = brain != null ? brain.OutputCamera.orthographic : false;
            if (!ortho)
                excluded.Add(FieldPath(x => x.m_ConfineScreenEdges));
#if CINEMACHINE_PHYSICS && CINEMACHINE_PHYSICS_2D
            if (Target.m_ConfineMode == CinemachineConfiner.Mode.Confine2D)
                excluded.Add(FieldPath(x => x.m_BoundingVolume));
            else
                excluded.Add(FieldPath(x => x.m_BoundingShape2D));
#endif
        }

        public override void OnInspectorGUI()
        {
            BeginInspector();
            this.IMGUI_DrawMissingCmCameraHelpBox();

#if CINEMACHINE_PHYSICS && CINEMACHINE_PHYSICS_2D
            if (Target.m_ConfineMode == CinemachineConfiner.Mode.Confine2D)
            {
#endif
#if CINEMACHINE_PHYSICS_2D
                if (Target.m_BoundingShape2D == null)
                    EditorGUILayout.HelpBox("需要一个边界形状", MessageType.Warning);
                else if (Target.m_BoundingShape2D.GetType() != typeof(PolygonCollider2D)
                    && Target.m_BoundingShape2D.GetType() != typeof(CompositeCollider2D))
                {
                    EditorGUILayout.HelpBox(
                        "必须是多边形碰撞器 2D（PolygonCollider2D）或复合碰撞器 2D（CompositeCollider2D）。",
                        MessageType.Warning);
                }
                else if (Target.m_BoundingShape2D.GetType() == typeof(CompositeCollider2D))
                {
                    CompositeCollider2D poly = Target.m_BoundingShape2D as CompositeCollider2D;
                    if (poly.geometryType != CompositeCollider2D.GeometryType.Polygons)
                    {
                        EditorGUILayout.HelpBox(
                            "复合碰撞器 2D（CompositeCollider2D）的几何类型必须为多边形（Polygons）。",
                            MessageType.Warning);
                    }
                }
#endif
#if CINEMACHINE_PHYSICS && CINEMACHINE_PHYSICS_2D
            }
            else
            {
#endif
#if CINEMACHINE_PHYSICS
                if (Target.m_BoundingVolume == null)
                    EditorGUILayout.HelpBox("需要一个边界体积", MessageType.Warning);
                else if (Target.m_BoundingVolume.GetType() != typeof(BoxCollider)
                    && Target.m_BoundingVolume.GetType() != typeof(SphereCollider)
                    && Target.m_BoundingVolume.GetType() != typeof(CapsuleCollider))
                {
                    EditorGUILayout.HelpBox(
                        "必须是盒体碰撞器（BoxCollider）、球体碰撞器（SphereCollider）或胶囊体碰撞器（CapsuleCollider）。",
                        MessageType.Warning);
                }
#endif
#if CINEMACHINE_PHYSICS && CINEMACHINE_PHYSICS_2D
            }
#endif
            DrawRemainingPropertiesInInspector();
        }
    }
}
#endif
#endif
