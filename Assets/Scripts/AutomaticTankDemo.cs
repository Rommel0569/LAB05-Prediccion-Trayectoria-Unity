using UnityEngine;

public class AutomaticTankDemo : MonoBehaviour
{
    const float Speed = 11f;
    readonly Vector2 muzzle = new Vector2 (-5f, -2f);
    GameObject target;
    LineRenderer preview;
    float nextShot;
    int hits;

    void Start ()
    {
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = 5.5f;
        Camera.main.backgroundColor = Color.white;
        MakeBlock ("Tank", new Vector2 (-5.5f, -2.6f), new Vector2 (1.8f, 0.65f), new Color (0.3f, 0.4f, 0.45f));
        MakeBlock ("Barrel", new Vector2 (-4.85f, -2.15f), new Vector2 (1.3f, 0.18f), new Color (0.1f, 0.2f, 0.25f));
        target = MakeBlock ("Enemy", new Vector2 (4f, -2f), new Vector2 (0.75f, 0.75f), new Color (0.9f, 0.25f, 0.25f));
        var collider = target.AddComponent<BoxCollider2D> ();
        collider.isTrigger = true;
        target.AddComponent<TankTarget> ().demo = this;
        preview = new GameObject ("Calculated trajectory").AddComponent<LineRenderer> ();
        preview.material = new Material (Shader.Find ("Sprites/Default"));
        preview.startColor = preview.endColor = new Color (0.1f, 0.5f, 0.7f);
        preview.startWidth = preview.endWidth = 0.04f;
        preview.positionCount = 40;
    }

    void Update ()
    {
        if (!TryVelocity (muzzle, target.transform.position, Speed, out Vector2 velocity)) return;
        float flightTime = (target.transform.position.x - muzzle.x) / velocity.x;
        for (int i = 0; i < preview.positionCount; i++) {
            float t = flightTime * i / (preview.positionCount - 1);
            preview.SetPosition (i, muzzle + velocity * t + 0.5f * Physics2D.gravity * t * t);
        }
        if (Time.time < nextShot) return;
        nextShot = Time.time + 2.5f;
        GameObject shell = MakeBlock ("Shell", muzzle, new Vector2 (0.25f, 0.25f), new Color (0.95f, 0.65f, 0.1f));
        shell.AddComponent<CircleCollider2D> ();
        Rigidbody2D body = shell.AddComponent<Rigidbody2D> ();
        body.gravityScale = 1f;
        body.linearVelocity = velocity;
        shell.AddComponent<TankShell> ();
        Destroy (shell, 6f);
    }

    static bool TryVelocity (Vector2 start, Vector2 end, float speed, out Vector2 velocity)
    {
        float dx = end.x - start.x;
        float dy = end.y - start.y;
        float g = -Physics2D.gravity.y;
        float square = speed * speed;
        float discriminant = square * square - g * (g * dx * dx + 2f * dy * square);
        velocity = Vector2.zero;
        if (dx <= 0f || g <= 0f || discriminant < 0f) return false;
        float angle = Mathf.Atan ((square - Mathf.Sqrt (discriminant)) / (g * dx));
        velocity = new Vector2 (Mathf.Cos (angle), Mathf.Sin (angle)) * speed;
        return true;
    }

    static GameObject MakeBlock (string name, Vector2 position, Vector2 size, Color color)
    {
        GameObject item = new GameObject (name);
        item.transform.position = position;
        item.transform.localScale = size;
        SpriteRenderer renderer = item.AddComponent<SpriteRenderer> ();
        renderer.sprite = Sprite.Create (Texture2D.whiteTexture, new Rect (0, 0, 1, 1), new Vector2 (0.5f, 0.5f), 1f);
        renderer.color = color;
        return item;
    }

    public void RegisterHit () { hits++; }

    void OnGUI ()
    {
        GUIStyle style = new GUIStyle (GUI.skin.label);
        style.fontSize = 24;
        style.normal.textColor = Color.black;
        GUI.Label (new Rect (20, 20, 400, 35), "Automatic tank hits: " + hits, style);
    }
}

public class TankTarget : MonoBehaviour
{
    public AutomaticTankDemo demo;
    void OnTriggerEnter2D (Collider2D other)
    {
        if (other.GetComponent<TankShell> () != null) demo.RegisterHit ();
    }
}

public class TankShell : MonoBehaviour { }
