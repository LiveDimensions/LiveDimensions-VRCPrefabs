using UnityEditor;
using UdonSharpEditor;
using UnityEngine;

namespace LiveDimensions.ThirdPersonCamera
{
    [CustomEditor(typeof(ThirdPersonCamera))]
    public class ThirdPersonCameraEditor : Editor
    {
        bool showDefaultInspector = false;

        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;

            LiveDimensionsEditorHelper.DrawHeader("Third Person Camera", "This prefab should work out of the box, you can configure the keys used for changing between camera views.");

            DrawKeyOptions();

            LiveDimensionsEditorHelper.DrawDefaultInspectorButton(ref showDefaultInspector, this);
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
    }
}
