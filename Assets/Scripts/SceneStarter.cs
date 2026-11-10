using UnityEngine;

public class SceneStarter : MonoBehaviour
{
    [SerializeField] private TextAsset dialogueJson;
    
    
    [SerializeField] private AudioClip ambienceMusic;
    

    [SerializeField] private FadeController fadeController;

    private void Start()
    {
        StartCoroutine(fadeController.FadeIn());
        AudioManager.Instance.PlayMusicWithFade(ambienceMusic);
        // DialogueController.Instance.StartDialogue(dialogueJson);
    }
}
