using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    private Rigidbody2D rb;
    private float moveInputX = 0f;
    private bool facingRight = true;

    // Các biến cho tính năng nhảy đôi
    private int jumpCount = 0;
    private int maxJumps = 2;
    private bool isGrounded = false;
    private bool wasGrounded = false;

    [Header("Kick Settings")]
    public Rigidbody2D ballRb;
    public float kickForceX = 15f;
    public float kickForceY = 20f;
    public float kickRange = 2f;
    public float kickBufferTime = 0.15f;
    private float kickBufferTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 1. DI CHUYỂN BẰNG A/D
        if (Input.GetKey(KeyCode.A)) moveInputX = -1f;
        else if (Input.GetKey(KeyCode.D)) moveInputX = 1f;
        else moveInputX = 0f;

        // 2. TỰ ĐỘNG QUAY MẶT THEO QUẢ BÓNG
        if (ballRb != null)
        {
            if (ballRb.transform.position.x > transform.position.x && !facingRight)
            {
                Flip();
            }
            else if (ballRb.transform.position.x < transform.position.x && facingRight)
            {
                Flip();
            }
        }

        // 3. KIỂM TRA CHẠM ĐẤT (Đồng bộ logic giống hệt Player 2)
        RaycastHit2D hit = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y - 0.8f),
            Vector2.down,
            0.3f
        );

        if (hit.collider != null && hit.collider.gameObject != gameObject)
            isGrounded = true;
        else
            isGrounded = false;

        // Reset số lần nhảy
        if (isGrounded && !wasGrounded)
        {
            jumpCount = 0;
        }
        wasGrounded = isGrounded;

        // 4. XỬ LÝ NHẢY (W) - CHO PHÉP NHẢY ĐÔI
        if (Input.GetKeyDown(KeyCode.W) && jumpCount < maxJumps)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;
            isGrounded = false;
        }

        // 5. XỬ LÝ SÚT BÓNG (NÚT Z)
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

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
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

        float kickDirection = facingRight ? 1f : -1f;
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
        // Vẽ tia Raycast chạm đất (màu vàng)
        Gizmos.color = Color.yellow;
        Vector3 rayStart = new Vector3(transform.position.x, transform.position.y - 0.8f, 0);
        Gizmos.DrawLine(rayStart, rayStart + Vector3.down * 0.3f);

        // Vẽ phạm vi sút bóng (màu xanh lá)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, kickRange);
    }
}