using UnityEngine;
using UnityEngine.InputSystem;

public class LoaderMenuController : MonoBehaviour
{
    public enum Mode
    {
        Save,
        Load
    }

    [SerializeField] private ConfirmationController confirmationController;
    [SerializeField] private GameObject loaderCanvas;
    [SerializeField] private SaveSlotView slotPrefab;
    [SerializeField] private Transform slotsRoot;
    [SerializeField] private int slotCount = 6;

    private Mode currentMode;
    public int SlotCount => slotCount;

    public void OpenSaveMenu()
    {
        Open(Mode.Save);
    }

    public void OpenLoadMenu()
    {
        Open(Mode.Load);
    }

    private void Open(Mode mode)
    {
        currentMode = mode;

        CreateSlots();

        loaderCanvas.SetActive(true);
    }

    private void Update()
    {
        if (!loaderCanvas.activeSelf)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Close();
        }
    }

    public void Close()
    {
        loaderCanvas.SetActive(false);
    }

    private void CreateSlots()
    {
        ClearSlots();

        for (int i = 0; i < slotCount; i++)
        {
            SaveSlotView slot = Instantiate(slotPrefab, slotsRoot);

            slot.Initialize(i, this);

            if (SaveManager.Instance.HasSave(i))
            {
                GameState state = SaveManager.Instance.GetSaveInfo(i);

                Debug.Log(state.backgroundId);
                Sprite background =
                    SaveManager.Instance.GetBackground(state.backgroundId);

                slot.SetSave(state, background);
            }
            else
            {
                slot.SetEmpty();
            }
        }
    }

    public void OnSlotClicked(int slot)
    {
        if (currentMode == Mode.Save)
        {
            if (SaveManager.Instance.HasSave(slot))
            {
                confirmationController.Show(
                    "Перезаписать игру?",
                    () => SaveGame(slot)
                );

                return;
            }

            SaveGame(slot);
        }
        else
        {
            if (!SaveManager.Instance.HasSave(slot))
                return;

            Close();

            SaveManager.Instance.LoadGame(slot);
        }
    }

    private void SaveGame(int slot)
    {

        SaveManager.Instance.SaveGame(slot);

        loaderCanvas.SetActive(false);

        Time.timeScale = 1f;
    }

    private void ClearSlots()
    {
        foreach (Transform child in slotsRoot)
        {
            Destroy(child.gameObject);
        }
    }
}
