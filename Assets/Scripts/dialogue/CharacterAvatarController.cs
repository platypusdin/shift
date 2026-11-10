using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CharacterAvatar
{
    public string id;
    public Sprite sprite;
}

public class CharacterAvatarController : MonoBehaviour
{

    [SerializeField] private Image character;
    [SerializeField] private CharacterAvatar[] avatars;
    
    public void Show(string avatarId)
    {
        if (string.IsNullOrEmpty(avatarId))
        {
            Hide();
            return;
        }

        CharacterAvatar avatar =
            avatars.FirstOrDefault(x => x.id == avatarId);

        if (avatar == null)
        {
            Debug.LogWarning($"Avatar not found: {avatarId}");
            Hide();
            return;
        }

        character.sprite = avatar.sprite;
        character.gameObject.SetActive(true);
    }

    public void Hide()
    {
        character.gameObject.SetActive(false);
    }
}
