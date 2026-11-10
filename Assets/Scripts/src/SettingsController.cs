using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private TMP_InputField textSpeedInput;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject previousMenu;

    private const float MinTextSpeed = 1f;
    private const float MaxTextSpeed = 10f;
    private readonly (int width, int height)[] resolutions =
    {
        (1280, 720),
        (1600, 900),
        (1920, 1080),
        (2560, 1440),
        (3840, 2160)
    };

    private void Start() 
    { 
        LoadSettingsToUI(); 
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Close();
        }
    }

    public void Apply()
    {
        if (float.TryParse(textSpeedInput.text, out float textSpeed))
        {
            textSpeed = Mathf.Clamp(textSpeed, MinTextSpeed, MaxTextSpeed);

            SettingsManager.Instance.SetTextSpeed(textSpeed);
        }
            
        SettingsManager.Instance.SetMusicVolume(musicVolumeSlider.value);
        var resolution = resolutions[resolutionDropdown.value];
        SettingsManager.Instance.SetScreenResolution(resolution.width, resolution.height);

        SettingsManager.Instance.SaveSettings();

        LoadSettingsToUI();
        Close();
    }

    public void Open()
    {
        if (previousMenu != null)
            previousMenu.SetActive(false);

        settingsPanel.SetActive(true);
        LoadSettingsToUI();
    }

    public void Reset()
    {
        SettingsManager.Instance.ResetSettings();
        Close();
    }

    private void Close()
    {
        settingsPanel.SetActive(false);

        if (previousMenu != null)
            previousMenu.SetActive(true);
    }

    private void LoadSettingsToUI()
    {
        if (SettingsManager.Instance == null)
        {
            Debug.LogError("SettingsController: SettingsManager.Instance is NULL");
            return;
        }
        
        textSpeedInput.text = SettingsManager.Instance.TextSpeed.ToString();
        musicVolumeSlider.value = SettingsManager.Instance.MusicVolume;
        LoadResolutionToUI();
    }

    private void LoadResolutionToUI()
    {
        resolutionDropdown.ClearOptions();

        List<string> options = new();

        int currentIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            var resolution = resolutions[i];

            options.Add(
                $"{resolution.width} x {resolution.height}"
            );

            if (resolution.width == SettingsManager.Instance.ScreenWidth &&
                resolution.height == SettingsManager.Instance.ScreenHeight)
            {
                currentIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentIndex;
        resolutionDropdown.RefreshShownValue();
    }

}
