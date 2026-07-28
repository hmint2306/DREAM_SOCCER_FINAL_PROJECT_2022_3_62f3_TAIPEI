using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource; // Dành riêng cho nhạc nền
    public AudioSource sfxSource; // Dành riêng cho hiệu ứng (Goal, còi, v.v.)

    [Header("BGM Settings")]
    public AudioClip menuMusic;
    public float fadeDuration = 0.8f;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float menuVolume = 1f;       // Âm lượng nhạc nền khi ở Menu
    [Range(0f, 1f)] public float gameplayVolume = 0.35f; // Âm lượng nhạc nền khi đá bóng (35%)

    [Header("Goal Ducking Settings")]
    [Range(0f, 1f)] public float goalBgmVolume = 0f;    // Ngắt nhạc nền hoàn toàn khi có GOAL để tạo điểm nhấn

    private Coroutine volumeFadeCoroutine;
    private Coroutine goalDuckingCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Tự động tìm hoặc bổ sung AudioSource nếu chưa gán
            AudioSource[] sources = GetComponents<AudioSource>();
            if (bgmSource == null)
            {
                bgmSource = (sources.Length > 0) ? sources[0] : gameObject.AddComponent<AudioSource>();
            }
            if (sfxSource == null)
            {
                sfxSource = (sources.Length > 1) ? sources[1] : gameObject.AddComponent<AudioSource>();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Contains("16") || scene.buildIndex == 16)
        {
            SetBGMVolumeSmooth(gameplayVolume);
        }
        else
        {
            SetBGMVolumeSmooth(menuVolume);
        }
    }

    public void PlayMenuMusic()
    {
        if (bgmSource == null || menuMusic == null) return;

        if (bgmSource.isPlaying && bgmSource.clip == menuMusic) return;

        bgmSource.clip = menuMusic;
        bgmSource.loop = true;

        string currentScene = SceneManager.GetActiveScene().name;
        bgmSource.volume = currentScene.Contains("16") ? gameplayVolume : menuVolume;

        bgmSource.Play();
    }

    public void SetBGMVolumeSmooth(float targetVolume)
    {
        if (bgmSource == null) return;

        if (volumeFadeCoroutine != null)
            StopCoroutine(volumeFadeCoroutine);

        volumeFadeCoroutine = StartCoroutine(ChangeVolumeRoutine(targetVolume));
    }

    private IEnumerator ChangeVolumeRoutine(float targetVolume)
    {
        if (bgmSource == null) yield break;

        float startVolume = bgmSource.volume;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            if (bgmSource != null)
            {
                bgmSource.volume = Mathf.Lerp(startVolume, targetVolume, timer / fadeDuration);
            }
            yield return null;
        }

        if (bgmSource != null) bgmSource.volume = targetVolume;
    }

    // ==========================================
    // TÍNH NĂNG TẠO ĐIỂM NHẤN CHO ÂM THANH GOAL
    // ==========================================
    public void PlayGoalSFXWithDucking(AudioClip goalClip)
    {
        if (goalClip == null) return;

        if (goalDuckingCoroutine != null)
            StopCoroutine(goalDuckingCoroutine);

        goalDuckingCoroutine = StartCoroutine(GoalAudioRoutine(goalClip));
    }

    private IEnumerator GoalAudioRoutine(AudioClip goalClip)
    {
        // 1. Tắt/dập nhạc nền BGM xuống 0%
        if (bgmSource != null)
        {
            bgmSource.volume = goalBgmVolume;
        }

        // 2. Phát tiếng GOAL qua SFX Source với âm lượng 100% tối đa
        if (sfxSource != null)
        {
            sfxSource.PlayOneShot(goalClip, 1.0f);
        }

        // 3. Chờ âm thanh phát gần xong
        float clipDuration = Mathf.Max(goalClip.length, 1.5f);
        yield return new WaitForSeconds(clipDuration);

        // 4. Trả BGM từ từ trở lại mức Gameplay (35%)
        SetBGMVolumeSmooth(gameplayVolume);
    }

    // ==========================================
    // GIẢM ÂM LƯỢNG VỀ 0 VÀ TẮT NHẠC (CHO SPLASH SCREEN)
    // ==========================================
    public IEnumerator FadeOutMusic()
    {
        if (bgmSource == null)
            yield break;

        float startVolume = bgmSource.volume;

        while (bgmSource.volume > 0f)
        {
            bgmSource.volume -= Time.deltaTime / fadeDuration;
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.volume = startVolume;
    }
}