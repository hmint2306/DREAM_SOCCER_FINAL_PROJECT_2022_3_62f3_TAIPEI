using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    
    private Rigidbody2D rb;
    private float moveInputX = 0f;
    private int jumpCount = 0;
    private int maxJumps = 2;
    private bool isGrounded = false;

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

        // Kiểm tra xem có chạm đất không - raycast từ dưới chân player
        RaycastHit2D hit = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y - 0.5f),
            Vector2.down,
            0.2f
        );
        
        if (hit.collider != null)
        {
            isGrounded = true;
            Debug.Log("Player 1 HIT GROUND: " + hit.collider.name);
        }
        else
        {
            isGrounded = false;
        }
        
        // Reset jump khi chạm đất
        if (isGrounded && jumpCount > 0)
        {
            jumpCount = 0;
        }

        // Nhảy
        if (Input.GetKeyDown(KeyCode.W) && jumpCount < maxJumps)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        Vector2 movement = new Vector2(moveInputX, 0) * moveSpeed;
        rb.velocity = new Vector2(movement.x, rb.velocity.y);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 rayStart = new Vector3(transform.position.x, transform.position.y - 0.5f, 0);
        Gizmos.DrawLine(rayStart, rayStart + Vector3.down * 0.2f);
    }
}