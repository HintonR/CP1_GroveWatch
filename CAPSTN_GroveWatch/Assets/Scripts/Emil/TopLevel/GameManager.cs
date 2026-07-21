using EasyTransition;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum IncidentType
{
    Fire,
    Drought,
    Flood,
    Camping,
    Logging,
    Construction
}

public enum CutsceneReason
{
    Reputation,
    Debt,
    Victory
}

public class GameManager : Singleton<GameManager>
{
    ServiceHub _sH;

    public float _reputation;
    public float _maxReputation;
    public float _progress;
    public float _maxProgress;

    public int _money;

    public bool _isPaused;
    public bool _inScreen;

    public bool _isEndless;

    public int[] _incidents = new int[6];


    [SerializeField] private int debtThreshold = -50000;

    public static CutsceneReason LastGameOverReason = CutsceneReason.Reputation;
    public bool _gameOverTriggered = false;
    public bool _victoryTriggered = false;

    void Awake()
    {
        ServiceHub.Instance._gM = this;
        _sH = ServiceHub.Instance;
    }

    void Start()
    {
        InitValues();
    }

    void InitValues()
    {
        _maxProgress = 50; //to be removed
        _maxReputation = 300;
        _reputation = _maxReputation * 0.8f;
        _progress = 0;

        _money = 5000;

        _gameOverTriggered = false;
        _victoryTriggered = false; 
    }

    void UpdateProgress()
    { 
        var progBar = _sH._UI.ProgBar;
        var progCo = _sH._UI.ProgFill;
        _sH._UI.UpdateBar(_progress, _maxProgress, progBar, progCo);
        _sH._UI.UpdateProgPercent();
        CheckVictory();
    }

    public void CheckVictory()
    {
        if (_gameOverTriggered) return;
        if (_isEndless) return;

        if (_progress >= _maxProgress)
            TriggerVictory();
    }

    private void TriggerVictory()
    {
        if (_victoryTriggered || _gameOverTriggered) return;
        _victoryTriggered = true;
        LastGameOverReason = CutsceneReason.Victory;

        var _tM = TransitionManager.Instance();
        var transitionSetting = Resources.Load<TransitionSettings>("Transitions/Brush/Brush");
        _tM.Transition("CutsceneScene", transitionSetting, 0.2f);
    }

    void UpdateReputation()
    {   
        var repBar = _sH._UI.RepBar;
        var repCo = _sH._UI.RepFill;
        _sH._UI.UpdateBar(_reputation, _maxReputation, repBar, repCo);
        _sH._UI.UpdateRepPercent();

        if (_reputation <= 0 && !_gameOverTriggered) //slight mods here
        {
            LastGameOverReason = CutsceneReason.Reputation;
            TriggerGameOver();
        }
    }

    public void SetMaxProgress(float value)
    {
        _maxProgress = value;
    }

    public void ChangeProgress(float value)
    {
        _progress += value;
        _progress = Mathf.Min(_progress, _maxProgress);
        UpdateProgress();
    }

    public void ChangeReputation(float value)
    {
        _reputation += value;
        _reputation = Mathf.Min(_reputation, _maxReputation);
        _reputation = Math.Max(0, _reputation);
        UpdateReputation();
    }

    private void TriggerGameOver()
    {
        if (_gameOverTriggered) return;
        var _tM = TransitionManager.Instance();
        if (_tM.isBusy) return;
        
        _gameOverTriggered = true;
        var transitionSetting = Resources.Load<TransitionSettings>("Transitions/Brush/Brush"); //hacky, the entire transitions folder got copied to Resources
        _tM.Transition("CutsceneScene", transitionSetting, 0.2f);
    }

    public void ChangeMoney(int value)
    {
        _sH._UI.SpawnFloatingMoney(value);
        
        if (value > 0)
            _sH._aM.PlaySFX(SFX.Money);

        _money += value;
        _sH._UI.UpdateMoney();

        CheckDebt();
    }

    public void CheckDebt()
    {
        if (_gameOverTriggered) return;
        if (_money <= debtThreshold)
        {
            LastGameOverReason = CutsceneReason.Debt;
            TriggerGameOver();
        }
    }

    public void IncreaseIncident(IncidentType i)
    {
        _incidents[(int)i]++;
    }


    //Method For Game Over
    public IncidentType GetDominantIncident()
    {
        int maxIndex = 0;
        int maxValue = _incidents[0];

        for (int i = 1; i < _incidents.Length; i++)
        {
            if (_incidents[i] > maxValue)
            {
                maxValue = _incidents[i];
                maxIndex = i;
            }
        }

        return (IncidentType)maxIndex;
    }

    public void ResetIncidents()
    {
        Array.Clear(_incidents, 0, _incidents.Length);
    }

    public void OpenSettings()
    {
        SceneManager.LoadScene("Settings", LoadSceneMode.Additive);
    }

}
