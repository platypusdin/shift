using System;
using UnityEngine;

[CreateAssetMenu(
    fileName = "BackgroundDatabase",
    menuName = "Game/Background Database"
)]
public class BackgroundDatabase : ScriptableObject
{
    [Serializable]
    private class BackgroundEntry
    {
        public string id;
        public Sprite sprite;
    }

    [SerializeField] private BackgroundEntry[] backgrounds;

    public Sprite GetBackground(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;

        foreach (BackgroundEntry background in backgrounds)
        {
            if (background.id == id)
                return background.sprite;
        }

        Debug.LogWarning($"Background '{id}' not found.");
        return null;
    }
}
