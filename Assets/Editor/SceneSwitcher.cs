using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor
{
    public class SceneSwitcher : EditorWindow
    {
        
        public static string MainMenuScene = "MainMenu";
        public static string GameScene = "Game";
        public static string BootstrapScene = "Bootstrap";
        
        
        [MenuItem("Switch Scene/MainMenu")]
        private static void OpenMainMenuScene()
        {
            OpenScene(MainMenuScene);
        }
        [MenuItem("Switch Scene/Bootstrap")]
        private static void OpenBootstrapScene()
        {
            OpenScene(BootstrapScene);
        }

        [MenuItem("Switch Scene/Game")]
        private static void OpenGameScene()
        {
            OpenScene(GameScene);
        }

        private static void OpenScene(string sceneName)
        {
            if (EditorApplication.isPlaying)
            {
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                if (!UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
                var scenePath = $"Assets/Game/Scenes/{sceneName}.unity";
                if (System.IO.File.Exists(scenePath))
                {
                    UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath);
                }
                else
                {
                    Debug.LogError($"Scene {sceneName} not found at {scenePath}!");
                }
            }
        }
    }
}