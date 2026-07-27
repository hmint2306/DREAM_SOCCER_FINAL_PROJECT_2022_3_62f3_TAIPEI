using UnityEngine;

public class BallDeformation : MonoBehaviour
{
    [Header("References")]
    public Transform visualTransform; // Trỏ vào BallVisual
    public Rigidbody2D rb;

    [Header("Deformation Physics")]
    public float deformationFactor = 0.05f; 
    public float maxDeformation = 0.4f;     
    
    [Header("Viscoelastic Mechanics")]
    public float springForce = 50f; // Độ cứng của "lò xo" cao su (k)
    public float damping = 5f;      // Hệ số cản/tắt dần (c)

    private Vector3 originalScale;
    private Vector3 currentScaleVelocity;
    private Vector3 currentScale;

    void Start()
    {
        originalScale = visualTransform.localScale;
        currentScale = originalScale;
    }

    void Update()
    {
        // 1. Tính toán độ lệch so với hình dáng gốc (Displacement: x)
        Vector3 displacement = originalScale - currentScale;
        
        // 2. Tính lực đàn hồi của lò xo (Spring Force: -kx)
        Vector3 springAccel = displacement * springForce;
        
        // 3. Cập nhật vận tốc biến dạng và áp dụng lực cản (Damping: -cv)
        currentScaleVelocity += springAccel * Time.deltaTime;
        currentScaleVelocity -= currentScaleVelocity * damping * Time.deltaTime;
        
        // 4. Áp dụng vận tốc để thay đổi kích thước hiện tại
        currentScale += currentScaleVelocity * Time.deltaTime;
        visualTransform.localScale = currentScale;

        // Phục hồi góc xoay về 0 khi động năng biến dạng đã tắt dần
        if (displacement.magnitude < 0.01f)
        {
            visualTransform.localRotation = Quaternion.Lerp(visualTransform.localRotation, Quaternion.identity, Time.deltaTime * 10f);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Tính toán động năng tương đối
        float impactForce = collision.relativeVelocity.magnitude;

        if (impactForce > 1f)
        {
            // Vector pháp tuyến của bề mặt va chạm
            Vector2 contactNormal = collision.contacts[0].normal;

            // ĐỘNG HỌC QUAY (Kinematics): Tính góc xoay bằng hàm Atan2
            float angle = Mathf.Atan2(contactNormal.y, contactNormal.x) * Mathf.Rad2Deg;
            
            // Xoay trục Y của visualTransform song song với vector pháp tuyến
            visualTransform.rotation = Quaternion.Euler(0, 0, angle - 90f);

            // TÍNH TOÁN BIẾN DẠNG:
            float squashAmount = Mathf.Clamp(impactForce * deformationFactor, 0f, maxDeformation);
            float k = 1f - squashAmount; // Hệ số nén
            float stretchAmount = 1f / k; // Hệ số giãn (bảo toàn diện tích bề mặt)

            // Áp dụng lực nén (dọc theo vector pháp tuyến Y) và giãn (dọc theo mặt phẳng tiếp xúc X)
            currentScale = new Vector3(originalScale.x * stretchAmount, originalScale.y * k, originalScale.z);
            
            // Reset vận tốc biến dạng để bắt đầu chu kỳ dao động mới
            currentScaleVelocity = Vector3.zero; 
        }
    }
}