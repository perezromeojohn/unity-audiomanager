using UnityEditor;
using UnityEngine;

namespace RumyooAudioManager
{
    /// <summary>
    /// Shared drawing routines for <see cref="AudioEntry"/> so the PropertyDrawer and the
    /// inspector's ReorderableLists render identically.
    /// BGM entries get a slim layout (pitch/variation/maxConcurrent are SFX-only at runtime).
    /// </summary>
    public static class AudioEntryGUI
    {
        public static float GetHeight(bool sfx)
        {
            int rows = sfx ? 7 : 4;
            return EditorGUIUtility.singleLineHeight * rows + EditorGUIUtility.standardVerticalSpacing * (rows - 1);
        }

        public static void Draw(Rect position, SerializedProperty property, bool sfx)
        {
            float line = EditorGUIUtility.singleLineHeight;
            float gap = EditorGUIUtility.standardVerticalSpacing;
            Rect rect = new Rect(position.x, position.y, position.width, line);

            // Row: Name
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("name"), new GUIContent("Name"));
            rect.y += line + gap;

            // Row: Clip
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("clip"), new GUIContent("Clip"));
            rect.y += line + gap;

            // Row: Loop
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("loop"), new GUIContent("Loop"));
            rect.y += line + gap;

            // Row: Volume
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("volume"), new GUIContent("Volume"));
            rect.y += line + gap;

            if (!sfx) return; // music ignores pitch / variation / maxConcurrent at runtime

            // Row: Pitch
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("pitch"), new GUIContent("Pitch"));
            rect.y += line + gap;

            // Row: ± Variation
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("pitchVariation"),
                new GUIContent("± Variation", "Random +/- pitch deviation applied on each play"));
            rect.y += line + gap;

            // Row: Max Concurrent
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("maxConcurrent"), new GUIContent("Max Concurrent"));
        }
    }

    [CustomPropertyDrawer(typeof(AudioEntry))]
    public class AudioEntryDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return AudioEntryGUI.GetHeight(true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            AudioEntryGUI.Draw(position, property, true);
            EditorGUI.EndProperty();
        }
    }
}