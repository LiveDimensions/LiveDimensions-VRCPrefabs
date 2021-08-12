
#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEditor;
using UdonSharpEditor;
using UnityEngine;
#endif

namespace LiveDimensions.ThirdPersonCamera
{
    #if !COMPILER_UDONSHARP && UNITY_EDITOR
    [CustomEditor(typeof(ThirdPersonCamera))]
    public class ThirdPersonCameraEditor : Editor
    {
        bool showReferences = false;

        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
            DrawInspectorTitle();

            DrawKeyOptions();

            DrawDebug();
        }

        private void DrawKeyOptions()
        {
            ThirdPersonCamera thirdPersonCamera = ((ThirdPersonCamera)target);

            EditorGUI.BeginChangeCheck();

            bool thirdPersonEnabled = EditorGUILayout.Toggle("Enable third person on start?", thirdPersonCamera.thirdPersonEnabled);

            KeyCode enableThirdPersonKey = (KeyCode)EditorGUILayout.EnumPopup("Enable Third Person Key:", thirdPersonCamera.enableThirdPersonKey);
            KeyCode frontViewKey = (KeyCode)EditorGUILayout.EnumPopup("Front View Key:", thirdPersonCamera.frontViewKey);
            KeyCode backViewKey = (KeyCode)EditorGUILayout.EnumPopup("Back View Key:", thirdPersonCamera.backViewKey);
            KeyCode leftShoulderKey = (KeyCode)EditorGUILayout.EnumPopup("Left Shoulder Key:", thirdPersonCamera.leftShoulderKey);
            KeyCode rightShoulderKey = (KeyCode)EditorGUILayout.EnumPopup("Right Shoulder Key:", thirdPersonCamera.rightShoulderKey);


            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(thirdPersonCamera, "Key option changed.");

                thirdPersonCamera.thirdPersonEnabled = thirdPersonEnabled;

                thirdPersonCamera.enableThirdPersonKey = enableThirdPersonKey;
                thirdPersonCamera.frontViewKey = frontViewKey;
                thirdPersonCamera.backViewKey = backViewKey;
                thirdPersonCamera.leftShoulderKey = leftShoulderKey;
                thirdPersonCamera.rightShoulderKey = rightShoulderKey;
            }
        }

        private void DrawDebug()
        {
            GUILayout.Space(6);
            showReferences = GUILayout.Toggle(showReferences, "(Debug) Show References", "Button");
            if (showReferences) DrawDefaultInspector();
        }

        private void DrawInspectorTitle()
        {
            GUIStyle titleStyle = new GUIStyle();
            titleStyle.fontSize = 16;
            titleStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.gray;

            GUIStyle subtitleStyle = new GUIStyle();
            subtitleStyle.fontSize = 12;
            subtitleStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.gray : Color.gray;

            GUILayout.Label("Third Person Camera", titleStyle);
            GUILayout.Label("by LiveDimensions", subtitleStyle);

            GUILayout.Space(6);
            GUILayout.Box("This prefab should work out of the box, you can configure the keys used for changing between camera views.");
        }
    }
    #endif
}
