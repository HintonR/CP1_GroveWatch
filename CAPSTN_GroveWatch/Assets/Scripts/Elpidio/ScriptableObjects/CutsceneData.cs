using UnityEngine;

[CreateAssetMenu(fileName = "Cutscene_New", menuName = "Cutscenes/Cutscene Data")]
public class CutsceneData : ScriptableObject
{
    public string cutsceneName;
    public DialogueLine[] lines;

    [Header("Completion")]
    public CutsceneCompletionAction onComplete = CutsceneCompletionAction.LoadScene;
    public string nextSceneName;
    [Header("Playback Mode")]
    public CutscenePlayMode playMode = CutscenePlayMode.Cutscene;
}
public enum CutscenePlayMode
{
    Cutscene,  
    Tutorial    
}

public enum CutsceneCompletionAction
{
    LoadScene,      
    DoNothing       //debug, will remove
}