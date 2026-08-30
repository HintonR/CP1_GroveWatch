using UnityEngine;

public enum MusicEndAction
{
    None,        //whatever the last line set keeps playing
    RestorePrevious,  //go back to whatever was playing before the tutorial started, used for Main scene gameplay
    PlaySpecific      //play endMusic
}

[CreateAssetMenu(fileName = "Cutscene_New", menuName = "Cutscenes/Cutscene Data")]
public class CutsceneData : ScriptableObject
{
    public string cutsceneName;
    public DialogueLine[] lines;

    [Header("Audio On End")]
    public MusicEndAction musicOnEnd = MusicEndAction.None;

    [Header("PlaySpecific only")]
    public Music endMusic;

    [Header("Completion (For tutorials this does nothing)")]
    public string nextSceneName;
}