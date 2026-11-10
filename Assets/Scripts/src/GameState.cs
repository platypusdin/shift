using UnityEngine;
using System;
using System.Collections.Generic;


[Serializable]
public class GameState
{
    public string dialogueId;
    public string dialogueNodeId;
    public string backgroundId;
    public string saveDate;

    public List<StoryStateEntry> storyStates = new();
}

[Serializable]
public class StoryStateEntry
{
    public string key;
    public bool value;
}
