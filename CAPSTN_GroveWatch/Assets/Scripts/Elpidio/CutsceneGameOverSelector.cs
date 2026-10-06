using UnityEngine;

[DefaultExecutionOrder(-100)] //you're a wizard harry

public class GameOverCutsceneSelector : MonoBehaviour
{
    [Header("Cutscene per IncidentType (match with GameManager.cs)")]
    [SerializeField] private CutsceneData fireEnding;         // 0 - Fire
    [SerializeField] private CutsceneData droughtEnding;      // 1 - Drought
    [SerializeField] private CutsceneData floodEnding;        // 2 - Flood
    [SerializeField] private CutsceneData campingEnding;      // 3 - Camping
    [SerializeField] private CutsceneData loggingEnding;      // 4 - Logging
    [SerializeField] private CutsceneData constructionEnding; // 5 - Construction

    [Header("Debt / Corruption Ending")]
    [SerializeField] private CutsceneData debtEnding;

    [Header("Victory Ending")]
    [SerializeField] private CutsceneData victoryEnding;

    [Header("Fallback")]
    [SerializeField] private CutsceneData fallbackEnding;

    [System.Serializable]
    public class LevelCutscenes
    {
        public string level;        //scene name: Cebu, TutorialLevel, Baguio, ect
        public CutsceneData intro;
        public CutsceneData win; 
    }

    [Header("Per Level (scene name > intro / win cutscene)")]
    [SerializeField] private LevelCutscenes[] levels = new LevelCutscenes[0];

    ServiceHub _sH; //_sH.gM. to access GameManager

    void Awake()
    {
        if (CutsceneState.SelectedCutscene != null)
            return;

        if (CutsceneState.PendingLevel != null)   //play current level's intro
        {
            var entry = FindLevel(CutsceneState.PendingLevel);
            CutsceneState.SelectedCutscene = entry != null ? entry.intro : null;
            CutsceneState.PendingLevel = null;
            return;
        }

        _sH = ServiceHub.Instance;
        CutsceneData chosen = SelectEnding();
        CutsceneState.SelectedCutscene = chosen != null ? chosen : fallbackEnding;
    }
    LevelCutscenes FindLevel(string level)
    {
        foreach (var l in levels)
            if (l.level == level) return l;
        return null;
    }
    CutsceneData SelectEnding()
    {
        //victory
        if (GameManager.LastGameOverReason == CutsceneReason.Victory)
        {
            var entry = FindLevel(CutsceneState.LastLevel);
            return entry != null && entry.win != null ? entry.win : victoryEnding;
        }

        //debt
        if (GameManager.LastGameOverReason == CutsceneReason.Debt)
            return debtEnding;

        //incident
        return SelectFromIncident();
    }

    CutsceneData SelectFromIncident()
    {

        IncidentType dominant = _sH._gM.GetDominantIncident();

        switch (dominant)
        {
            case IncidentType.Fire: return fireEnding;
            case IncidentType.Drought: return droughtEnding;
            case IncidentType.Flood: return floodEnding;
            case IncidentType.Camping: return campingEnding;
            case IncidentType.Logging: return loggingEnding;
            case IncidentType.Construction: return constructionEnding;
            default: return null;
        }
    }
}