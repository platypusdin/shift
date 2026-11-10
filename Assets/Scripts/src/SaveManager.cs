using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public bool HasPendingLoad => pendingLoadState != null;

    [SerializeField] private string gameplaySceneName = "MainScene";
    [SerializeField] private BackgroundDatabase backgroundDatabase;

    private GameState pendingLoadState;
    
    public bool IsLoadingGame => pendingLoadState != null;
    public string PendingDialogueId =>
        pendingLoadState?.dialogueId;

    public string PendingDialogueNodeId =>
        pendingLoadState?.dialogueNodeId;


    private string GetSavePath(int slot)
    {
        return Path.Combine(
            Application.persistentDataPath,
            $"save_{slot}.json"
        );
    }

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


    public void SaveGame(int slot)
    {
        DialogueController dialogueController =
            FindFirstObjectByType<DialogueController>();

        StoryController storyController =
            FindFirstObjectByType<StoryController>();

        StoryStateManager storyStateManager =
            StoryStateManager.Instance;

        if (dialogueController == null ||
            storyController == null ||
            storyStateManager == null)
        {
            Debug.LogError("Required game controllers not found.");
            return;
        }

        GameState state = new GameState
        {
            dialogueId = dialogueController.CurrentDialogueId,
            dialogueNodeId = dialogueController.CurrentNodeId,
            backgroundId = storyController.CurrentBackgroundId,
            saveDate = DateTime.Now.ToString("dd.MM.yyyy HH:mm"),
            storyStates = new List<StoryStateEntry>()
        };

        foreach (var entry in storyStateManager.GetFlags())
        {
            state.storyStates.Add(new StoryStateEntry
            {
                key = entry.Key,
                value = entry.Value
            });
        }

        string json = JsonUtility.ToJson(state, true);
        File.WriteAllText(GetSavePath(slot), json);

        Debug.Log($"Game saved to slot {slot}.");
    }

    public void LoadGame(int slot)
    {
        if (!HasSave(slot))
            return;

        string json = File.ReadAllText(GetSavePath(slot));
        pendingLoadState = JsonUtility.FromJson<GameState>(json);

        if (pendingLoadState == null)
        {
            Debug.LogError($"Failed to load save slot {slot}.");
            return;
        }

        StartCoroutine(LoadGameRoutine());
    }

    private IEnumerator LoadGameRoutine()
    {
        FadeController fadeController =
            FindFirstObjectByType<FadeController>();

        if (fadeController != null)
            yield return fadeController.FadeOut();

        Time.timeScale = 1f;

        if (SceneManager.GetActiveScene().name != gameplaySceneName)
        {
            SceneManager.LoadScene(gameplaySceneName);
        }
        else
        {
            ApplyLoadedState();
        }
    }

    public void ApplyStoryState()
    {
        if (pendingLoadState == null)
            return;

        StoryStateManager.Instance.SetFlags(
            pendingLoadState.storyStates
        );
    }

    private void ApplyLoadedState()
    {
        StoryController storyController =
            FindFirstObjectByType<StoryController>();

        if (storyController == null)
        {
            Debug.LogError("StoryController not found.");
            return;
        }

        StoryStateManager.Instance.SetFlags(
            pendingLoadState.storyStates
        );

        storyController.LoadSavedGame(
            pendingLoadState.dialogueId,
            pendingLoadState.dialogueNodeId
        );

        pendingLoadState = null;

        Debug.Log("Game loaded.");
    }

    public bool HasSave(int slot)
    {
        return File.Exists(GetSavePath(slot));
    }

    public GameState GetSaveInfo(int slot)
    {
        if (!HasSave(slot))
            return null;

        string json = File.ReadAllText(GetSavePath(slot));

        return JsonUtility.FromJson<GameState>(json);
    }

    public Sprite GetBackground(string backgroundId)
    {
        return backgroundDatabase.GetBackground(backgroundId);
    }

    public void DeleteSave(int slot)
    {
        Debug.Log($"Save path: {Application.persistentDataPath}");
        if (HasSave(slot))
            File.Delete(GetSavePath(slot));
    }

    public int GetLatestSaveSlot(int slotCount)
    {
        int latestSlot = -1;
        DateTime latestDate = DateTime.MinValue;

        for (int slot = 0; slot < slotCount; slot++)
        {
            if (!HasSave(slot))
                continue;

            GameState state = GetSaveInfo(slot);

            if (DateTime.TryParseExact(
                state.saveDate,
                "dd.MM.yyyy HH:mm",
                null,
                System.Globalization.DateTimeStyles.None,
                out DateTime saveDate))
            {
                if (saveDate > latestDate)
                {
                    latestDate = saveDate;
                    latestSlot = slot;
                }
            }
        }

        return latestSlot;
    }

    public void ClearPendingLoad()
    {
        pendingLoadState = null;
    }

    public void ClearAllSaves()
    {
        string[] files = Directory.GetFiles(
            Application.persistentDataPath,
            "save_*.json"
        );

        foreach (string file in files)
        {
            File.Delete(file);
        }

        Debug.Log($"Deleted {files.Length} save files.");
    }
}
