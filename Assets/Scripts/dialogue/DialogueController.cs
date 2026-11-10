using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private Canvas dialogueCanvas;
    [SerializeField] private GameObject panel;

    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private CharacterAvatarController avatarController;

    [Header("Choices")]
    [SerializeField] private Transform choicesRoot;
    [SerializeField] private Button choiceButtonPrefab;

    private StoryData currentStory;
    private StoryNode currentNode;

    private Coroutine typingRoutine;

    private bool typing;
    private bool active;

    public Action<StoryChoice> OnChoiceSelected;
    // public Action<string> OnDialogueTransition;
    public Action<string, string> OnDialogueTransition;
    public Action OnFinished;

    public Action OnChapterFinished;

    public bool IsActive => active;

    public string CurrentDialogueId { get; private set; }
    public string CurrentNodeId => currentNode?.id;
    

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        dialogueCanvas.enabled = false;
        active = false;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }


    private void Update()
    {
        if (!active)
            return;

        if (Mouse.current == null ||
            !Mouse.current.leftButton.wasPressedThisFrame)
            return;

        if (typing)
        {
            FinishTyping();
            return;
        }

        if (HasChoices())
            return;

        Next();
    }

    public void LoadDialogue(TextAsset jsonFile, string dialogueId, string nodeId = null)
    {
        if (jsonFile == null)
        {
            Debug.LogError("Dialogue JSON is not assigned.");
            return;
        }

        StoryData story =
            JsonUtility.FromJson<StoryData>(jsonFile.text);

        if (story == null || string.IsNullOrEmpty(story.start))
        {
            Debug.LogError($"Invalid dialogue JSON: {jsonFile.name}");
            return;
        }

        CurrentDialogueId = dialogueId;
        currentStory = story;
        active = true;

        dialogueCanvas.enabled = true;
        panel.SetActive(true);

        OpenNode(nodeId ?? currentStory.start);
    }

    private void OpenNode(string nodeId)
    {
        StoryNode node = currentStory.GetNode(nodeId);

        if (node == null)
        {
            Debug.LogError(
                $"Node '{nodeId}' not found in dialogue '{CurrentDialogueId}'."
            );

            EndDialogue();
            return;
        }

        currentNode = node;

        if (!string.IsNullOrEmpty(node.condition) &&
            !StoryStateManager.Instance.GetFlag(node.condition))
        {
            Debug.LogWarning(
                $"Condition '{node.condition}' is not satisfied for node '{node.id}'."
            );

            Next();
            return;
        }

        ExecuteActions(node);

        speakerText.text = node.speaker ?? string.Empty;

        avatarController.Show(node.avatar);

        ShowChoices(node);

        StartTyping(node.text);
    }

    private void StartTyping(string text)
    {
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        typingRoutine = StartCoroutine(TypeText(text));
    }


    private IEnumerator TypeText(string text)
    {
        typing = true;
        dialogueText.text = string.Empty;

        foreach (char c in text)
        {
            dialogueText.text += c;

            float normalizedSpeed = Mathf.InverseLerp(1f, 10f, SettingsManager.Instance.TextSpeed);

            float delay = Mathf.Lerp(0.08f, 0.01f, normalizedSpeed);

            yield return new WaitForSeconds(delay);
        }

        typing = false;
        typingRoutine = null;
    }

    private void FinishTyping()
    {
        if (typingRoutine != null)
        {
            StopCoroutine(typingRoutine);
            typingRoutine = null;
        }

        dialogueText.text = currentNode.text;

        typing = false;
    }

    private void ShowChoices(StoryNode node)
    {
        ClearChoices();

        if (node.choices == null || node.choices.Count == 0)
        {
            choicesRoot.gameObject.SetActive(false);
            return;
        }

        bool hasVisibleChoices = false;

        for (int i = 0; i < node.choices.Count; i++)
        {
            StoryChoice choice = node.choices[i];

            if (!string.IsNullOrEmpty(choice.condition) &&
                StoryStateManager.Instance.GetFlag(choice.condition) != choice.conditionValue)
            {
                continue;
            }

            hasVisibleChoices = true;

            Button button =
                Instantiate(choiceButtonPrefab, choicesRoot);

            button.GetComponentInChildren<TMP_Text>().text =
                choice.text;

            button.onClick.AddListener(
                () => Choose(choice)
            );
        }

        choicesRoot.gameObject.SetActive(hasVisibleChoices);
    }

    private void ClearChoices()
    {
        for (int i = choicesRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(choicesRoot.GetChild(i).gameObject);
        }
    }

    private bool HasChoices()
    {
        return currentNode != null &&
               currentNode.choices != null &&
               currentNode.choices.Count > 0;
    }

    private void Choose(StoryChoice choice)
    {
        if (choice == null)
            return;

        ExecuteActions(choice);

        choicesRoot.gameObject.SetActive(false);

        StoryTransition transition = GetNextTransition(
            choice.transitions
        );

        if (transition == null)
        {
            if (!string.IsNullOrEmpty(choice.nextDialogue))
            {
                transition = new StoryTransition
                {
                    nextDialogue = choice.nextDialogue,
                    nextDialogueNode = choice.nextDialogueNode
                };
            }
            else if (!string.IsNullOrEmpty(choice.next))
            {
                transition = new StoryTransition
                {
                    next = choice.next
                };
            }
        }

        ExecuteTransition(transition);
}

    private void Next()
    {
        if (currentNode == null)
            return;

        if (currentNode.endChapter)
        {
            OnChapterFinished?.Invoke();
            return;
        }

        StoryTransition transition = GetNextTransition(currentNode.transitions);

        if (transition == null)
        {
            if (!string.IsNullOrEmpty(currentNode.nextDialogue))
            {
                transition = new StoryTransition
                {
                    nextDialogue = currentNode.nextDialogue,
                    nextDialogueNode = currentNode.nextDialogueNode
                };
            }
            else if (!string.IsNullOrEmpty(currentNode.next))
            {
                transition = new StoryTransition
                {
                    next = currentNode.next
                };
            }
        }

        ExecuteTransition(transition);

    }

    private void ExecuteTransition(StoryTransition transition)
    {
        if (transition == null)
        {
            EndDialogue();
            return;
        }

        if (!string.IsNullOrEmpty(transition.nextDialogue))
        {
            OnDialogueTransition?.Invoke(
                transition.nextDialogue,
                transition.nextDialogueNode
            );

            return;
        }

        if (!string.IsNullOrEmpty(transition.next))
        {
            OpenNode(transition.next);
            return;
        }

        EndDialogue();
    }

    private StoryTransition GetNextTransition(StoryTransition[] transitions)
    {
        if (transitions == null || transitions.Length == 0)
            return null;

        foreach (StoryTransition transition in transitions)
        {
            if (transition.condition == null)
                return transition;

            if (StoryStateManager.Instance.GetFlag(
                    transition.condition.key)
                == transition.condition.value)
            {
                return transition;
            }
        }

        return null;
    }


    private void ExecuteActions(StoryNode node)
    {
        if (node.actions == null)
            return;

        foreach (StoryAction action in node.actions)
        {
            ExecuteAction(action);
        }
    }

    private void ExecuteActions(StoryChoice choice)
    {
        if (choice.actions == null)
            return;

        foreach (StoryAction action in choice.actions)
        {
            ExecuteAction(action);
        }
    }

    private void ExecuteAction(StoryAction action)
    {
        if (action == null)
            return;

        switch (action.type)
        {
            case "setFlag":
                StoryStateManager.Instance.SetFlag(
                    action.key,
                    action.value
                );
                break;
            case "playSound":
                SoundManager.Instance.Play(action.key);
                break;

            default:
                Debug.LogWarning(
                    $"Unknown story action: {action.type}"
                );
                break;
        }
    }

    public void EndDialogue()
    {
        if (typingRoutine != null)
        {
            StopCoroutine(typingRoutine);
            typingRoutine = null;
        }

        typing = false;
        active = false;

        currentStory = null;
        currentNode = null;

        dialogueCanvas.enabled = false;
        panel.SetActive(false);
        choicesRoot.gameObject.SetActive(false);

        OnFinished?.Invoke();
    }
}