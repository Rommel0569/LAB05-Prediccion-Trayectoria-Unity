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

        GameObject originalTarget = GameObject.Find ("Cub - objetivo adicional");
        if (originalTarget != null && Vector3.Distance (originalTarget.transform.position, new Vector3 (2.5f, 0f, 0f)) < 0.1f) {
            originalTarget.transform.position = new Vector3 (0f, 2.2f, 0f);
            changed = true;
        }
        changed |= EnsureTarget ("Cub - objetivo adicional", new Vector3 (0f, 2.2f, 0f), new Color (0.95f, 0.3f, 0.25f));
        changed |= EnsureTarget ("Cub - objetivo 2", new Vector3 (-2.5f, 1.5f, 0f), new Color (0.12f, 0.68f, 0.72f));
        changed |= EnsureTarget ("Cub - objetivo 3", new Vector3 (1.4f, 3.2f, 0f), new Color (0.95f, 0.7f, 0.15f));

        changed |= EnsurePlatform ("Plataforma morada izquierda", new Vector3 (-4.2f, -2.5f, 0f), new Vector2 (4.5f, 0.35f));
        changed |= EnsurePlatform ("Plataforma morada elevada", new Vector3 (-3.5f, -0.6f, 0f), new Vector2 (2.6f, 0.35f));
        changed |= EnsurePlatform ("Plataforma morada derecha", new Vector3 (5.1f, -2.5f, 0f), new Vector2 (4.5f, 0.35f));
        changed |= EnsurePlatform ("Plataforma morada superior", new Vector3 (4.7f, 1.2f, 0f), new Vector2 (2.5f, 0.35f));
        Camera camera = Camera.main;
        if (camera != null && camera.orthographic && camera.orthographicSize < 6.2f) {
            camera.orthographicSize = 6.2f;
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

    static bool EnsureTarget (string name, Vector3 position, Color color)
    {
        if (GameObject.Find (name) != null) return false;
        GameObject target = new GameObject (name, typeof (SpriteRenderer), typeof (BoxCollider2D), typeof (ScoreTarget));
        target.transform.position = position;
        target.transform.localScale = new Vector3 (0.8f, 0.8f, 1f);
        target.GetComponent<SpriteRenderer> ().sprite = AssetDatabase.LoadAssetAtPath<Sprite> ("Assets/Sprites/box.png");
        target.GetComponent<SpriteRenderer> ().color = color;
        target.GetComponent<BoxCollider2D> ().isTrigger = true;
        return true;
    }

    static bool EnsurePlatform (string name, Vector3 position, Vector2 size)
    {
        if (GameObject.Find (name) != null) return false;
        GameObject platform = new GameObject (name, typeof (SpriteRenderer), typeof (BoxCollider2D));
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite> ("Assets/Sprites/box.png");
        SpriteRenderer renderer = platform.GetComponent<SpriteRenderer> ();
        renderer.sprite = sprite;
        renderer.color = new Color (0.46f, 0.05f, 0.8f);
        Vector2 spriteSize = sprite.bounds.size;
        platform.transform.position = position;
        platform.transform.localScale = new Vector3 (size.x / spriteSize.x, size.y / spriteSize.y, 1f);
        platform.GetComponent<BoxCollider2D> ().size = spriteSize;
        return true;
    }
}
