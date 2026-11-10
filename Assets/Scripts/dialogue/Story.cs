using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class StoryData
{
    public string start;
    public List<StoryNode> nodes;

    public StoryNode GetNode(string id)
    {
        return nodes?.Find(node => node.id == id);
    }
}

[System.Serializable]
public class StoryNode
{
    public string id;

// nullable
    public string speaker;
// nullable
    public string avatar;
// nullable
    public string condition;
    public bool conditionValue;

    public string text;
    public string next;
    public StoryTransition[] transitions;

// nullable
    public string nextDialogue;
    public string nextDialogueNode;

// nullable
    public List<StoryChoice> choices;
// nullable
    public List<StoryAction> actions;
    public bool endChapter;
}

[System.Serializable]
public class StoryTransition
{
    public string next;
    public string nextDialogue;
    public string nextDialogueNode;
    public StoryCondition condition;
}

[System.Serializable]
public class StoryCondition
{
    public string key;
    public bool value;
}

[System.Serializable]
public class StoryChoice
{
    public string text;
    public string next;
    public string condition;
    public bool conditionValue;

    public string nextDialogue;
    public string nextDialogueNode;

    public List<StoryAction> actions;
    public StoryTransition[] transitions;
}

[System.Serializable]
public class StoryState
{
    private Dictionary<string, bool> flags = new();

    public bool GetFlag(string id)
    {
        return flags.TryGetValue(id, out bool value) && value;
    }

    public void SetFlag(string id, bool value)
    {
        flags[id] = value;
    }
}

[System.Serializable]
public class StoryAction
{
    public string type;
    public string key;
    public bool value;
}
