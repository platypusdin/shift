using UnityEngine;

public class BackgroundController : MonoBehaviour
{

    [SerializeField] private SpriteRenderer backgroundRenderer;

    public void SetBackground(Sprite background)
    {
        backgroundRenderer.sprite = background;
    }
}
