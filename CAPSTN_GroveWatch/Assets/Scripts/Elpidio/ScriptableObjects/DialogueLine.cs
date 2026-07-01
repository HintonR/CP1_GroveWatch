using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public Sprite image;
    public string title;
    [TextArea(3, 10)] public string text;

    [Header("Audio (optional)")]
    public bool changeMusic = false;
    public Music music;
}