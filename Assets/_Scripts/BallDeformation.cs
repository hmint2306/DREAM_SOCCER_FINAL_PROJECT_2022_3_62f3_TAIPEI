using UnityEngine;

public class BallDeformation : MonoBehaviour
{
    [Header("References")]
    public Transform visualTransform; // Trỏ vào BallVisual (hoặc tự lấy chính nó)
    public Rigidbody2D rb;

    [Header("Deformation Limits (Giới hạn độ méo)")]
    [Range(0.01f, 0.2f)]
    public float maxSquish = 0.15f;    // Tối đa móp 15%
    public float minImpactForce = 2.5f; // Lực va chạm tối thiểu mới nhún

    [Header("Spring Physics (Lực lò xo)")]
    public float springForce = 120f;    // Độ cứng lò xo
    public float damping = 12f;         // Độ dập tắt rung

    private Vector3 originalScale;
    private Vector3 currentScale;
    private Vector3 currentScaleVelocity;

    void Start()
    {
        if (visualTransform == null) visualTransform = transform;
        if (rb == null) rb = GetComponentInParent<Rigidbody2D>();

        originalScale = visualTransform.localScale;
        currentScale = originalScale;
    }

    void Update()
    {
        // Mô phỏng lò xo đàn hồi đưa bóng về lại hình tròn
        Vector3 displacement = originalScale - currentScale;
        Vector3 springAccel = displacement * springForce;

        currentScaleVelocity += springAccel * Time.deltaTime;
        currentScaleVelocity -= currentScaleVelocity * damping * Time.deltaTime;

        currentScale += currentScaleVelocity * Time.deltaTime;
        visualTransform.localScale = currentScale;

        // Trả góc xoay cục bộ của sprite về 0
        visualTransform.localRotation = Quaternion.Lerp(visualTransform.localRotation, Quaternion.identity, Time.deltaTime * 15f);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        float impactForce = collision.relativeVelocity.magnitude;

        if (impactForce < minImpactForce) return;

        float squishAmount = Mathf.Clamp((impactForce - minImpactForce) * 0.015f, 0f, maxSquish);
        if (squishAmount <= 0f) return;

        Vector2 contactNormal = collision.contacts[0].normal;

        // Đập theo phương đứng
        if (Mathf.Abs(contactNormal.y) > Mathf.Abs(contactNormal.x))
        {
            currentScale = new Vector3(
                originalScale.x * (1f + squishAmount * 0.5f),
                originalScale.y * (1f - squishAmount),
                originalScale.z
            );
        }
        else // Đập theo phương ngang
        {
            currentScale = new Vector3(
                originalScale.x * (1f - squishAmount),
                originalScale.y * (1f + squishAmount * 0.5f),
                originalScale.z
            );
        }

        currentScaleVelocity = Vector3.zero;
    }

    // Hàm hỗ trợ đưa bóng về tròn xịn mịn khi Reset
    public void ResetShape()
    {
        currentScale = originalScale;
        currentScaleVelocity = Vector3.zero;
        if (visualTransform != null) visualTransform.localScale = originalScale;
    }
}