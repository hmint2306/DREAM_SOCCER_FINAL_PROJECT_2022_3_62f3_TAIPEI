using UnityEngine;
using UnityEngine.UI;

public class GlobalSettings : MonoBehaviour
{
    public static GlobalSettings Instance { get; private set; }

    [Header("UI Reference (Tùy chọn)")]
    [Tooltip("Kéo Slider Brightness trong Scene Settings vào đây (nếu có)")]
    public Slider brightnessSlider;

    [Header("Settings Data")]
    [Range(0f, 1f)]
    public float brightness = 1f; // 1 = Sáng nhất, 0 = Tối nhất

    // Component lưu trữ nội bộ
    private Image brightnessImage;

    private void Awake()
    {
        // 1. Singleton & DontDestroyOnLoad
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 2. TỰ ĐỘNG TẠO OVERLAY NẾU CHƯA CÓ (Dành cho trường hợp Play thẳng từ Scene Settings)
            EnsureBrightnessOverlayExists();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Áp dụng độ sáng ban đầu
        ApplyBrightness(brightness);

        // Nếu Slider có sẵn trong Inspector thì tự động đăng ký Event
        if (brightnessSlider != null)
        {
            SetupBrightnessSlider(brightnessSlider);
        }
    }

    /// <summary>
    /// Tự động kiểm tra và vẽ Canvas Overlay phủ kín màn hình nếu chưa có
    /// </summary>
    private void EnsureBrightnessOverlayExists()
    {
        if (brightnessImage != null) return;

        // Tìm xem trong Scene đã có Canvas Overlay chưa
        GameObject existingCanvas = GameObject.Find("Global_BrightnessCanvas");

        if (existingCanvas == null)
        {
            // TỰ TẠO CANVAS OVERLAY MỚI
            existingCanvas = new GameObject("Global_BrightnessCanvas");
            DontDestroyOnLoad(existingCanvas);

            // Set RenderMode
            Canvas canvas = existingCanvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999; // Đảm bảo luôn nằm trên cùng UI

            existingCanvas.AddComponent<CanvasScaler>();
            existingCanvas.AddComponent<GraphicRaycaster>();

            // Tạo Panel Image màu đen làm mờ
            GameObject imgObj = new GameObject("BrightnessImage");
            imgObj.transform.SetParent(existingCanvas.transform, false);

            brightnessImage = imgObj.AddComponent<Image>();
            brightnessImage.color = Color.black;
            brightnessImage.raycastTarget = false; // Tắt Raycast để không cản click mouse/touch UI bên dưới

            // Co giãn kín màn hình
            RectTransform rect = imgObj.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
        }
        else
        {
            brightnessImage = existingCanvas.GetComponentInChildren<Image>();
            DontDestroyOnLoad(existingCanvas);
        }
    }

    /// <summary>
    /// Hàm gọi khi kéo Slider Brightness (Giá trị val từ 0.0 đến 1.0)
    /// </summary>
    public void SetBrightness(float val)
    {
        brightness = Mathf.Clamp01(val);
        ApplyBrightness(brightness);
    }

    private void ApplyBrightness(float val)
    {
        if (brightnessImage != null)
        {
            // Sáng 100% (val = 1) -> Alpha nền đen = 0 (Trong suốt)
            // Tối 0% (val = 0) -> Alpha nền đen = 0.8 (Tối hẳn)
            float alpha = (1f - val) * 0.8f;

            Color c = brightnessImage.color;
            c.a = alpha;
            brightnessImage.color = c;
        }
    }

    /// <summary>
    /// Hàm kết nối Slider UI từ Scene Settings vào GlobalSettings
    /// </summary>
    public void SetupBrightnessSlider(Slider slider)
    {
        brightnessSlider = slider;
        if (brightnessSlider != null)
        {
            brightnessSlider.value = brightness;
            brightnessSlider.onValueChanged.RemoveAllListeners();
            brightnessSlider.onValueChanged.AddListener(SetBrightness);
        }
    }
}