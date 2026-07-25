using UnityEngine;

public class Ball : MonoBehaviour
{
    private Vector3 startPosition;
    private Rigidbody2D rb;
    private float resetDelay = 1f;

    private void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        
        // Thêm tag "Ball" nếu chưa có
        if (!gameObject.CompareTag("Ball"))
        {
            gameObject.tag = "Ball";
        }
    }

    public void ResetPosition()
    {
        // Reset vị trí về vị trí ban đầu
        transform.position = startPosition;
        
        // Reset vận tốc
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        Debug.Log("🔄 Đã reset vị trí bóng!");
    }

    // Nếu bóng rơi khỏi sân
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Field"))
        {
            Invoke("ResetPosition", resetDelay);
        }
    }
}