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

    string[] _months = { "January", "February", "March", "April", 
                         "May", "June", "July", "August", 
                         "September", "October", "November", "December" };

    int _mCounter;
    int _yCounter;
    int _dCounter;

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
    public Season CurrentSeason => _isWet ? Season.Wet : Season.Dry;

    Coroutine _timeRoutine;

    public event Action NewPolicy;

    void Awake()
    {
        ServiceHub.Instance._time = this;
    }

    void Start()
    {
        _sH = ServiceHub.Instance;
        
        InitCounters();

        UpdateMonth();
        UpdateYear();
        UpdateSeason();

        StartTime();
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

        if (_mCounter % 4 == 0)
            _dCounter++;

        if (_mCounter % 6 == 0)
            NewPolicy?.Invoke();

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
