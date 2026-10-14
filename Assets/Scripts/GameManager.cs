
using UnityEngine;
using UnityEngine.InputSystem;
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
	[SerializeField] Text detailText;

	bool isDragging = false;

	Vector2 startPoint;
	Vector2 endPoint;
	Vector2 direction;
	Vector2 force;
	float distance;
	int score;
	int cubeHits;
	int soapHits;
	GUIStyle fallbackScoreStyle;
	Vector3 ballStartPosition;
	Transform soap;
	Rigidbody2D soapBody;
	Vector3 soapStartPosition;
	Quaternion soapStartRotation;
	bool shotActive;
	bool resetting;
	float shotTime;
	float stillTime;

	//---------------------------------------
	void Start ()
	{
		cam = Camera.main;
		ballStartPosition = ball.transform.position;
		GameObject soapObject = GameObject.Find ("Cup");
		if (soapObject != null) {
			soap = soapObject.transform;
			soapBody = soapObject.GetComponent<Rigidbody2D> ();
			soapStartPosition = soap.position;
			soapStartRotation = soap.rotation;
		}
		ball.DesactivateRb ();
		if (scoreText != null) {
			scoreText.enabled = true;
			scoreText.color = new Color (0.08f, 0.08f, 0.08f, 1f);
			scoreText.fontSize = 36;
			scoreText.fontStyle = FontStyle.Bold;
		}
		if (detailText != null) {
			detailText.enabled = true;
			detailText.color = new Color (0.08f, 0.08f, 0.08f, 1f);
		}
		UpdateScoreText ();
	}

	void Update ()
	{
		if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame) {
			ResetBall ();
			return;
		}
		if (shotActive && !resetting) {
			shotTime += Time.deltaTime;
			stillTime = ball.rb.linearVelocity.sqrMagnitude < 0.04f ? stillTime + Time.deltaTime : 0f;
			if (shotTime > 12f || (shotTime > 1f && stillTime > 1.5f) || ball.transform.position.y < -7f)
				ResetBall ();
		}
		Mouse mouse = Mouse.current;
		if (mouse == null) return;
		if (!shotActive && !resetting && mouse.leftButton.wasPressedThisFrame) {
			isDragging = true;
			OnDragStart ();
		}
		if (isDragging && mouse.leftButton.wasReleasedThisFrame) {
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
		startPoint = cam.ScreenToWorldPoint (Mouse.current.position.ReadValue ());

		trajectory.Show ();
	}

	void OnDrag ()
	{
		endPoint = cam.ScreenToWorldPoint (Mouse.current.position.ReadValue ());
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

	public void ScoreCube (int points)
	{
		if (!shotActive || resetting) return;
		score += points;
		cubeHits++;
		FinishScoringShot ();
	}

	public void ScoreSoap ()
	{
		if (!shotActive || resetting) return;
		score += 50;
		soapHits++;
		FinishScoringShot ();
	}

	void FinishScoringShot ()
	{
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
		if (soap != null) {
			if (soapBody != null) {
				soapBody.linearVelocity = Vector2.zero;
				soapBody.angularVelocity = 0f;
			}
			soap.SetPositionAndRotation (soapStartPosition, soapStartRotation);
			Physics2D.SyncTransforms ();
		}
		trajectory.Hide ();
	}

	void UpdateScoreText ()
	{
		if (scoreText != null)
			scoreText.text = "SCORE: " + score;
		if (detailText != null)
			detailText.text = "CUBOS: " + cubeHits + "   SOAP: " + soapHits;
	}

	void OnGUI ()
	{
		if (scoreText != null) return;
		if (fallbackScoreStyle == null) {
			fallbackScoreStyle = new GUIStyle (GUI.skin.label);
			fallbackScoreStyle.fontSize = 36;
			fallbackScoreStyle.fontStyle = FontStyle.Bold;
			fallbackScoreStyle.normal.textColor = new Color (0.08f, 0.08f, 0.08f, 1f);
		}
		GUI.Label (new Rect (20f, 20f, 350f, 55f), "SCORE: " + score, fallbackScoreStyle);
		GUI.Label (new Rect (20f, 70f, 500f, 35f), "CUBOS: " + cubeHits + "   SOAP: " + soapHits);
	}

}
