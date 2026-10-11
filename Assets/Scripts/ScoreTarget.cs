using UnityEngine;

public class ScoreTarget : MonoBehaviour
{
    public int points = 10;
    void OnCollisionEnter2D (Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Ball> () != null)
            GameManager.Instance.ScoreCube (points);
    }
}
