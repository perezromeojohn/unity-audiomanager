using UnityEditor;
using UnityEngine;

namespace RumyooAudioManager
{
    [CustomPropertyDrawer(typeof(AudioEntry))]
    public class AudioEntryDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 5 + EditorGUIUtility.standardVerticalSpacing * 4;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float line = EditorGUIUtility.singleLineHeight;
            float gap = EditorGUIUtility.standardVerticalSpacing;
            Rect rect = new Rect(position.x, position.y, position.width, line);

            EditorGUI.PropertyField(rect, property.FindPropertyRelative("name"), new GUIContent("Name"));
            rect.y += line + gap;

            // Clip + Preview button
            SerializedProperty clipProp = property.FindPropertyRelative("clip");
            Rect clipRect = new Rect(rect.x, rect.y, position.width - 70f, line);
            EditorGUI.PropertyField(clipRect, clipProp, new GUIContent("Clip"));
            if (GUI.Button(new Rect(rect.x + position.width - 65f, rect.y, 65f, line), "Preview"))
            {
                AudioClip clip = (AudioClip)clipProp.objectReferenceValue;
                if (clip != null)
                    AudioManager.PreviewClip(clip, property.FindPropertyRelative("volume").floatValue);
            }
            rect.y += line + gap;

            // Loop + Volume
            SerializedProperty loopProp = property.FindPropertyRelative("loop");
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, 100f, line), loopProp, new GUIContent("Loop"));
            EditorGUI.PropertyField(new Rect(rect.x + 110f, rect.y, position.width - 110f, line), property.FindPropertyRelative("volume"), new GUIContent("Volume"));
            rect.y += line + gap;

            // Pitch + variation
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, 90f, line), property.FindPropertyRelative("pitch"), new GUIContent("Pitch"));
            EditorGUI.PropertyField(new Rect(rect.x + 110f, rect.y, position.width - 110f, line), property.FindPropertyRelative("pitchVariation"), new GUIContent("± Variation"));
            rect.y += line + gap;

            // Max concurrent
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("maxConcurrent"), new GUIContent("Max Concurrent"));

            EditorGUI.EndProperty();
        }
    }
}