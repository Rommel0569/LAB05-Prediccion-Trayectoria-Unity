
using UnityEngine;
using UnityEngine.UI;

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
	[SerializeField] Text scoreText;

	bool isDragging = false;

	Vector2 startPoint;
	Vector2 endPoint;
	Vector2 direction;
	Vector2 force;
	float distance;
	int score;
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
		UpdateScoreText ();
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.R)) {
			ResetBall ();
			return;
		}
		if (shotActive && !resetting) {
			shotTime += Time.deltaTime;
			stillTime = ball.rb.linearVelocity.sqrMagnitude < 0.04f ? stillTime + Time.deltaTime : 0f;
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

	public void AddPoint ()
	{
		if (!shotActive || resetting) return;
		score++;
		UpdateScoreText ();
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

	void UpdateScoreText ()
	{
		if (scoreText != null)
			scoreText.text = "SCORE: " + score;
	}

}
