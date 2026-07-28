using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("1. Reset Settings")]
    private Vector3 startPosition;
    private Rigidbody2D rb;
    public float resetDelay = 1f;

    [Header("2. Anti-Stuck Settings (Giải phóng khi kẹt)")]
    [Tooltip("Vận tốc dưới mức này sẽ coi là bóng đang bị dính/kẹt")]
    public float stuckVelocityThreshold = 0.6f; 

    [Tooltip("Thời gian bị kẹt (giây) trước khi tự văng ra")]
    public float timeToPop = 0.3f; 

    [Tooltip("Lực nảy bổng lên không trung")]
    public float popForceY = 8f; 

    [Tooltip("Lực văng ngang ngẫu nhiên")]
    public float popForceX = 4f; 

    private float stuckTimer = 0f;

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

    private void Update()
    {
        HandleAntiStuck();
    }

    // --- LOGIC GỠ KẸT BÓNG ---
    private void HandleAntiStuck()
    {
        if (rb == null) return;

        // Kiểm tra xem vận tốc bóng có đang quá chậm (bị người/tường ép dính) không
        if (rb.velocity.magnitude < stuckVelocityThreshold)
        {
            stuckTimer += Time.deltaTime;

            if (stuckTimer >= timeToPop)
            {
                PopBallOut();
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }
    }

    private void PopBallOut()
    {
        rb.velocity = Vector2.zero;

        // Bắn bóng văng ra ngẫu nhiên bên trái hoặc bên phải
        float randomDirX = (Random.value > 0.5f) ? 1f : -1f;
        Vector2 popVector = new Vector2(randomDirX * popForceX, popForceY);
        rb.AddForce(popVector, ForceMode2D.Impulse);

        Debug.Log("🔓 Đã tự động kích bóng văng ra khỏi vị trí kẹt!");
    }

    // --- LOGIC RESET BÓNG ---
    public void ResetPosition()
    {
        // Reset vị trí về ban đầu
        transform.position = startPosition;
        
        // Reset vận tốc và bộ đếm kẹt
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        stuckTimer = 0f;

        // Nếu có gắn BallDeformation thì reset luôn hình dạng méo của bóng về tròn
        BallDeformation deformation = GetComponent<BallDeformation>();
        if (deformation != null)
        {
            deformation.ResetShape();
        }

        Debug.Log("🔄 Đã reset vị trí bóng!");
    }

    // Nếu bóng rơi khỏi sân
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Field"))
        {
            Invoke(nameof(ResetPosition), resetDelay);
        }
    }
}