using System.Collections.Generic;
using UnityEngine;

public class TrajectoryGame : MonoBehaviour
{
    const float LaunchScale = 5f;
    readonly List<LineRenderer> dots = new List<LineRenderer>();
    Rigidbody2D ball;
    Vector2 start = new Vector2(-6f, -2.5f);
    bool dragging;
    bool launched;
    int score;
    GUIStyle label;

    void Start()
    {
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = 5.5f;
        Camera.main.backgroundColor = new Color(.09f, .13f, .17f);
        CreateWall(new Vector2(0, -4.4f), new Vector2(18, .4f));
        CreateWall(new Vector2(-8.8f, 0), new Vector2(.4f, 9));
        CreateWall(new Vector2(8.8f, 0), new Vector2(.4f, 9));
        CreateWall(new Vector2(0, 4.4f), new Vector2(18, .4f));
        CreateWall(new Vector2(1, -2.1f), new Vector2(.4f, 4.2f));
        CreateCup(new Vector2(5.4f, -3.4f));
        CreateTarget(new Vector2(3.4f, .5f));
        CreateBall();
        for (int i = 0; i < 28; i++)
        {
            var dot = new GameObject("Trajectory dot " + i).AddComponent<LineRenderer>();
            dot.material = new Material(Shader.Find("Sprites/Default"));
            dot.startColor = dot.endColor = new Color(1f, .85f, .25f, .8f);
            dot.startWidth = dot.endWidth = .10f;
            dot.positionCount = 2;
            dot.enabled = false;
            dots.Add(dot);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) ResetBall();
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0) && !launched && Vector2.Distance(mouse, ball.position) < .7f)
            dragging = true;
        if (dragging && Input.GetMouseButton(0)) DrawTrajectory((start - mouse) * LaunchScale);
        if (dragging && Input.GetMouseButtonUp(0))
        {
            dragging = false;
            launched = true;
            HideTrajectory();
            ball.simulated = true;
            ball.velocity = (start - mouse) * LaunchScale;
        }
        if (launched && ball.position.y < -6)
            Invoke(nameof(ResetBall), 1f);
    }

    void DrawTrajectory(Vector2 velocity)
    {
        Vector2 gravity = Physics2D.gravity * ball.gravityScale;
        for (int i = 0; i < dots.Count; i++)
        {
            float t = (i + 1) * .075f;
            Vector2 p = start + velocity * t + .5f * gravity * t * t;
            dots[i].enabled = true;
            dots[i].SetPosition(0, p);
            dots[i].SetPosition(1, p + Vector2.up * .01f);
        }
    }

    void HideTrajectory() { foreach (var dot in dots) dot.enabled = false; }
    void ResetBall()
    {
        CancelInvoke(nameof(ResetBall));
        ball.simulated = false;
        ball.transform.position = start;
        ball.velocity = Vector2.zero;
        launched = false;
        HideTrajectory();
    }

    void CreateBall()
    {
        var go = MakeSprite("Ball", start, new Vector2(.58f, .58f), new Color(1f, .68f, .24f));
        ball = go.AddComponent<Rigidbody2D>();
        ball.simulated = false;
        ball.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        var collider = go.AddComponent<CircleCollider2D>();
        var bounce = new PhysicsMaterial2D("Rebote") { bounciness = .65f, friction = .15f };
        collider.sharedMaterial = bounce;
    }

    void CreateWall(Vector2 position, Vector2 size)
    {
        var go = MakeSprite("Wall", position, size, new Color(.35f, .54f, .56f));
        go.AddComponent<BoxCollider2D>();
    }

    void CreateCup(Vector2 position)
    {
        var root = new GameObject("Cup");
        root.transform.position = position;
        var body = root.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Static;
        var edge = root.AddComponent<EdgeCollider2D>();
        edge.points = new[] { new Vector2(-.85f, .7f), new Vector2(-.65f, -.45f), new Vector2(.65f, -.45f), new Vector2(.85f, .7f) };
        MakeSprite("Cup base", position + Vector2.down * .45f, new Vector2(1.3f, .16f), new Color(.3f, .8f, .65f)).transform.SetParent(root.transform, true);
        MakeSprite("Cup left", position + new Vector2(-.75f, .1f), new Vector2(.16f, 1.2f), new Color(.3f, .8f, .65f)).transform.SetParent(root.transform, true);
        MakeSprite("Cup right", position + new Vector2(.75f, .1f), new Vector2(.16f, 1.2f), new Color(.3f, .8f, .65f)).transform.SetParent(root.transform, true);
        var sensor = new GameObject("Cup score zone");
        sensor.transform.SetParent(root.transform, false);
        sensor.transform.localPosition = new Vector2(0, -.1f);
        var trigger = sensor.AddComponent<BoxCollider2D>();
        trigger.size = new Vector2(1.1f, .5f);
        trigger.isTrigger = true;
        sensor.AddComponent<ScoreTarget>().game = this;
    }

    void CreateTarget(Vector2 position)
    {
        var go = MakeSprite("Cub target", position, new Vector2(.75f, .75f), new Color(.95f, .35f, .4f));
        var trigger = go.AddComponent<BoxCollider2D>();
        trigger.isTrigger = true;
        go.AddComponent<ScoreTarget>().game = this;
    }

    static GameObject MakeSprite(string name, Vector2 position, Vector2 size, Color color)
    {
        var go = new GameObject(name);
        go.transform.position = position;
        var texture = Texture2D.whiteTexture;
        var sprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(.5f, .5f), 1);
        var renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = color;
        go.transform.localScale = size;
        return go;
    }

    public void AwardPoint()
    {
        if (!launched) return;
        score++;
        ResetBall();
    }

    void OnGUI()
    {
        if (label == null) label = new GUIStyle(GUI.skin.label) { fontSize = 24, fontStyle = FontStyle.Bold };
        label.normal.textColor = Color.white;
        GUI.Label(new Rect(20, 20, 500, 40), "Puntos: " + score, label);
        GUI.Label(new Rect(20, 55, 700, 35), "Arrastra la bola y suelta para lanzar. R: reiniciar.");
    }
}

public class ScoreTarget : MonoBehaviour
{
    public TrajectoryGame game;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Rigidbody2D>() && other.name == "Ball") game.AwardPoint();
    }
}
