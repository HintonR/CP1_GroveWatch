using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyTransition;

public class StageSelectVictory : MonoBehaviour
{
    [SerializeField] private CutsceneData victoryCutscene;
    // Start is called before the first frame update
    void Start() //attached in Overworld scene, just sees if all 3 stages have been won and give an ending if it is
    {
        CheckVictory();
        
    }
    void CheckVictory()
    {
        var gM = ServiceHub.Instance._gM;
        if (gM._CebuWin && gM._BagiuoWin && gM._PalawanWin)
            CutsceneSceneDirector.PlayInCutsceneScene(victoryCutscene, Resources.Load<TransitionSettings>("Transitions/Brush/Brush"));
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void DebugCompleteAll()
    {
        var gM = ServiceHub.Instance._gM;
        gM._CebuWin = gM._BagiuoWin = gM._PalawanWin = true;
        CheckVictory();
    }
}
