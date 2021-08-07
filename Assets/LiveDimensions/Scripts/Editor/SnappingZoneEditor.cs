using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEditor;
using UdonSharpEditor;
#endif

namespace LiveDimensions.SnappingZone
{
    #if !COMPILER_UDONSHARP && UNITY_EDITOR
    [CustomEditor(typeof(SnappingZone))]
    public class SnappingZoneEditor : Editor
    {
        protected virtual void OnSceneGUI()
        {
            EditorGUI.BeginChangeCheck();

            SnappingZone snappingZone = ((SnappingZone)target);

            if (!snappingZone.customSnapLocation) return;

            Vector3 newPosition = Handles.PositionHandle(snappingZone.transform.TransformPoint(snappingZone.customSnapPosition), snappingZone.customSnapRotation);
            Quaternion newRotation = Handles.RotationHandle(snappingZone.customSnapRotation, snappingZone.transform.TransformPoint(snappingZone.customSnapPosition));

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(snappingZone, "Target Snap Moved/Rotated");
                snappingZone.customSnapPosition = snappingZone.transform.InverseTransformPoint(newPosition);
                snappingZone.customSnapRotation = newRotation;
            }
        }

        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
            DrawInspectorTitle();

            SnappingZone snappingZone = ((SnappingZone)target);

            EditorGUI.BeginChangeCheck();

            bool customSnapLocation = GUILayout.Toggle(snappingZone.customSnapLocation, "Custom Snap Location", "Button");

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(snappingZone, "Toggle custom snap");
                snappingZone.customSnapLocation = customSnapLocation;
            }

            //Show custom snap positions and rotations
            if (snappingZone.customSnapLocation)
            {
                EditorGUI.BeginChangeCheck();
                //Custom Snap Postion
                Vector3 editorPosition = EditorGUILayout.Vector3Field("Custom Snap Position", snappingZone.customSnapPosition);

                //Custom Snap Rotation
                Vector3 editorRotation = EditorGUILayout.Vector3Field("Custom Snap Rotation", snappingZone.customSnapRotation.eulerAngles);

                //Repaint editor scene to update handles.
                SceneView.lastActiveSceneView.Repaint();

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(snappingZone, "Location inspector");
                    snappingZone.customSnapPosition = editorPosition;
                    snappingZone.customSnapRotation = Quaternion.Euler(editorRotation);
                }
            }

            if (snappingZone.customSnapLocation == snappingZone.lastCustomSnapLocation) return;

            snappingZone.lastCustomSnapLocation = snappingZone.customSnapLocation;

            if (snappingZone.customSnapLocation)
            {
                snappingZone.customSnapPosition = snappingZone.transform.position + new Vector3(0, 0.5f, 0);
                snappingZone.customSnapRotation = snappingZone.transform.rotation;
            }

        }

        private void DrawInspectorTitle()
        {
            GUIStyle titleStyle = new GUIStyle();
            titleStyle.fontSize = 16;
            titleStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.gray;

            GUIStyle subtitleStyle = new GUIStyle();
            subtitleStyle.fontSize = 12;
            subtitleStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.gray : Color.gray;

            GUILayout.Label("Snapping Zone", titleStyle);
            GUILayout.Label("by LiveDimensions", subtitleStyle);

            GUILayout.Space(6);
            GUILayout.Box("Any VRC_Pickups that enter the trigger will be snapped to place, if you want to set a custom postion and rotation please enable and adjust it below.");
        }
    }
    #endif
}