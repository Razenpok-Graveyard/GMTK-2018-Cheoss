using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor
{
    [InitializeOnLoad]
    public class AutoSaveScenes
    {
        static AutoSaveScenes()
        {
            EditorApplication.playModeStateChanged += mode =>
            {
                var isSwitchingToPlayMode = EditorApplication.isPlayingOrWillChangePlaymode &&
                                            !EditorApplication.isPlaying;
                if (!isSwitchingToPlayMode) return;
                if (!SceneManager.GetActiveScene().isDirty) return;
                Debug.Log("Auto-Saved opened scenes before entering Play mode");
                EditorApplication.Beep();
                AssetDatabase.SaveAssets();
                EditorSceneManager.SaveOpenScenes();
            };
        }
    }
}