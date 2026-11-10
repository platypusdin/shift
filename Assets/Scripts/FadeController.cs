using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeController : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    public IEnumerator FadeIn()
    {
        yield return Fade(1f, 0f);
    }

    public IEnumerator FadeOut()
    {
        yield return Fade(0f, 1f);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        fadeImage.gameObject.SetActive(true);

        float time = 0f;
        Color color = fadeImage.color;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;

            color.a = Mathf.Lerp(startAlpha, endAlpha, time / fadeDuration);

            fadeImage.color = color;

            yield return null;
        }

        color.a = endAlpha;
        fadeImage.color = color;

        if (endAlpha == 0f)
            fadeImage.gameObject.SetActive(false);
    }
}
