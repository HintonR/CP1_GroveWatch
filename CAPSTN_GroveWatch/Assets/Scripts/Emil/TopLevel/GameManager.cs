using EasyTransition;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum IncidentType
{
    Fire,
    Drought,
    Flood,
    Camping,
    Logging,
    Construction
}

public enum GameOverReason
{
    Reputation,
    Debt
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
    public event Action GameOver;

    [SerializeField] private int debtThreshold = -50000;

    public static GameOverReason LastGameOverReason = GameOverReason.Reputation;
    private bool _gameOverTriggered = false;

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
        _reputation = _maxReputation * 0.9f;
        _progress = 0;

        _money = 1000;
    }

    void UpdateProgress()
    { 
        var progBar = _sH._UI.ProgBar;
        var progCo = _sH._UI.ProgFill;
        _sH._UI.UpdateBar(_progress, _maxProgress, progBar, progCo);
    }

    void UpdateReputation()
    {   
        var repBar = _sH._UI.RepBar;
        var repCo = _sH._UI.RepFill;
        _sH._UI.UpdateBar(_reputation, _maxReputation, repBar, repCo);
    }

    public void ResetProgress()
    {
        _progress = 0;
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
        UpdateReputation();

        //if (_reputation <= 0)
        //    GameOver?.Invoke();
        if (_reputation <= 0 && !_gameOverTriggered) //slight mods here
        {
            LastGameOverReason = GameOverReason.Reputation;
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        if (_gameOverTriggered) return;
        _gameOverTriggered = true;
        GameOver?.Invoke();
        //UnityEngine.SceneManagement.SceneManager.LoadScene("CutsceneScene"); //find out how to put in a transition without using inspector :(
        var _tM = TransitionManager.Instance();
        var transitionSetting = Resources.Load<TransitionSettings>("Transitions/Brush/Brush"); //hacky, the entire transitions folder got copied to Resources
        _tM.Transition("CutsceneScene", transitionSetting, 0.2f);
    }

    public void ChangeMoney(int value)
    {
        _money += value;
        _sH._UI.UpdateMoney();
        CheckDebt();
    }

    public void CheckDebt()
    {
        if (_gameOverTriggered) return;
        if (_money <= debtThreshold)
        {
            LastGameOverReason = GameOverReason.Debt;
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

}
