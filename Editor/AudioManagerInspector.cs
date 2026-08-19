using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace RumyooAudioManager
{
    [CustomEditor(typeof(AudioManager))]
    public class AudioManagerInspector : Editor
    {
        // ----- muted pro palette -----
        private static readonly Color BgmAccent = new Color(0.545f, 0.482f, 0.847f); // violet
        private static readonly Color SfxAccent = new Color(0.878f, 0.643f, 0.347f); // amber
        private static readonly Color RouteAccent = new Color(0.470f, 0.510f, 0.560f); // slate

        private static Color BannerBg => EditorGUIUtility.isProSkin
            ? new Color(0.16f, 0.17f, 0.19f)
            : new Color(0.80f, 0.80f, 0.82f);

        private static Color TitleColor => EditorGUIUtility.isProSkin
            ? new Color(0.92f, 0.92f, 0.95f)
            : new Color(0.13f, 0.13f, 0.16f);

        private ReorderableList bgmList;
        private ReorderableList sfxList;

        private void OnEnable()
        {
            bgmList = MakeList("backgroundClips", false);
            sfxList = MakeList("effectClips", true);
        }

        private ReorderableList MakeList(string arrayPath, bool sfx)
        {
            ReorderableList list = new ReorderableList(serializedObject, serializedObject.FindProperty(arrayPath), true, false, true, true);
            list.headerHeight = 0f;
            list.footerHeight = 18f; // reserve room for the default +/− buttons so they never overlap the next section
            list.elementHeightCallback = _ => AudioEntryGUI.GetHeight(sfx) + 4f;
            list.drawElementCallback = (rect, index, active, focused) =>
            {
                Rect inner = new Rect(rect.x, rect.y + 2f, rect.width, rect.height - 4f);
                AudioEntryGUI.Draw(inner, list.serializedProperty.GetArrayElementAtIndex(index), sfx);
            };
            return list;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawBanner("♫", "Background Music", BgmAccent, TrackBadge(bgmList.count, "track"));
            bgmList.DoLayoutList();

            DrawBanner("⚡", "Sound Effects", SfxAccent, TrackBadge(sfxList.count, "clip"));
            sfxList.DoLayoutList();

            DrawBanner("⚙", "Output & Routing", RouteAccent, null);
            EditorGUILayout.Space(2);

            // top-level fields only — enter children once, then siblings only.
            // (the old NextVisible(true) loop dove into the arrays and re-drew
            //  sizes/elements at the bottom, producing ghost duplicate entries)
            SerializedProperty prop = serializedObject.GetIterator();
            bool enterChildren = true;
            while (prop.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (prop.name == "m_Script" || prop.name == "backgroundClips" || prop.name == "effectClips")
                    continue;
                EditorGUILayout.PropertyField(prop);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static string TrackBadge(int count, string unit)
        {
            return count + " " + unit + (count == 1 ? "" : "s");
        }

        private static void DrawBanner(string icon, string title, Color accent, string badge)
        {
            EditorGUILayout.Space(6);
            Rect rect = EditorGUILayout.GetControlRect(false, 24f);
            EditorGUI.DrawRect(rect, BannerBg);
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, 3f, rect.height), accent);

            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleLeft };
            titleStyle.normal.textColor = TitleColor;
            GUI.Label(new Rect(rect.x + 10f, rect.y, rect.width - 20f, rect.height), icon + "  " + title, titleStyle);

            if (!string.IsNullOrEmpty(badge))
            {
                GUIStyle badgeStyle = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleRight };
                badgeStyle.normal.textColor = accent;
                GUI.Label(new Rect(rect.x + 8f, rect.y, rect.width - 16f, rect.height), badge, badgeStyle);
            }
        }
    }
}