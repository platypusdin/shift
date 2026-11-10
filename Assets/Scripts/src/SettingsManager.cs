using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    private const float DefaultTextSpeed = 5f;
    private const float DefaultMusicVolume = 1f;

    private const int DefaultScreenWidth = 1920;
    private const int DefaultScreenHeight = 1080;
    

    public float TextSpeed { get; private set; }
    public float MusicVolume { get; private set; }
    public int ScreenWidth { get; private set; }
    public int ScreenHeight { get; private set; }

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

    private void Start()
    {
        LoadSettings();
    }

    public void SetTextSpeed(float value)
    {
        TextSpeed = value;
    }

    public void SetMusicVolume(float value)
    {
        MusicVolume = value;
        AudioManager.Instance.SetMusicVolume(value);
    }

    public void SetScreenResolution(int width, int height)
    {
        ScreenWidth = width;
        ScreenHeight = height;

        Screen.SetResolution(width, height, FullScreenMode.FullScreenWindow);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("TextSpeed", TextSpeed);
        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        PlayerPrefs.SetInt("ScreenWidth", ScreenWidth);
        PlayerPrefs.SetInt("ScreenHeight", ScreenHeight);
        PlayerPrefs.Save();
    }

    public void ResetSettings()
    {
        TextSpeed = DefaultTextSpeed;
        MusicVolume = DefaultMusicVolume;
        ScreenWidth = DefaultScreenWidth;
        ScreenHeight = DefaultScreenHeight;

        AudioManager.Instance.SetMusicVolume(MusicVolume);
        Screen.SetResolution(ScreenWidth, ScreenHeight, FullScreenMode.FullScreenWindow);

        SaveSettings();
    }


    private void LoadSettings()
    {
        TextSpeed = PlayerPrefs.GetFloat(
            "TextSpeed",
            DefaultTextSpeed
        );

        MusicVolume = PlayerPrefs.GetFloat(
            "MusicVolume",
            DefaultMusicVolume
        );

        ScreenWidth = PlayerPrefs.GetInt(
            "ScreenWidth",
            DefaultScreenWidth
        );

        ScreenHeight = PlayerPrefs.GetInt(
            "ScreenHeight",
            DefaultScreenHeight
        );

        AudioManager.Instance.SetMusicVolume(MusicVolume);
    }
}
