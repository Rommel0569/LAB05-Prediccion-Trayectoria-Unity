using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class SetupScene
{
    static SetupScene()
    {
        EditorApplication.delayCall += () =>
        {
            if (System.IO.File.Exists("Assets/Scenes/Main.unity")) return;
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            var camera = Camera.main;
            camera.transform.position = new Vector3(0, 0, -10);
            new GameObject("GameManager").AddComponent<TrajectoryGame>();
            System.IO.Directory.CreateDirectory("Assets/Scenes");
            EditorSceneManager.SaveScene(scene, "Assets/Scenes/Main.unity");
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene("Assets/Scenes/Main.unity", true) };
        };
    }
}
