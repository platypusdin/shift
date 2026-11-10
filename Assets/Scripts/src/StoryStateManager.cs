using System.Collections.Generic;
using UnityEngine;

public class StoryStateManager : MonoBehaviour
{
    public static StoryStateManager Instance { get; private set; }

    private readonly Dictionary<string, bool> flags = new();

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

    public bool GetFlag(string id)
    {
        return flags.TryGetValue(id, out bool value) && value;
    }

    public void SetFlag(string id, bool value)
    {
        flags[id] = value;
    }

    public Dictionary<string, bool> GetFlags()
    {
        return new Dictionary<string, bool>(flags);
    }

    public void SetFlags(List<StoryStateEntry> states)
    {
        flags.Clear();

        if (states == null)
            return;

        foreach (StoryStateEntry state in states)
        {
            flags[state.key] = state.value;
        }
    }

    public void Clear()
    {
        flags.Clear();
    }
}
