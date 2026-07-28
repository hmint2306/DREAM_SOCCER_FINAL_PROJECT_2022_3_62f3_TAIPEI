using UnityEngine;
using UnityEngine.EventSystems;

// Cần kế thừa IPointerDownHandler và IPointerUpHandler để bắt sự kiện chuột
public class ButtonScaleEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 originalScale;
    
    [Header("Scale Settings")]
    public float clickScale = 0.95f; // Tỉ lệ thu nhỏ khi bấm (0.95 = thu nhỏ 5%)

    private void Start()
    {
        // Lưu lại kích thước ban đầu của Panel
        originalScale = transform.localScale;
    }

    // Hàm gọi ngay khi NHẤN chuột xuống
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = originalScale * clickScale;
    }

    // Hàm gọi ngay khi NHẢ chuột ra
    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localScale = originalScale;
    }
}