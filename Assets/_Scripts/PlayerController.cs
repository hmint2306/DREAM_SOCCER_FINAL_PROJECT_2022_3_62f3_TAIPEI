using UnityEngine;
using UnityEngine.UI; // Thêm thư viện quản lý UI
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public enum PlayerIndex { Player1, Player2 }

    [Header("Player Identity")]
    public PlayerIndex playerIndex = PlayerIndex.Player1;

    [Header("Game Mode / AI Settings")]
    public bool isAI = false;

    [Header("AI Parameters (Điều chỉnh sức mạnh AI)")]
    public float aiSpeedMultiplier = 0.75f;
    public float aiKickCooldown = 1.5f;
    public float aiJumpDistanceX = 1.2f; 
    public float aiJumpHeightY = 0.9f;    
    private bool isFrozen = false;
    private float aiKickTimer = 0f;

    [Header("Sprite Direction")]
    public bool isSpriteFacingLeftByDefault = false;

    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float jumpForce = 8f;

    private Rigidbody2D rb;
    private float moveInputX = 0f;

    [Header("Ground Check Settings")]
    public Transform groundCheckPoint; 
    public Vector2 groundCheckSize = new Vector2(0.8f, 0.2f);
    public LayerMask groundLayer;
    
    private int jumpCount = 0;
    private int maxJumps = 1;
    private bool isGrounded = false;

    [Header("Normal Kick Settings")]
    public Rigidbody2D ballRb;
    public float kickForceX = 10f;
    public float kickForceY = 12f;
    public float kickRange = 2f;
    public float kickBufferTime = 0.15f;
    private float kickBufferTimer = 0f;

    [Header("Skill Settings - Fire Kick")]
    public float skillCooldown = 10f;       
    private float skillTimer = 0f;          
    public float fireKickForceX = 18f;      
    public float fireKickForceY = 16f;      
    public GameObject fireVFXPrefab;        
    private bool skillKickBuffer = false;   

    [Header("UI Settings")]
    public Image cooldownImage; // Biến chứa ảnh UI Cooldown đếm ngược

    private float facingDirection = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        facingDirection = isSpriteFacingLeftByDefault ? -1f : 1f;

        int gameMode = PlayerPrefs.GetInt("GameMode", 1);
        if (playerIndex == PlayerIndex.Player2)
        {
            isAI = (gameMode == 1);
        }
    }

    void Update()
    {
        if (isFrozen)
        {
            moveInputX = 0f;
            return;
        }

        bool jumpPressed = false;
        bool kickPressed = false;
        bool skillKickPressed = false;

        // Giảm thời gian chờ sút của AI và hồi chiêu Skill
        if (aiKickTimer > 0f) aiKickTimer -= Time.deltaTime;
        if (skillTimer > 0f) 
        {
            skillTimer -= Time.deltaTime;
        }

        // Cập nhật UI Cooldown (Nếu đã gán UI)
        if (cooldownImage != null)
        {
            // Hiển thị phần trăm thời gian còn lại (1 là đầy, 0 là đã hồi xong)
            cooldownImage.fillAmount = skillTimer / skillCooldown;
        }

        if (isAI)
        {
            HandleAIInput(out jumpPressed, out kickPressed, out skillKickPressed);
        }
        else
        {
            HandleHumanInput(out jumpPressed, out kickPressed, out skillKickPressed);
        }

        // TỰ ĐỘNG QUAY MẶT THEO BÓNG
        if (ballRb != null)
        {
            float xOffset = ballRb.transform.position.x - transform.position.x;
            if (xOffset > 0.35f && facingDirection != 1f)
            {
                SetFacingDirection(1f);
            }
            else if (xOffset < -0.35f && facingDirection != -1f)
            {
                SetFacingDirection(-1f);
            }
        }

        // KIỂM TRA CHẠM ĐẤT
        Vector3 checkPos = (groundCheckPoint != null) ? groundCheckPoint.position : new Vector3(transform.position.x, transform.position.y - 0.9f, 0);
        Collider2D[] colliders = Physics2D.OverlapBoxAll(checkPos, groundCheckSize, 0f);
        isGrounded = false;

        foreach (var col in colliders)
        {
            if (col.gameObject != gameObject && col.attachedRigidbody != ballRb)
            {
                isGrounded = true;
                break;
            }
        }

        if (isGrounded) jumpCount = 0;

        // XỬ LÝ NHẢY
        if (jumpPressed && (isGrounded || jumpCount < maxJumps))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;
            isGrounded = false;
        }

        // XỬ LÝ SÚT BÓNG (Ưu tiên Skill nếu được bấm và đã hồi xong)
        if (skillKickPressed && skillTimer <= 0f)
        {
            kickBufferTimer = kickBufferTime;
            skillKickBuffer = true;
            skillTimer = skillCooldown; // Bắt đầu đếm ngược thời gian hồi chiêu
        }
        else if (kickPressed)
        {
            kickBufferTimer = kickBufferTime;
            skillKickBuffer = false;
        }

        if (kickBufferTimer > 0f)
        {
            kickBufferTimer -= Time.deltaTime;
            if (ballRb != null)
            {
                float distanceToBall = Vector2.Distance(transform.position, ballRb.position);
                if (distanceToBall < kickRange)
                {
                    KickBall(skillKickBuffer);
                    kickBufferTimer = 0f;
                    skillKickBuffer = false; 
                }
            }
        }
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(moveInputX * moveSpeed, rb.velocity.y);
    }

    private void HandleHumanInput(out bool jumpPressed, out bool kickPressed, out bool skillKickPressed)
    {
        if (playerIndex == PlayerIndex.Player1)
        {
            if (Input.GetKey(KeyCode.A)) moveInputX = -1f;
            else if (Input.GetKey(KeyCode.D)) moveInputX = 1f;
            else moveInputX = 0f;

            jumpPressed = Input.GetKeyDown(KeyCode.W);
            kickPressed = Input.GetKeyDown(KeyCode.Z);
            skillKickPressed = Input.GetKeyDown(KeyCode.C); 
        }
        else 
        {
            if (Input.GetKey(KeyCode.LeftArrow)) moveInputX = -1f;
            else if (Input.GetKey(KeyCode.RightArrow)) moveInputX = 1f;
            else moveInputX = 0f;

            jumpPressed = Input.GetKeyDown(KeyCode.UpArrow);
            kickPressed = Input.GetKeyDown(KeyCode.J);
            skillKickPressed = Input.GetKeyDown(KeyCode.K); 
        }
    }

    public void FreezePlayer()
    {
       isFrozen = true;
       moveInputX = 0f;

       if (rb != null)
       {
           rb.velocity = Vector2.zero;
           rb.angularVelocity = 0f;
       }
    }

    public void UnfreezePlayer()
    {
        isFrozen = false;
    }

    private void HandleAIInput(out bool jumpPressed, out bool kickPressed, out bool skillKickPressed)
    {
        jumpPressed = false;
        kickPressed = false;
        skillKickPressed = false;

        if (ballRb == null) return;

        float xDiff = ballRb.transform.position.x - transform.position.x;
        float yDiff = ballRb.transform.position.y - transform.position.y;
        float distanceToBall = Vector2.Distance(transform.position, ballRb.position);

        if (xDiff > 0.4f) moveInputX = 1f * aiSpeedMultiplier;
        else if (xDiff < -0.4f) moveInputX = -1f * aiSpeedMultiplier;
        else moveInputX = 0f;

        if (yDiff > aiJumpHeightY && Mathf.Abs(xDiff) < aiJumpDistanceX)
        {
            jumpPressed = true;
        }

        if (distanceToBall <= kickRange && aiKickTimer <= 0f)
        {
            if (skillTimer <= 0f)
            {
                skillKickPressed = true; 
            }
            else
            {
                kickPressed = true;      
            }
            aiKickTimer = aiKickCooldown; 
        }
    }

    private void SetFacingDirection(float dir)
    {
        facingDirection = dir;
        Vector3 scaler = transform.localScale;
        float targetScaleX = isSpriteFacingLeftByDefault ? -dir : dir;
        scaler.x = Mathf.Abs(scaler.x) * targetScaleX;
        transform.localScale = scaler;
    }

    void KickBall(bool isFireKick)
    {
        Collider2D playerCol = GetComponent<Collider2D>();
        Collider2D ballCol = ballRb.GetComponent<Collider2D>();
    
        if (playerCol != null && ballCol != null)
        {
            Physics2D.IgnoreCollision(playerCol, ballCol, true);
            StartCoroutine(ReenableCollisionAfterDelay(playerCol, ballCol, 0.2f));
        }
    
        Vector2 kickPosition = transform.position + new Vector3(facingDirection * 0.8f, -0.2f, 0);
        ballRb.position = kickPosition;
    
        ballRb.velocity = Vector2.zero;
        ballRb.angularVelocity = 0f;

        Vector2 kickVector;

        if (isFireKick)
        {
            kickVector = new Vector2(facingDirection * fireKickForceX, fireKickForceY);
            
            if (fireVFXPrefab != null)
            {
                GameObject vfx = Instantiate(fireVFXPrefab, ballRb.transform.position, Quaternion.identity, ballRb.transform);
                Destroy(vfx, 1.5f);
            }
            Debug.Log($"🔥 {gameObject.name} đã tung CÚ SÚT LỬA!");
        }
        else
        {
            kickVector = new Vector2(facingDirection * kickForceX, kickForceY);
        }
    
        ballRb.AddForce(kickVector, ForceMode2D.Impulse);
    }

    private IEnumerator ReenableCollisionAfterDelay(Collider2D a, Collider2D b, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (a != null && b != null)
        {
            Physics2D.IgnoreCollision(a, b, false);
        }
    }
}