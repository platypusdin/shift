using System.Collections;
using UnityEngine;

public class StoryController : MonoBehaviour
{
    [System.Serializable]
    private class DialogueFile
    {
        public string id;
        public TextAsset dialogue;
        public Sprite background;
    }

    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private FadeController fadeController;
    [SerializeField] private BackgroundController backgroundController;
    [SerializeField] private ChapterScreenController chapterScreenController;

    [SerializeField] private DialogueFile[] dialogues;
    public string CurrentBackgroundId { get; private set; }

    private void Start()
    {
        dialogueController.OnChoiceSelected += OnChoiceSelected;
        dialogueController.OnDialogueTransition += OnDialogueTransition;
        dialogueController.OnChapterFinished += OnChapterFinished;

        StartGame();
    }
    
    public void StartGame()
    {
        if (SaveManager.Instance != null &&
            SaveManager.Instance.IsLoadingGame)
        {
            string dialogueId =
                SaveManager.Instance.PendingDialogueId;

            string nodeId =
                SaveManager.Instance.PendingDialogueNodeId;

            SaveManager.Instance.ApplyStoryState();

            LoadSavedGame(dialogueId, nodeId);

            SaveManager.Instance.ClearPendingLoad();

            return;
        }

        StartNewGame();
    }

    public void StartNewGame()
    {
        StartCoroutine(StartChapter());
    }

    private IEnumerator StartChapter()
    {
        yield return chapterScreenController.ShowChapterStart("Глава 1");

        LoadDialogue("start");

        yield return fadeController.FadeIn();
    }

    public void LoadSavedGame(string dialogueId, string nodeId)
    {
        dialogueController.enabled = true;
        LoadDialogue(dialogueId, nodeId);
        SaveManager.Instance.ClearPendingLoad();
        StartCoroutine(FadeInAfterLoad());
    }

    private IEnumerator FadeInAfterLoad()
    {
        yield return fadeController.FadeIn();
    }

    private void OnDestroy()
    {
        if (dialogueController != null)
        {
            dialogueController.OnChoiceSelected -= OnChoiceSelected;
            dialogueController.OnDialogueTransition -= OnDialogueTransition;
            dialogueController.OnChapterFinished -= OnChapterFinished;
        }
    }

    public void LoadDialogue(string dialogueId, string nodeId = null)
    {
        DialogueFile dialogue = FindDialogue(dialogueId);

        if (dialogue == null)
        {
            Debug.LogError($"Dialogue '{dialogueId}' not found.");
            return;
        }

        CurrentBackgroundId = dialogue.id;

        if (dialogue.background != null)
            backgroundController.SetBackground(dialogue.background);

        dialogueController.LoadDialogue(dialogue.dialogue, dialogue.id, nodeId);
    }

    private void OnDialogueTransition(string dialogueId, string nodeId)
    {
        StartCoroutine(ChangeDialogue(dialogueId, nodeId));
    }

    private void OnChoiceSelected(StoryChoice choice)
    {
        if (choice == null ||
            string.IsNullOrEmpty(choice.nextDialogue))
        {
            return;
        }

        StartCoroutine(
            ChangeDialogue(
                choice.nextDialogue,
                choice.nextDialogueNode
            )
        );
    }

    private IEnumerator ChangeDialogue(
        string dialogueId,
        string nodeId)
    {
        yield return fadeController.FadeOut();

        dialogueController.EndDialogue();

        LoadDialogue(dialogueId, nodeId);

        yield return fadeController.FadeIn();
    }

    private DialogueFile FindDialogue(string id)
    {
        foreach (DialogueFile dialogue in dialogues)
        {
            if (dialogue.id == id)
                return dialogue;
        }

        return null;
    }

    public Sprite GetBackground(string dialogueId)
    {
        DialogueFile dialogue = FindDialogue(dialogueId);

        if (dialogue == null)
            return null;

        return dialogue.background;
    }

    private void OnChapterFinished()
    {
        StartCoroutine(EndChapter());
    }

    private IEnumerator EndChapter()
    {
        yield return chapterScreenController.ShowChapterEnd("Конец главы 1");
    }

}