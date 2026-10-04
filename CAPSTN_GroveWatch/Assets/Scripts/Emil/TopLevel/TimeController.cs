using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    const float TIME_SPEED = 20;
    
    ServiceHub _sH;

    [SerializeField] int _baseActiveEvents = 1;
    [SerializeField] int _difficultyScaler = 2;
    [SerializeField] UnitData _f, _r, _p;
    [SerializeField] int _quarterlyBudget;

    string[] _months = { "January", "February", "March", "April", 
                         "May", "June", "July", "August", 
                         "September", "October", "November", "December" };

    int _mCounter;
    int _yCounter;
    int _dCounter;

    public void SetQuarterlyBudget(int value) { _quarterlyBudget = value; }
    public int GetQuarterlyBudget => _quarterlyBudget;

    public bool _needPolicy = false;

    public int MaxActiveForestEvents
    {
        get
        {
            int baseCap = Mathf.Max(1, _baseActiveEvents);
            int difficultyStep = Mathf.Max(1, _difficultyScaler);
            int extraEvents = Mathf.Max(0, _dCounter - 1) / difficultyStep;

            return baseCap + extraEvents;
        }
    }

    bool _isWet;
    public bool IsWet => _isWet;
    public Season CurrentSeason => _isWet ? Season.Wet : Season.Dry;

    Coroutine _timeRoutine;

    void Awake()
    {
        _sH = ServiceHub.Instance;
        _sH._time = this;
    }

    void Start()
    {
        InitCounters();

        UpdateMonth();
        UpdateYear();
        UpdateSeason();

        StartTime();
    }

    void LateUpdate()
    {
        if (_needPolicy)
            _sH._UI.OpenPolicyScreen();
    }

    void InitCounters()
    {
        _mCounter = 1;
        _yCounter = 1;
        _dCounter = 1;
    }

    public void StartTime()
    {
        if (_timeRoutine != null)
            return;

        _timeRoutine = StartCoroutine(TimeRoutine());
    }

    IEnumerator TimeRoutine()
    {
        while (true)
        {
            while (_sH._gM._isPaused || _sH._gM._inScreen)            
                yield return null;

            yield return new WaitForSeconds(TIME_SPEED);

            if (_sH._gM._isPaused || _sH._gM._inScreen)
                continue;

            _mCounter++;
            UpdateMonth();
            UpdateSeason();
        }
    }

    void UpdateMonth()
    {        
        if (_mCounter % 3 == 0)
        {
            int finc = _sH._iM.GetIncomeForUnit(_f);
            int rinc = _sH._iM.GetIncomeForUnit(_r);
            int pinc = _sH._iM.GetIncomeForUnit(_p);
            
            finc *= _dCounter + 1;
            rinc *= _dCounter + 1;
            pinc *= _dCounter + 1;

            int deploymentIncentive = finc + rinc + pinc;

            _sH._nM._deployment = deploymentIncentive;

            _sH._gM.ChangeMoney(_quarterlyBudget + deploymentIncentive + _sH._gMods._tourBudget);
        }

        if (_mCounter % 4 == 0)
            _dCounter++;

        if (_mCounter % 6 == 0)
            _needPolicy = true;

        if (_mCounter > 12)
        {
            _mCounter = 1;
            _yCounter++;
            UpdateYear();
        }

        _sH._UI.UpdateMonth(_months[_mCounter - 1]);

    }

    void UpdateYear()
    {
        _sH._UI.UpdateYear(_yCounter);
        _sH._nM._year = _yCounter;
        _sH._nM._dispatches = 0;
    }

    void UpdateSeason()
    {
        _isWet = _mCounter >= 5 && _mCounter <= 10;
        _sH._UI.UpdateSeason(_isWet);
    }

    public bool IsStateAvailable(Season availableSeason)
    {
        return availableSeason == Season.Both || availableSeason == CurrentSeason;
    }

    public bool CanSpawnForestEvent()
    {
        return Forest.ActiveNegativeForestCount < MaxActiveForestEvents;
    }

}
