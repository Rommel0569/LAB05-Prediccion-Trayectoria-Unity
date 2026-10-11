using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class CreateTankDemoScene
{
    const string Path = "Assets/Scenes/CalculatingTrajectories.unity";

    static CreateTankDemoScene ()
    {
        EditorApplication.delayCall += CreateIfMissing;
    }

    [MenuItem ("Laboratorio/Crear demo Calculating Trajectories")]
    static void CreateIfMissing ()
    {
        if (Application.isPlaying || AssetDatabase.LoadAssetAtPath<SceneAsset> (Path) != null) return;
        var scene = EditorSceneManager.NewScene (NewSceneSetup.DefaultGameObjects, NewSceneMode.Additive);
        foreach (GameObject root in scene.GetRootGameObjects ()) {
            Camera camera = root.GetComponent<Camera> ();
            if (camera != null) camera.transform.position = new Vector3 (0f, 0f, -10f);
        }
        GameObject manager = new GameObject ("Automatic Tank Demo");
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene (manager, scene);
        manager.AddComponent<AutomaticTankDemo> ();
        EditorSceneManager.SaveScene (scene, Path);
        EditorSceneManager.CloseScene (scene, true);
    }
}
