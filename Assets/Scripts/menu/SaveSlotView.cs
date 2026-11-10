using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlotView : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TMP_Text dateText;

    private int slot;
    private LoaderMenuController controller;

    public void Initialize(
        int slot,
        LoaderMenuController controller)
    {
        this.slot = slot;
        this.controller = controller;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    public void SetEmpty()
    {
        backgroundImage.sprite = null;
        backgroundImage.color = new Color(0f, 0f, 0f, 0.4f);
        dateText.text = "Пусто";
    }

    public void SetSave(GameState state, Sprite background)
    {
     Debug.Log(background);
        backgroundImage.sprite = background;
        backgroundImage.color = Color.white;
        dateText.text = state.saveDate;
    }
    

    private void OnClick()
    {
        controller.OnSlotClicked(slot);
    }
}
