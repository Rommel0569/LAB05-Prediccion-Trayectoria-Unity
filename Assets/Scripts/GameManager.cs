
using UnityEngine;

public class GameManager : MonoBehaviour
{
	#region Singleton class: GameManager

	public static GameManager Instance;

	void Awake ()
	{
		if (Instance == null) {
			Instance = this;
		}
	}

	#endregion

	Camera cam;

	public Ball ball;
	public Trajectory trajectory;
	[SerializeField] float pushForce = 4f;

	bool isDragging = false;

	Vector2 startPoint;
	Vector2 endPoint;
	Vector2 direction;
	Vector2 force;
	float distance;
	int score;
	GUIStyle scoreStyle;

	//---------------------------------------
	void Start ()
	{
		cam = Camera.main;
		ball.DesactivateRb ();
		CreateExtraTarget ();
	}

	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {
			isDragging = true;
			OnDragStart ();
		}
		if (Input.GetMouseButtonUp (0)) {
			isDragging = false;
			OnDragEnd ();
		}

		if (isDragging) {
			OnDrag ();
		}
	}

	//-Drag--------------------------------------
	void OnDragStart ()
	{
		ball.DesactivateRb ();
		startPoint = cam.ScreenToWorldPoint (Input.mousePosition);

		trajectory.Show ();
	}

	void OnDrag ()
	{
		endPoint = cam.ScreenToWorldPoint (Input.mousePosition);
		distance = Vector2.Distance (startPoint, endPoint);
		direction = (startPoint - endPoint).normalized;
		force = direction * distance * pushForce;

		//just for debug
		Debug.DrawLine (startPoint, endPoint);


		trajectory.UpdateDots (ball.pos, force);
	}

	void OnDragEnd ()
	{
		//push the ball
		ball.ActivateRb ();

		ball.Push (force);

		trajectory.Hide ();
	}

	void CreateExtraTarget ()
	{
		GameObject target = new GameObject ("Cub - objetivo adicional");
		target.transform.position = new Vector3 (2.5f, 0f, 0f);
		target.transform.localScale = new Vector3 (0.8f, 0.8f, 1f);
		SpriteRenderer renderer = target.AddComponent<SpriteRenderer> ();
		renderer.sprite = Sprite.Create (Texture2D.whiteTexture, new Rect (0, 0, 1, 1), new Vector2 (0.5f, 0.5f), 1f);
		renderer.color = new Color (0.95f, 0.3f, 0.25f);
		BoxCollider2D collider = target.AddComponent<BoxCollider2D> ();
		collider.isTrigger = true;
		target.AddComponent<ScoreTarget> ();
	}

	public void AddPoint ()
	{
		score++;
	}

	void OnGUI ()
	{
		if (scoreStyle == null) {
			scoreStyle = new GUIStyle (GUI.skin.label);
			scoreStyle.fontSize = 24;
			scoreStyle.normal.textColor = Color.white;
		}
		GUI.Label (new Rect (20, 20, 250, 40), "Puntos: " + score, scoreStyle);
	}

}
