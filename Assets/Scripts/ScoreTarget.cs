using UnityEngine;

public class ScoreTarget : MonoBehaviour
{
    public int points = 10;
    void OnTriggerEnter2D (Collider2D other)
    {
        if (other.GetComponent<Ball> () != null)
            GameManager.Instance.ScoreCube (points);
    }
}
