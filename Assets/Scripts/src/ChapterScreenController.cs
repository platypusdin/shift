using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ChapterScreenController : MonoBehaviour
{
    [SerializeField] private FadeController fadeController;
    [SerializeField] private TMP_Text chapterText;

    [SerializeField] private float textDisplayDuration = 2f;
    [SerializeField] private string menuSceneName = "Bootstrap";

    public IEnumerator ShowChapterStart(string title)
    {

        chapterText.text = title;
        chapterText.gameObject.SetActive(true);


        Color color = chapterText.color;
        color.a = 0f;
        chapterText.color = color;

        yield return FadeText(0f, 1f, 0.5f);

        yield return new WaitForSecondsRealtime(textDisplayDuration);

        yield return FadeText(1f, 0f, 0.5f);

        chapterText.gameObject.SetActive(false);
    }

    private IEnumerator FadeText(
        float startAlpha,
        float endAlpha,
        float duration)
    {
        Color color = chapterText.color;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;

            color.a = Mathf.Lerp(
                startAlpha,
                endAlpha,
                time / duration
            );

            chapterText.color = color;

            yield return null;
        }

        color.a = endAlpha;
        chapterText.color = color;
    }

    // public IEnumerator ShowChapterEnd(string title)
    // {
    //     Debug.Log("story is end");
    //     chapterText.text = title;
    //     chapterText.gameObject.SetActive(true);

    //     yield return fadeController.FadeOut();

    //     yield return new WaitForSecondsRealtime(textDisplayDuration);

    //     SceneManager.LoadScene(menuSceneName);
    // }

    public IEnumerator ShowChapterEnd(string title)
    {
        chapterText.text = title;
        chapterText.gameObject.SetActive(true);

        Color color = chapterText.color;
        color.a = 0f;
        chapterText.color = color;

        yield return fadeController.FadeOut();

        yield return FadeText(0f, 1f, 0.5f);

        yield return new WaitForSecondsRealtime(textDisplayDuration);

        yield return FadeText(1f, 0f, 0.5f);

        chapterText.gameObject.SetActive(false);

        SceneManager.LoadScene(menuSceneName);
    }
}
