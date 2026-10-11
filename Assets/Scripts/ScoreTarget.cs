using UnityEngine;

public class ScoreTarget : MonoBehaviour
{
    void OnTriggerEnter2D (Collider2D other)
    {
        if (other.GetComponent<Ball> () != null)
            GameManager.Instance.AddPoint ();
    }
}
