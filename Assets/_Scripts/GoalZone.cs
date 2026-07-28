using UnityEngine;

public class GoalZone : MonoBehaviour
{
    public enum GoalSide { Home, Away }

    [Header("Goal Configuration")]
    public GoalSide goalSide;

    [Header("Goal Line Detection")]
    public float goalLineX = -4.8f;
    public bool isLeftGoal = true;

    private bool hasScored = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (hasScored) return;

        if (collision.CompareTag("Ball"))
        {
            float ballX = collision.transform.position.x;
            bool isBallInsideGoal = isLeftGoal ? (ballX < goalLineX) : (ballX > goalLineX);

            if (isBallInsideGoal)
            {
                hasScored = true;

                // Dừng ngay vận tốc bóng
                Rigidbody2D ballRb = collision.GetComponent<Rigidbody2D>();
                if (ballRb != null)
                {
                    ballRb.velocity *= 0.1f; 
                    ballRb.angularVelocity *= 0.1f;
                }

                // Báo cho GameManager ghi bàn VÀ DỪNG TOÀN BỘ GAME
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.ScoreGoal(goalSide);
                }
            }
        }
    }

    public void ResetGoalZone()
    {
        hasScored = false;
    }
}