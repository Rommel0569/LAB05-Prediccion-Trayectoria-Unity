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
        changed |= EnsureGuideComponents (scene, manager);

        bool arrange = GameObject.Find ("Laboratorio - distribucion v3") == null;
        changed |= EnsurePlatform ("Plataforma morada izquierda", new Vector3 (-4.2f, -2.5f, 0f), new Vector2 (4.5f, 0.35f), arrange);
        changed |= EnsurePlatform ("Plataforma morada elevada", new Vector3 (-3.5f, -0.6f, 0f), new Vector2 (2.6f, 0.35f), arrange);
        changed |= EnsurePlatform ("Plataforma morada derecha", new Vector3 (5.1f, -2.5f, 0f), new Vector2 (4.5f, 0.35f), arrange);
        changed |= EnsurePlatform ("Plataforma morada superior", new Vector3 (4.7f, 1.2f, 0f), new Vector2 (2.5f, 0.35f), arrange);
        changed |= EnsureTarget ("Cub - objetivo adicional", "Plataforma morada elevada", new Color (0.95f, 0.3f, 0.25f), 10, arrange);
        changed |= EnsureTarget ("Cub - objetivo 2", "Plataforma morada derecha", new Color (0.12f, 0.68f, 0.72f), 20, arrange);
        changed |= EnsureTarget ("Cub - objetivo 3", "Plataforma morada superior", new Color (0.95f, 0.7f, 0.15f), 30, arrange);
        if (arrange) {
            new GameObject ("Laboratorio - distribucion v3");
            changed = true;
        }
        Camera camera = Camera.main;
        if (camera != null && camera.orthographic && camera.orthographicSize < 6.2f) {
            camera.orthographicSize = 6.2f;
            changed = true;
        }
        if (SceneView.lastActiveSceneView != null)
            SceneView.lastActiveSceneView.in2DMode = true;

        GameObject cup = GameObject.Find ("Cup");
        if (cup != null && cup.GetComponent<Cup> () == null) {
            cup.AddComponent<Cup> ();
            changed = true;
        }
        if (cup != null && cup.transform.Find ("Soap - zona interior") == null) {
            GameObject zone = new GameObject ("Soap - zona interior", typeof (BoxCollider2D), typeof (SoapGoalZone));
            zone.transform.SetParent (cup.transform, false);
            SpriteRenderer cupRenderer = cup.GetComponent<SpriteRenderer> ();
            Vector2 cupSize = cupRenderer != null && cupRenderer.sprite != null ? cupRenderer.sprite.bounds.size : new Vector2 (2f, 2f);
            zone.transform.localPosition = new Vector3 (0f, cupSize.y * 0.12f, 0f);
            BoxCollider2D zoneCollider = zone.GetComponent<BoxCollider2D> ();
            zoneCollider.size = new Vector2 (cupSize.x * 0.48f, cupSize.y * 0.38f);
            zoneCollider.isTrigger = true;
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
        Text detailText = EnsureUiText (canvasObject.transform, "Detalle de puntos", new Vector2 (20f, -65f), 20, "CUBOS: 0   SOAP: 0");

        SerializedObject serialized = new SerializedObject (manager);
        SerializedProperty scoreField = serialized.FindProperty ("scoreText");
        if (scoreField.objectReferenceValue != scoreText) {
            scoreField.objectReferenceValue = scoreText;
            serialized.ApplyModifiedPropertiesWithoutUndo ();
            changed = true;
        }
        SerializedProperty detailField = serialized.FindProperty ("detailText");
        if (detailField.objectReferenceValue != detailText) {
            detailField.objectReferenceValue = detailText;
            serialized.ApplyModifiedPropertiesWithoutUndo ();
            changed = true;
        }

        if (changed) {
            EditorSceneManager.MarkSceneDirty (scene);
            EditorSceneManager.SaveScene (scene);
        }
    }

    static bool EnsureTarget (string name, string platformName, Color color, int points, bool arrange)
    {
        GameObject target = GameObject.Find (name);
        bool created = target == null;
        if (created) target = new GameObject (name, typeof (SpriteRenderer), typeof (BoxCollider2D), typeof (ScoreTarget));
        if (created || arrange) {
            target.transform.rotation = Quaternion.identity;
            target.transform.localScale = new Vector3 (0.8f, 0.8f, 1f);
        }
        SpriteRenderer renderer = target.GetComponent<SpriteRenderer> ();
        renderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite> ("Assets/Sprites/box.png");
        renderer.color = color;
        BoxCollider2D collider = target.GetComponent<BoxCollider2D> ();
        Vector2 colliderSize = renderer.sprite.bounds.size;
        bool colliderChanged = collider.size != colliderSize || collider.isTrigger;
        collider.size = colliderSize;
        collider.isTrigger = false;
        if (created || arrange) {
            GameObject platform = GameObject.Find (platformName);
            float platformTop = platform.GetComponent<BoxCollider2D> ().bounds.max.y;
            float targetHalfHeight = target.GetComponent<SpriteRenderer> ().bounds.extents.y;
            target.transform.position = new Vector3 (platform.transform.position.x, platformTop + targetHalfHeight + 0.005f, 0f);
        }
        target.GetComponent<ScoreTarget> ().points = points;
        return created || arrange || colliderChanged;
    }

    static bool EnsurePlatform (string name, Vector3 position, Vector2 size, bool arrange)
    {
        GameObject platform = GameObject.Find (name);
        bool created = platform == null;
        if (created) platform = new GameObject (name, typeof (SpriteRenderer), typeof (BoxCollider2D));
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite> ("Assets/Sprites/box.png");
        SpriteRenderer renderer = platform.GetComponent<SpriteRenderer> ();
        renderer.sprite = sprite;
        renderer.color = new Color (0.46f, 0.05f, 0.8f);
        Vector2 spriteSize = sprite.bounds.size;
        if (created || arrange) {
            platform.transform.position = position;
            platform.transform.rotation = Quaternion.identity;
            platform.transform.localScale = new Vector3 (size.x / spriteSize.x, size.y / spriteSize.y, 1f);
        }
        platform.GetComponent<BoxCollider2D> ().size = spriteSize;
        return created || arrange;
    }

    static Text EnsureUiText (Transform parent, string name, Vector2 position, int size, string content)
    {
        Transform existing = parent.Find (name);
        GameObject textObject = existing != null ? existing.gameObject : new GameObject (name, typeof (RectTransform), typeof (Text));
        if (existing == null) textObject.transform.SetParent (parent, false);
        RectTransform rect = textObject.GetComponent<RectTransform> ();
        rect.anchorMin = rect.anchorMax = new Vector2 (0f, 1f);
        rect.pivot = new Vector2 (0f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2 (420f, 35f);
        Text text = textObject.GetComponent<Text> ();
        text.font = Resources.GetBuiltinResource<Font> ("LegacyRuntime.ttf");
        text.fontSize = size;
        text.color = new Color (0.08f, 0.08f, 0.08f, 1f);
        text.text = content;
        text.raycastTarget = false;
        return text;
    }

    static bool EnsureGuideComponents (Scene scene, GameManager manager)
    {
        bool changed = false;
        foreach (Transform item in Object.FindObjectsByType<Transform> (FindObjectsSortMode.None)) {
            if (item.gameObject.scene != scene || !item.name.StartsWith ("wall", System.StringComparison.OrdinalIgnoreCase)) continue;
            if (item.GetComponent<BoxCollider2D> () == null) {
                item.gameObject.AddComponent<BoxCollider2D> ();
                changed = true;
            }
        }

        GameObject ballObject = GameObject.Find ("Ball");
        if (ballObject != null) {
            Ball ball = ballObject.GetComponent<Ball> ();
            if (ball == null) { ball = ballObject.AddComponent<Ball> (); changed = true; }
            Rigidbody2D ballBody = ballObject.GetComponent<Rigidbody2D> ();
            if (ballBody == null) { ballBody = ballObject.AddComponent<Rigidbody2D> (); changed = true; }
            if (ballBody.mass != 1f || ballBody.gravityScale != 1f || ballBody.linearDamping != 0f || ballBody.angularDamping != 0f || ballBody.collisionDetectionMode != CollisionDetectionMode2D.Discrete) {
                ballBody.mass = 1f;
                ballBody.gravityScale = 1f;
                ballBody.linearDamping = 0f;
                ballBody.angularDamping = 0f;
                ballBody.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
                changed = true;
            }
            CircleCollider2D circle = ballObject.GetComponent<CircleCollider2D> ();
            if (circle == null) { circle = ballObject.AddComponent<CircleCollider2D> (); changed = true; }
            PhysicsMaterial2D bounce = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D> ("Assets/Physics/bounciness.physicsMaterial2D");
            if (bounce != null && circle.sharedMaterial != bounce) { circle.sharedMaterial = bounce; changed = true; }
            if (bounce != null && ballBody.sharedMaterial != bounce) { ballBody.sharedMaterial = bounce; changed = true; }
            SerializedObject managerData = new SerializedObject (manager);
            SerializedProperty ballField = managerData.FindProperty ("ball");
            if (ballField.objectReferenceValue != ball) {
                ballField.objectReferenceValue = ball;
                managerData.ApplyModifiedPropertiesWithoutUndo ();
                changed = true;
            }
        }

        GameObject cupObject = GameObject.Find ("Cup");
        if (cupObject != null) {
            Rigidbody2D cupBody = cupObject.GetComponent<Rigidbody2D> ();
            if (cupBody == null) { cupBody = cupObject.AddComponent<Rigidbody2D> (); changed = true; }
            if (cupBody.mass != 1.5f || cupBody.gravityScale != 0.5f || cupBody.linearDamping != 0.5f || cupBody.angularDamping != 0.5f || cupBody.collisionDetectionMode != CollisionDetectionMode2D.Continuous) {
                cupBody.mass = 1.5f;
                cupBody.gravityScale = 0.5f;
                cupBody.linearDamping = 0.5f;
                cupBody.angularDamping = 0.5f;
                cupBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                changed = true;
            }
            if (cupObject.GetComponent<EdgeCollider2D> () == null) {
                EdgeCollider2D edge = cupObject.AddComponent<EdgeCollider2D> ();
                edge.points = new [] { new Vector2 (-0.7f, 0.8f), new Vector2 (-0.5f, -0.8f), new Vector2 (0.5f, -0.8f), new Vector2 (0.7f, 0.8f) };
                changed = true;
            }
        }

        Trajectory trajectory = Object.FindFirstObjectByType<Trajectory> ();
        if (trajectory != null) {
            SerializedObject managerData = new SerializedObject (manager);
            SerializedProperty trajectoryField = managerData.FindProperty ("trajectory");
            if (trajectoryField.objectReferenceValue != trajectory) {
                trajectoryField.objectReferenceValue = trajectory;
                managerData.ApplyModifiedPropertiesWithoutUndo ();
                changed = true;
            }
        }
        return changed;
    }
}
