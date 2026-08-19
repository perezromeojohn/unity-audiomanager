using UnityEditor;
using UnityEngine;

namespace RumyooAudioManager
{
    [CustomEditor(typeof(AudioManager))]
    public class AudioManagerInspector : Editor
    {
        private static readonly Color BGMColor = new Color(0.54f, 0.75f, 0.96f);   // light blue
        private static readonly Color SFXColor = new Color(0.949f, 0.047f, 0.365f); // (242, 12, 93)

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawHeader("Background Music", BGMColor);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("backgroundClips"), true);

            EditorGUILayout.Space();
            DrawHeader("Sound Effects", SFXColor);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("effectClips"), true);

            EditorGUILayout.Space();

            // everything else in declaration order, minus the two arrays
            SerializedProperty prop = serializedObject.GetIterator();
            while (prop.NextVisible(true))
            {
                if (prop.name == "backgroundClips" || prop.name == "effectClips") continue;
                EditorGUILayout.PropertyField(prop, true);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static void DrawHeader(string text, Color color)
        {
            EditorGUILayout.Space(2);
            Rect rect = EditorGUILayout.GetControlRect(false, 24f);
            EditorGUI.DrawRect(rect, color);
            GUI.Label(new Rect(rect.x + 10f, rect.y, rect.width - 20f, rect.height), text,
                new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleLeft });
        }
    }
}