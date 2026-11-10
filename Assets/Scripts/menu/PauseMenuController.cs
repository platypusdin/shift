using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private LoaderMenuController loaderMenuController;
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private string sceneName;

    private bool isOpen;

    private void Start()
    {
        CloseMenu();
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // Пока открыты настройки, меню паузы не реагирует на ESC
            if (settingsPanel.activeSelf)
                return;

            if (isOpen)
                CloseMenu();
            else
                OpenMenu();
        }
    }

    public void OpenMenu()
    {
        isOpen = true;

        menuCanvas.SetActive(true);

        if (DialogueController.Instance != null)
            DialogueController.Instance.enabled = false;

        Time.timeScale = 0f;
    }

    public void CloseMenu()
    {
        isOpen = false;

        menuCanvas.SetActive(false);

        if (DialogueController.Instance != null)
            DialogueController.Instance.enabled = true;

        Time.timeScale = 1f;
    }

    public void OpenSave()
    {
        menuCanvas.SetActive(false);
        loaderMenuController.OpenSaveMenu();
    }

    public void OpenLoad()
    {
        menuCanvas.SetActive(false);
        loaderMenuController.OpenLoadMenu();
    }

    public void Continue()
    {
        CloseMenu();
    }

    public void ExitToMenu()
    {
        StartCoroutine(ExitToMenuRoutine());
    }


    private IEnumerator ExitToMenuRoutine()
    {
        FadeController fadeController =
            FindFirstObjectByType<FadeController>();

        if (fadeController != null)
            yield return fadeController.FadeOut();

        Time.timeScale = 1f;

        SceneManager.LoadScene(sceneName);
    }
}
