using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEditor;
using UdonSharpEditor;
#endif

namespace LiveDimensions.Audio.PlayerSettings
{
#if !COMPILER_UDONSHARP && UNITY_EDITOR
    [CustomEditor(typeof(PlayerAudioOverride))]
    public class PlayerAudioOverrideEditor : Editor
    {
        private static readonly string[] _dontIncludeMe = new string[] { "m_Script" };

        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
            DrawInspectorTitle();

            //serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, _dontIncludeMe);

            serializedObject.ApplyModifiedProperties();

        }

        private void DrawInspectorTitle()
        {
            GUIStyle titleStyle = new GUIStyle();
            titleStyle.fontSize = 16;
            titleStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.gray;

            GUIStyle subtitleStyle = new GUIStyle();
            subtitleStyle.fontSize = 12;
            subtitleStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.gray : Color.gray;

            GUILayout.Label("Player Audio Override", titleStyle);
            GUILayout.Label("by LiveDimensions", subtitleStyle);

            GUILayout.Space(6);
            GUILayout.Box("Whenever a player enters the trigger in this GameObject their audio settings will be adjusted to the settings below. When they exit their values will reset to default or to the World Audio Settings (if specified). This script is meant to be a replacement to the SDK2 VRC_PlayerAudioOverride component.");
        }
    }
#endif
}


