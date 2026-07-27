using UnityEngine;

public class BallDeformation : MonoBehaviour
{
    [Header("References")]
    public Transform visualTransform; // Kéo GameObject con chứa SpriteRenderer vào đây
    public Rigidbody2D rb;

    [Header("Deformation Physics")]
    public float deformationFactor = 0.05f; // Hệ số biến dạng theo lực
    public float maxDeformation = 0.4f;     // Giới hạn biến dạng tối đa
    public float recoverySpeed = 15f;       // Tốc độ đàn hồi về trạng thái cũ

    private Vector3 originalScale;

    void Start()
    {
        originalScale = visualTransform.localScale;
    }

    void Update()
    {
        // Áp dụng định luật Hooke mô phỏng lò xo để phục hồi hình dáng
        visualTransform.localScale = Vector3.Lerp(visualTransform.localScale, originalScale, Time.deltaTime * recoverySpeed);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Tính toán động năng tương đối của va chạm
        float impactForce = collision.relativeVelocity.magnitude;

        if (impactForce > 1f)
        {
            // Vector pháp tuyến của bề mặt va chạm
            Vector2 contactNormal = collision.contacts[0].normal;

            // Tính toán biến lượng nén (k)
            float squashAmount = Mathf.Clamp(impactForce * deformationFactor, 0f, maxDeformation);
            float k = 1f - squashAmount;

            // Tính toán kích thước mới dựa trên công thức bảo toàn diện tích (h' = h/k)
            float stretchAmount = 1f / k;

            Vector3 newScale = originalScale;

            // Định hướng biến dạng dựa trên trục va chạm
            if (Mathf.Abs(contactNormal.x) > Mathf.Abs(contactNormal.y))
            {
                // Va chạm theo trục X (vào tường dọc)
                newScale.x *= k;
                newScale.y *= stretchAmount;
            }
            else
            {
                // Va chạm theo trục Y (xuống mặt đất)
                newScale.x *= stretchAmount;
                newScale.y *= k;
            }

            visualTransform.localScale = newScale;
        }
    }
}