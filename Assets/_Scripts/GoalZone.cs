using UnityEngine;

public class GoalZone : MonoBehaviour
{
    public enum GoalSide { Home, Away }
    public GoalSide goalSide;
    
    private bool hasScored = false;
    private float resetDelay = 2f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra xem vật thể va chạm có phải là bóng không
        if (collision.CompareTag("Ball") && !hasScored)
        {
            hasScored = true;
            
            // Gọi hàm ghi bàn trong GameManager
            GameManager.Instance.ScoreGoal(goalSide);
            
            Debug.Log($"⚽ GHI BÀN! Đội {goalSide} ghi bàn!");
            
            // Reset ball sau một thời gian
            Invoke("ResetBall", resetDelay);
        }
    }

    private void ResetBall()
    {
        hasScored = false;
        Ball ball = FindObjectOfType<Ball>();
        if (ball != null)
        {
            ball.ResetPosition();
        }
    }
}