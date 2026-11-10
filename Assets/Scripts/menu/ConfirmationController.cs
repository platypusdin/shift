using System;
using TMPro;
using UnityEngine;

public class ConfirmationController : MonoBehaviour
{
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private TMP_Text messageText;

    private Action onConfirmed;

    public void Show(string message, Action onConfirmed)
    {
        this.onConfirmed = onConfirmed;

        messageText.text = message;
        confirmationPanel.SetActive(true);
    }

    public void Confirm()
    {
        confirmationPanel.SetActive(false);

        onConfirmed?.Invoke();
        onConfirmed = null;
    }

    public void Cancel()
    {
        confirmationPanel.SetActive(false);
        onConfirmed = null;
    }
}
