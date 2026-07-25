using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenManager : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup splashCanvasGroup;
    public RectTransform splashLogoTransform;
    public CanvasGroup mainMenuCanvasGroup;
    public RectTransform menuRectTransform;

    [Header("Timing")]
    public float logoFadeInDuration = 0.8f;
    public float logoDisplayDuration = 1.5f;
    public float logoFadeOutDuration = 0.6f;
    public float pauseInBlack = 0.2f;
    public float menuFadeInDuration = 0.6f;

    [Header("Effects")]
    public Vector3 logoStartScale = new Vector3(0.95f, 0.95f, 1f);
    public Vector3 logoEndScale = new Vector3(1.1f, 1.1f, 1f);
    public float menuSlideOffsetY = -100f;

    private Vector2 menuOriginalAnchoredPos;

    private void Start()
    {
        splashCanvasGroup.gameObject.SetActive(true);
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuMusic();

        if (menuRectTransform != null)
        {
            menuOriginalAnchoredPos = menuRectTransform.anchoredPosition;
            menuRectTransform.anchoredPosition =
                menuOriginalAnchoredPos + new Vector2(0, menuSlideOffsetY);
        }

        if (mainMenuCanvasGroup != null)
        {
            mainMenuCanvasGroup.alpha = 0;
            mainMenuCanvasGroup.interactable = false;
            mainMenuCanvasGroup.blocksRaycasts = false;
        }

        if (splashCanvasGroup != null)
        {
            splashCanvasGroup.alpha = 0;
            splashCanvasGroup.interactable = false;
            splashCanvasGroup.blocksRaycasts = false; // Tắt blocksRaycasts ban đầu để tránh đơ
        }

        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        float timer = 0;

        if (splashLogoTransform != null)
            splashLogoTransform.localScale = logoStartScale;

        // Bật Raycasts khi bắt đầu animation Intro
        if (splashCanvasGroup != null)
            splashCanvasGroup.blocksRaycasts = true;

        // Fade In
        while (timer < logoFadeInDuration)
        {
            timer += Time.deltaTime;
            float t = timer / logoFadeInDuration;

            if (splashCanvasGroup != null)
                splashCanvasGroup.alpha = Mathf.SmoothStep(0, 1, t);

            if (splashLogoTransform != null)
            {
                splashLogoTransform.localScale =
                    Vector3.Lerp(logoStartScale, logoEndScale, t * 0.3f);
            }

            yield return null;
        }

        // Display
        timer = 0;
        while (timer < logoDisplayDuration)
        {
            timer += Time.deltaTime;
            float p = (logoFadeInDuration + timer) /
                      (logoFadeInDuration + logoDisplayDuration + logoFadeOutDuration);

            if (splashLogoTransform != null)
            {
                splashLogoTransform.localScale =
                    Vector3.Lerp(logoStartScale, logoEndScale, p);
            }

            yield return null;
        }

        // Fade Out
        timer = 0;
        while (timer < logoFadeOutDuration)
        {
            timer += Time.deltaTime;
            float t = timer / logoFadeOutDuration;

            if (splashCanvasGroup != null)
                splashCanvasGroup.alpha = Mathf.SmoothStep(1, 0, t);

            yield return null;
        }

        if (splashCanvasGroup != null)
        {
            splashCanvasGroup.alpha = 0;
            splashCanvasGroup.blocksRaycasts = false; // Tắt chặn click sau khi fade out xong
        }

        yield return new WaitForSeconds(pauseInBlack);

        // Menu Fade In
        timer = 0;
        Vector2 startPos = menuOriginalAnchoredPos + new Vector2(0, menuSlideOffsetY);

        while (timer < menuFadeInDuration)
        {
            timer += Time.deltaTime;
            float t = timer / menuFadeInDuration;
            float ease = 1f - Mathf.Pow(1f - t, 3);

            if (mainMenuCanvasGroup != null)
                mainMenuCanvasGroup.alpha = Mathf.Lerp(0, 1, t);

            if (menuRectTransform != null)
            {
                menuRectTransform.anchoredPosition =
                    Vector2.Lerp(startPos, menuOriginalAnchoredPos, ease);
            }

            yield return null;
        }

        if (mainMenuCanvasGroup != null)
        {
            mainMenuCanvasGroup.alpha = 1;
            mainMenuCanvasGroup.interactable = true;
            mainMenuCanvasGroup.blocksRaycasts = true;
        }

        if (menuRectTransform != null)
            menuRectTransform.anchoredPosition = menuOriginalAnchoredPos;
    }

    public void PlayGame(string sceneName)
    {
        StartCoroutine(PlayGameRoutine(sceneName));
    }

    IEnumerator PlayGameRoutine(string sceneName)
    {
        if (AudioManager.Instance != null)
            yield return AudioManager.Instance.FadeOutMusic();

        SceneManager.LoadScene(sceneName);
    }
}