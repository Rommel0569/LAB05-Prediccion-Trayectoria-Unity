
using UnityEngine;

public class Cup : MonoBehaviour
{
	float lastScoreTime = -10f;
	void OnCollisionEnter2D (Collision2D collision)
	{
		if (collision.gameObject.GetComponent<Ball> () != null && Time.time - lastScoreTime > 0.5f) {
			GameManager.Instance.AddPoint ();
			lastScoreTime = Time.time;
		}
	}
}
