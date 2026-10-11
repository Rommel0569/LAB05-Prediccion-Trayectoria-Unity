using UnityEngine;

public class SoapGoalZone : MonoBehaviour
{
    BoxCollider2D zone;

    void Awake ()
    {
        zone = GetComponent<BoxCollider2D> ();
    }

    void OnTriggerStay2D (Collider2D other)
    {
        Ball ball = other.GetComponent<Ball> ();
        if (ball != null && zone.OverlapPoint (ball.rb.position))
            GameManager.Instance.ScoreSoap ();
    }
}
