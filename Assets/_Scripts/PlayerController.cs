using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    
    private Rigidbody2D rb;
    private float moveInputX = 0f;
    private int jumpCount = 0;
    
    // Đặt chính xác số lần nhảy tối đa là 2
    private int maxJumps = 2; 
    private bool isGrounded = false;
    private bool wasGrounded = false;
    
    // Tham chiếu đến banh
    public Rigidbody2D ballRb;
    public float kickForceX = 15f;  // Lực ngang
    public float kickForceY = 20f;  // Lực dọc (bay lên) - tăng lên

    [Header("Kick Settings")]
    public float kickRange = 2f;          // Phạm vi đá bóng - tăng lên cho dễ trúng
    public float kickBufferTime = 0.15f;  // Thời gian "nhớ" phím đá nếu bấm hơi sớm/trễ
    private float kickBufferTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Dùng phím WASD
        if (Input.GetKey(KeyCode.A)) moveInputX = -1;
        else if (Input.GetKey(KeyCode.D)) moveInputX = 1;
        else moveInputX = 0;

        // Flip sprite theo hướng di chuyển
        if (moveInputX > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);  // Hướng phải
        }
        else if (moveInputX < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);  // Hướng trái
        }

        RaycastHit2D hit = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y - 0.8f),
            Vector2.down,
            0.3f
        );
        
        // Kiểm tra chạm đất và không tự chạm vào chính mình
        if (hit.collider != null && hit.collider.gameObject != gameObject)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        
        // Reset jump chỉ khi landing (từ air sang ground)
        if (isGrounded && !wasGrounded)
        {
            jumpCount = 0;
        }
        
        wasGrounded = isGrounded;

        // Nhảy
        if (Input.GetKeyDown(KeyCode.W) && jumpCount < maxJumps)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;
            isGrounded = false;
        }

        // Đá bóng - dùng buffer nên không cần bấm đúng khung hình khi banh vừa vào tầm
        if (Input.GetKeyDown(KeyCode.Z))
        {
            kickBufferTimer = kickBufferTime;
        }

        if (kickBufferTimer > 0f)
        {
            kickBufferTimer -= Time.deltaTime;

            if (ballRb != null)
            {
                float distanceToBall = Vector2.Distance(transform.position, ballRb.position);

                // Không bắt buộc phải đang chạm đất nữa -> có thể đá/vô-lê cả khi đang nhảy
                if (distanceToBall < kickRange)
                {
                    KickBall();
                    kickBufferTimer = 0f;
                }
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 movement = new Vector2(moveInputX, 0) * moveSpeed;
        rb.velocity = new Vector2(movement.x, rb.velocity.y);
    }

    void KickBall()
    {
        Collider2D playerCol = GetComponent<Collider2D>();
        Collider2D ballCol = ballRb.GetComponent<Collider2D>();

        // Tắt va chạm tạm thời giữa player và banh
        // để tránh bị "đẩy" (depenetration) khi teleport banh đè lên player
        // -> đây là nguyên nhân khiến player bị nhảy lên khi sút
        if (playerCol != null && ballCol != null)
        {
            Physics2D.IgnoreCollision(playerCol, ballCol, true);
            StartCoroutine(ReenableCollisionAfterDelay(playerCol, ballCol, 0.2f));
        }

        // Di chuyển bóng tới chân player
        Vector2 kickPosition = new Vector2(transform.position.x, transform.position.y - 0.5f);
        ballRb.position = kickPosition;
        ballRb.velocity = Vector2.zero;  // Reset vận tốc bóng

        // Xác định hướng đá dựa trên hướng mặt player
        float kickDirection = transform.localScale.x;

        // Áp dụng lực sút cho bóng
        ballRb.AddForce(new Vector2(kickDirection * kickForceX, kickForceY), ForceMode2D.Impulse);
    }

    private System.Collections.IEnumerator ReenableCollisionAfterDelay(Collider2D a, Collider2D b, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (a != null && b != null)
        {
            Physics2D.IgnoreCollision(a, b, false);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 rayStart = new Vector3(transform.position.x, transform.position.y - 0.8f, 0);
        Gizmos.DrawLine(rayStart, rayStart + Vector3.down * 0.3f);

        // Vẽ phạm vi đá bóng để dễ chỉnh trong Editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, kickRange);
    }
}