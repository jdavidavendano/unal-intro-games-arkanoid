using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.TryGetComponent<Ball>(out Ball ball))
        {
            ArkanoidEvent.OnBallReachDeadZoneEvent?.Invoke(ball);
        }
    }
}