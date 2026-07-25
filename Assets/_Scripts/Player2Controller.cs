using UnityEngine;
using System.Collections;

public class Player2Controller : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    private Rigidbody2D rb;
    private float moveInputX = 0f;

    private int jumpCount = 0;
    private int maxJumps = 2;
    private bool isGrounded = false;
    private bool wasGrounded = false;

    [Header("Kick Settings")]
    public Rigidbody2D ballRb;
    public float kickForceX = 10f;  // ← Giảm từ 15 xuống 10 (lực sút nhẹ hơn)
    public float kickForceY = 12f;  // ← Giảm từ 20 xuống 12 (parabol ít cong hơn)
    public float kickRange = 2f;
    public float kickBufferTime = 0.15f;
    private float kickBufferTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 1. DI CHUYỂN BẰNG MŨI TÊN
        if (Input.GetKey(KeyCode.LeftArrow)) moveInputX = -1f;
        else if (Input.GetKey(KeyCode.RightArrow)) moveInputX = 1f;
        else moveInputX = 0f;

        // 2. TỰ ĐỘNG QUAY MẶT THEO QUẢ BÓNG (Giữ nguyên kích thước thực của nhân vật)
        if (ballRb != null)
        {
            Vector3 currentScale = transform.localScale;

            if (ballRb.transform.position.x > transform.position.x)
            {
                // R7 nhìn sang phải -> scale âm. Lấy trị tuyệt đối rồi nhân -1
                currentScale.x = -Mathf.Abs(currentScale.x);
            }
            else if (ballRb.transform.position.x < transform.position.x)
            {
                // R7 nhìn sang trái -> scale dương. Lấy trị tuyệt đối
                currentScale.x = Mathf.Abs(currentScale.x);
            }

            transform.localScale = currentScale;
        }

        // 3. KIỂM TRA CHẠM ĐẤT
        RaycastHit2D hit = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y - 0.8f),
            Vector2.down,
            0.3f
        );

        if (hit.collider != null && hit.collider.gameObject != gameObject)
            isGrounded = true;
        else
            isGrounded = false;

        if (isGrounded && !wasGrounded)
        {
            jumpCount = 0;
        }
        wasGrounded = isGrounded;

        // 4. NHẢY BẰNG MŨI TÊN LÊN
        if (Input.GetKeyDown(KeyCode.UpArrow) && jumpCount < maxJumps)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;
            isGrounded = false;
        }

        // 5. XỬ LÝ SÚT BÓNG (NÚT J)
        if (Input.GetKeyDown(KeyCode.J))
        {
            kickBufferTimer = kickBufferTime;
        }

        if (kickBufferTimer > 0f)
        {
            kickBufferTimer -= Time.deltaTime;
            if (ballRb != null)
            {
                float distanceToBall = Vector2.Distance(transform.position, ballRb.position);
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

        if (playerCol != null && ballCol != null)
        {
            Physics2D.IgnoreCollision(playerCol, ballCol, true);
            StartCoroutine(ReenableCollisionAfterDelay(playerCol, ballCol, 0.2f));
        }

        Vector2 kickPosition = new Vector2(transform.position.x, transform.position.y - 0.5f);
        ballRb.position = kickPosition;
        ballRb.velocity = Vector2.zero;

        // Dùng Mathf.Sign để chỉ lấy dấu (1 hoặc -1) của Hướng, tránh việc kích thước nhân vật làm yếu lực sút
        float kickDirection = Mathf.Sign(-transform.localScale.x);
        ballRb.AddForce(new Vector2(kickDirection * kickForceX, kickForceY), ForceMode2D.Impulse);
    }

    private IEnumerator ReenableCollisionAfterDelay(Collider2D a, Collider2D b, float delay)
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

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, kickRange);
    }
}