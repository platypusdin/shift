using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour
{

    [Header("Menu")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Buttons")]
    [SerializeField] private Button continueButton;

    [Header("Scenes")]
    [SerializeField] private string gameSceneName = "MainScene";

    [SerializeField] private LoaderMenuController loaderMenuController;
    [SerializeField] private AudioClip ambienceMusic;


    private void Start()
    {
        ShowMainMenu();
        int latestSlot =
        SaveManager.Instance.GetLatestSaveSlot(
            loaderMenuController.SlotCount
        );

        continueButton.gameObject.SetActive(latestSlot >= 0);
        AudioManager.Instance.PlayMusicWithFade(ambienceMusic);
    }

    public void StartGame()
    {
        StartCoroutine(StartGameCoroutine());
    }

    private IEnumerator StartGameCoroutine()
    {
        FadeController fadeController =
            FindFirstObjectByType<FadeController>();

        if (fadeController != null)
            yield return fadeController.FadeOut();

        SceneManager.LoadScene(gameSceneName);
    }

    public void Continue()
    {
        int latestSlot =
            SaveManager.Instance.GetLatestSaveSlot(
                loaderMenuController.SlotCount
            );

        if (latestSlot >= 0)
            SaveManager.Instance.LoadGame(latestSlot);
    }

    public void OpenLoad()
    {
        loaderMenuController.OpenLoadMenu();
    }

    public void CloseSettings()
    {
        ShowMainMenu();
    }

        public void ExitGame()
    {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
            Application.Quit();
    #endif
    }

    private void ShowMainMenu()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }
}
