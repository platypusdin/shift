using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [System.Serializable]
    public class SoundEffect
    {
        public string id;
        public AudioClip clip;
    }

    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private SoundEffect[] sounds;

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

    public void Play(string id)
    {
        foreach (SoundEffect sound in sounds)
        {
            if (sound.id == id)
            {
                audioSource.PlayOneShot(sound.clip);
                return;
            }
        }

        Debug.LogWarning($"Sound '{id}' not found.");
    }
}
