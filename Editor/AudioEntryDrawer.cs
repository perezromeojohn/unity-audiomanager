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
        public static float GetHeight(bool sfx, bool expanded)
        {
            float line = EditorGUIUtility.singleLineHeight;
            float gap = EditorGUIUtility.standardVerticalSpacing;
            if (!expanded) return line;
            int rows = sfx ? 7 : 4;
            return line + gap + line * rows + gap * (rows - 1);
        }

        public static float GetHeight(SerializedProperty property, bool sfx)
        {
            return GetHeight(sfx, property.isExpanded);
        }

        private static int GetIndex(SerializedProperty property)
        {
            string path = property.propertyPath;
            int start = path.LastIndexOf('[');
            int end = path.LastIndexOf(']');
            if (start >= 0 && end > start && int.TryParse(path.Substring(start + 1, end - start - 1), out int index))
                return index;
            return -1;
        }

        public static void Draw(Rect position, SerializedProperty property, bool sfx)
        {
            float line = EditorGUIUtility.singleLineHeight;
            float gap = EditorGUIUtility.standardVerticalSpacing;
            Rect rect = new Rect(position.x, position.y, position.width, line);

            // Header: collapsible foldout titled with the entry name
            string title = property.FindPropertyRelative("name").stringValue;
            if (string.IsNullOrEmpty(title))
            {
                int index = GetIndex(property);
                title = index >= 0 ? "Entry " + (index + 1) : "Entry";
            }
            property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, title, true);

            AudioClip clip = property.FindPropertyRelative("clip").objectReferenceValue as AudioClip;
            if (clip != null)
            {
                GUIStyle clipStyle = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleRight };
                GUI.Label(new Rect(rect.x + rect.width * 0.5f, rect.y, rect.width * 0.5f, line), clip.name, clipStyle);
            }

            if (!property.isExpanded) return;
            rect.y += line + gap;

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
            return AudioEntryGUI.GetHeight(property, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            AudioEntryGUI.Draw(position, property, true);
            EditorGUI.EndProperty();
        }
    }
}