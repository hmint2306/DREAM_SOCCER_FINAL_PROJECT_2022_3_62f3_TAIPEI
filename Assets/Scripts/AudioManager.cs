using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("BGM")]
    public AudioSource bgmSource;
    public AudioClip menuMusic;
    public float fadeDuration = 0.8f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMenuMusic()
    {
        if (bgmSource == null || menuMusic == null)
            return;

        if (bgmSource.isPlaying && bgmSource.clip == menuMusic)
            return;

        bgmSource.clip = menuMusic;
        bgmSource.loop = true;
        bgmSource.volume = 1f;
        bgmSource.Play();
    }

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