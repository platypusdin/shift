using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource musicSource;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource.clip == clip)
            return;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        musicSource.Stop();

        musicSource.clip = clip;
        musicSource.volume = SettingsManager.Instance.MusicVolume;
        musicSource.loop = loop;

        musicSource.Play();
    }

    public void PlayMusicWithFade(
        AudioClip clip,
        float fadeDuration = 1f)
    {
        if (musicSource.clip == clip)
            return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(
            ChangeMusicRoutine(
                clip,
                fadeDuration
            )
        );
    }

    public void SetMusicVolume(float value)
    {
        musicSource.volume = value;
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void FadeOutMusic(float duration = 2f)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(
            FadeOutRoutine(duration)
        );
    }

    private IEnumerator FadeOutRoutine(float duration)
    {
        float startVolume = musicSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;

            musicSource.volume = Mathf.Lerp(
                startVolume,
                0f,
                time / duration
            );

            yield return null;
        }

        musicSource.Stop();

        musicSource.volume =
            SettingsManager.Instance.MusicVolume;

        fadeCoroutine = null;
    }

    private IEnumerator ChangeMusicRoutine(
        AudioClip newClip,
        float duration)
    {
        float startVolume = musicSource.volume;
        float time = 0f;

        // Fade out
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;

            musicSource.volume = Mathf.Lerp(
                startVolume,
                0f,
                time / duration
            );

            yield return null;
        }

        musicSource.Stop();

        // New music
        musicSource.clip = newClip;
        musicSource.loop = true;
        musicSource.Play();

        time = 0f;

        float targetVolume =
            SettingsManager.Instance.MusicVolume;

        // Fade in
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;

            musicSource.volume = Mathf.Lerp(
                0f,
                targetVolume,
                time / duration
            );

            yield return null;
        }

        musicSource.volume = targetVolume;

        fadeCoroutine = null;
    }
}
