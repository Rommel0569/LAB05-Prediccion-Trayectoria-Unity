using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[InitializeOnLoad]
public static class ConfigureLabScene
{
    const string ScenePath = "Assets/Scenes/level1.unity";

    static ConfigureLabScene ()
    {
        EditorSceneManager.sceneOpened += OnSceneOpened;
        EditorApplication.delayCall += () => Configure (SceneManager.GetActiveScene ());
    }

    [MenuItem ("Laboratorio/Preparar escena y puntuacion")]
    static void ConfigureFromMenu ()
    {
        Configure (SceneManager.GetActiveScene ());
    }

    static void OnSceneOpened (Scene scene, OpenSceneMode mode)
    {
        EditorApplication.delayCall += () => Configure (scene);
    }

    static void Configure (Scene scene)
    {
        if (Application.isPlaying || scene.path != ScenePath) return;
        GameManager manager = Object.FindFirstObjectByType<GameManager> ();
        if (manager == null) return;
        bool changed = false;

        GameObject target = GameObject.Find ("Cub - objetivo adicional");
        if (target == null) {
            target = new GameObject ("Cub - objetivo adicional", typeof (SpriteRenderer), typeof (BoxCollider2D), typeof (ScoreTarget));
            target.transform.position = new Vector3 (2.5f, 0f, 0f);
            target.transform.localScale = new Vector3 (0.8f, 0.8f, 1f);
            target.GetComponent<SpriteRenderer> ().sprite = AssetDatabase.LoadAssetAtPath<Sprite> ("Assets/Sprites/box.png");
            target.GetComponent<SpriteRenderer> ().color = new Color (0.95f, 0.3f, 0.25f);
            target.GetComponent<BoxCollider2D> ().isTrigger = true;
            changed = true;
        }

        GameObject cup = GameObject.Find ("Cup");
        if (cup != null && cup.GetComponent<Cup> () == null) {
            cup.AddComponent<Cup> ();
            changed = true;
        }

        GameObject canvasObject = GameObject.Find ("Marcador");
        if (canvasObject == null) {
            canvasObject = new GameObject ("Marcador", typeof (Canvas), typeof (CanvasScaler));
            Canvas canvas = canvasObject.GetComponent<Canvas> ();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            changed = true;
        }
        Transform scoreTransform = canvasObject.transform.Find ("Puntuacion");
        Text scoreText;
        if (scoreTransform == null) {
            GameObject textObject = new GameObject ("Puntuacion", typeof (RectTransform), typeof (Text));
            textObject.transform.SetParent (canvasObject.transform, false);
            RectTransform rect = textObject.GetComponent<RectTransform> ();
            rect.anchorMin = rect.anchorMax = new Vector2 (0f, 1f);
            rect.pivot = new Vector2 (0f, 1f);
            rect.anchoredPosition = new Vector2 (20f, -20f);
            rect.sizeDelta = new Vector2 (300f, 50f);
            scoreText = textObject.GetComponent<Text> ();
            scoreText.font = Resources.GetBuiltinResource<Font> ("LegacyRuntime.ttf");
            changed = true;
        } else {
            scoreText = scoreTransform.GetComponent<Text> ();
        }
        if (scoreText != null) {
            scoreText.fontSize = 36;
            scoreText.fontStyle = FontStyle.Bold;
            scoreText.color = new Color (0.08f, 0.08f, 0.08f, 1f);
            scoreText.text = "SCORE: 0";
            scoreText.raycastTarget = false;
            changed = true;
        }

        SerializedObject serialized = new SerializedObject (manager);
        SerializedProperty scoreField = serialized.FindProperty ("scoreText");
        if (scoreField.objectReferenceValue != scoreText) {
            scoreField.objectReferenceValue = scoreText;
            serialized.ApplyModifiedPropertiesWithoutUndo ();
            changed = true;
        }

        if (changed) {
            EditorSceneManager.MarkSceneDirty (scene);
            EditorSceneManager.SaveScene (scene);
        }
    }
}
