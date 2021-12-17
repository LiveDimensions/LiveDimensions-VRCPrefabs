

using UnityEditor;
using UnityEngine;

public static class LiveDimensionsEditorHelper
{
    public static void DrawDefaultInspectorButton(ref bool showDefaultInspector, Editor editor)
    {
        SeparatorLine();
        if (GUILayout.Button($"{(showDefaultInspector ? "Hide" : "Show")} Default Inspector"))
        {
            showDefaultInspector = !showDefaultInspector;
        }

        if (showDefaultInspector)
        {
            GUILayout.Space(EditorGUIUtility.singleLineHeight/4);
            editor.DrawDefaultInspector();
        }
    }

    public static void SeparatorLine()
    {
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }

    public static void DrawHeader(string title, string description)
    {
        GUIStyle titleStyle = new GUIStyle();
        titleStyle.fontSize = 16;
        titleStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.gray;

        GUIStyle subtitleStyle = new GUIStyle();
        subtitleStyle.fontSize = 12;
        subtitleStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.gray : Color.gray;

        GUILayout.Label(title, titleStyle);
        GUILayout.Label("by LiveDimensions", subtitleStyle);

        GUILayout.Space(6);
        GUILayout.Box(description);
    }
}
