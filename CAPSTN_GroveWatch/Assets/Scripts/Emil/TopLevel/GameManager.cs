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

    public int[] _incidents = new int[6];
    public event Action GameOver;

    void Awake()
    {
        ServiceHub.Instance._gM = this;
        _sH = ServiceHub.Instance;
    }

    void Start()
    {
        InitValues();

        UpdateProgress();
        UpdateReputation();

        _sH._UI.UpdateMoney();
    }

    void InitValues()
    {
        _maxReputation = 300;
        _reputation = _maxReputation * 0.9f;
        _maxProgress = 30;
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

    //Method For Main Menu
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

        if (_reputation <= 0)
            GameOver?.Invoke();
    }

    public void ChangeMoney(int value)
    {
        _money += value;
        _sH._UI.UpdateMoney();
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
