using System.Collections;
using UnityEngine;
using TMPro;

public class PressAnyKeyMenu : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI pressKeyText; // Kéo 'PressKey' vào đây
    public GameObject menuContainer;     // Kéo 'Menu' vào đây

    [Header("Blink Settings")]
    public float blinkSpeed = 2f;         
    public float minAlpha = 0.1f;         

    [Header("Juicy Menu Appearance")]
    public float delayBeforeMenu = 0.15f; // Độ trễ ngắn (giây)
    public float animDuration = 0.35f;   // Thời gian bật menu (giây)
    public Vector3 startScale = new Vector3(0.8f, 0.8f, 1f); // Kích thước ban đầu (nhỏ hơn 1 chút)

    private bool isMenuOpen = false;
    private Color originalColor;
    private CanvasGroup menuCanvasGroup;
    private RectTransform menuRectTransform;
    private Coroutine menuAnimationCoroutine;

    void Start()
    {
        if (pressKeyText == null)
            pressKeyText = GetComponent<TextMeshProUGUI>();

        if (pressKeyText != null) 
        {
            pressKeyText.enabled = true;
            originalColor = pressKeyText.color;
        }

        if (menuContainer != null) 
        {
            menuCanvasGroup = menuContainer.GetComponent<CanvasGroup>();
            if (menuCanvasGroup == null)
                menuCanvasGroup = menuContainer.AddComponent<CanvasGroup>();

            menuRectTransform = menuContainer.GetComponent<RectTransform>();

            // Mặc định ban đầu ẩn
            menuCanvasGroup.alpha = 0f;
            menuCanvasGroup.interactable = false;
            menuCanvasGroup.blocksRaycasts = false;
            
            if (menuRectTransform != null)
                menuRectTransform.localScale = startScale;
        }

        isMenuOpen = false;
    }

    void Update()
    {
        if (!isMenuOpen && pressKeyText != null && pressKeyText.enabled)
        {
            float alpha = Mathf.Lerp(minAlpha, 1f, Mathf.PingPong(Time.time * blinkSpeed, 1f));
            Color newColor = originalColor;
            newColor.a = alpha;
            pressKeyText.color = newColor;
        }

        HandleInput();
    }

    void HandleInput()
    {
        if (!isMenuOpen)
        {
            if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape))
            {
                OpenMenu();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseMenu();
            }
        }
    }

    public void OpenMenu()
    {
        isMenuOpen = true;
        
        if (pressKeyText != null) 
            pressKeyText.enabled = false; 

        if (menuAnimationCoroutine != null) 
            StopCoroutine(menuAnimationCoroutine);

        menuAnimationCoroutine = StartCoroutine(AnimateMenu(true));
    }

    public void CloseMenu()
    {
        isMenuOpen = false;

        if (menuAnimationCoroutine != null) 
            StopCoroutine(menuAnimationCoroutine);

        menuAnimationCoroutine = StartCoroutine(AnimateMenu(false));
    }

    private IEnumerator AnimateMenu(bool isOpen)
    {
        if (isOpen)
        {
            yield return new WaitForSeconds(delayBeforeMenu);

            float timer = 0f;
            while (timer < animDuration)
            {
                timer += Time.deltaTime;
                float progress = timer / animDuration;

                // Hàm Ease Out Back: Tạo cảm giác bung ra và nảy nhẹ ở điểm dừng
                float easeProgress = EaseOutBack(progress);

                if (menuCanvasGroup != null)
                    menuCanvasGroup.alpha = Mathf.Clamp01(progress * 2f); // Hiện rõ nhanh

                if (menuRectTransform != null)
                    menuRectTransform.localScale = Vector3.LerpUnclamped(startScale, Vector3.one, easeProgress);

                yield return null;
            }

            if (menuCanvasGroup != null)
            {
                menuCanvasGroup.alpha = 1f;
                menuCanvasGroup.interactable = true;
                menuCanvasGroup.blocksRaycasts = true;
            }
            if (menuRectTransform != null)
                menuRectTransform.localScale = Vector3.one;
        }
        else
        {
            if (menuCanvasGroup != null)
            {
                menuCanvasGroup.interactable = false;
                menuCanvasGroup.blocksRaycasts = false;
            }

            float timer = 0f;
            Vector3 currentScale = menuRectTransform != null ? menuRectTransform.localScale : Vector3.one;
            float startAlpha = menuCanvasGroup != null ? menuCanvasGroup.alpha : 1f;

            while (timer < animDuration * 0.7f) // Thu về nhanh hơn một chút
            {
                timer += Time.deltaTime;
                float progress = timer / (animDuration * 0.7f);

                if (menuCanvasGroup != null)
                    menuCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, progress);

                if (menuRectTransform != null)
                    menuRectTransform.localScale = Vector3.Lerp(currentScale, startScale, progress);

                yield return null;
            }

            if (menuCanvasGroup != null)
                menuCanvasGroup.alpha = 0f;

            if (pressKeyText != null) 
                pressKeyText.enabled = true;
        }
    }

    // Công thức toán học Ease Out Back (Nảy nhẹ sinh động)
    private float EaseOutBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(x - 1f, 3) + c1 * Mathf.Pow(x - 1f, 2);
    }
}