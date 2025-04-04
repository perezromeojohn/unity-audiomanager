using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RumyooAudioManager
{
    public class AudioManagerMenu
    {
        [MenuItem("Tools/rumyoonomicon/Audio Manager")]
        public static void CreateOrFindAudioManager()
        {
            GameObject existingAudioManager = GameObject.Find("__AUDIOMANAGER");
            if (existingAudioManager != null)
            {
                Debug.LogError("An Audio Manager already exists in the scene.");
                Selection.activeGameObject = existingAudioManager;
                return;
            }

            string prefabPath = "Packages/com.rumyoonomicon.audiomanager/Core/__AUDIOMANAGER.prefab";
            GameObject audioManagerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (audioManagerPrefab == null)
            {
                Debug.LogError($"AudioManager prefab not found at {prefabPath}. Please ensure the path is correct.");
                return;
            }

            GameObject audioManagerInstance = (GameObject)PrefabUtility.InstantiatePrefab(audioManagerPrefab);
            PrefabUtility.UnpackPrefabInstance(audioManagerInstance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            Debug.Log("Audio Manager prefab instantiated and unpacked successfully.");
            Selection.activeGameObject = audioManagerInstance;
        }
    }
}