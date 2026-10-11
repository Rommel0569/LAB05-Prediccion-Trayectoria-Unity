
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
	Vector3 ballStartPosition;
	bool shotActive;
	bool resetting;
	float shotTime;
	float stillTime;

	//---------------------------------------
	void Start ()
	{
		cam = Camera.main;
		ballStartPosition = ball.transform.position;
		ball.DesactivateRb ();
		CreateExtraTarget ();
		GameObject cup = GameObject.Find ("Cup");
		if (cup != null && cup.GetComponent<Cup> () == null)
			cup.AddComponent<Cup> ();
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.R)) {
			ResetBall ();
			return;
		}
		if (shotActive && !resetting) {
			shotTime += Time.deltaTime;
			stillTime = ball.rb.velocity.sqrMagnitude < 0.04f ? stillTime + Time.deltaTime : 0f;
			if (shotTime > 12f || (shotTime > 1f && stillTime > 1.5f) || ball.transform.position.y < -7f)
				ResetBall ();
		}
		if (!shotActive && !resetting && Input.GetMouseButtonDown (0)) {
			isDragging = true;
			OnDragStart ();
		}
		if (isDragging && Input.GetMouseButtonUp (0)) {
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
		shotActive = true;
		shotTime = 0f;
		stillTime = 0f;

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
		if (!shotActive || resetting) return;
		score++;
		shotActive = false;
		resetting = true;
		Invoke (nameof (ResetBall), 0.5f);
	}

	void ResetBall ()
	{
		CancelInvoke (nameof (ResetBall));
		isDragging = false;
		shotActive = false;
		resetting = false;
		shotTime = 0f;
		stillTime = 0f;
		ball.DesactivateRb ();
		ball.transform.position = ballStartPosition;
		trajectory.Hide ();
	}

	void OnGUI ()
	{
		if (scoreStyle == null) {
			scoreStyle = new GUIStyle (GUI.skin.label);
			scoreStyle.fontSize = 24;
			scoreStyle.normal.textColor = Color.white;
		}
		GUI.Label (new Rect (20, 20, 250, 40), "Puntos: " + score, scoreStyle);
		GUI.Label (new Rect (20, 55, 500, 30), "R: reiniciar bola");
	}

}
