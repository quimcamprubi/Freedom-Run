using System;
using UnityEngine;

[Serializable]
public class Dialogue
{
    public string name;

    [TextArea(3, 10)] public string[] sentenceList;
}