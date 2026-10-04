using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NewsManager : MonoBehaviour
{
    ServiceHub _sH;

    public string _policy, _decision, _season;
    public int _year, _dispatches, 
        _allowance, _deployment, _incentive,
        _f1, _f2, _f3;

    public int[] _events = new int[6];


    void Awake()
    {
        _sH = ServiceHub.Instance;
        _sH._nM = this;
        _policy = String.Empty;
        _decision = String.Empty;
        _dispatches = 0;
    }

    //_policy, _decision, _year, _dispatches, _deployment, are externally updated
    public void UpdateValues()
    {
        _season = _sH._time.IsWet ? "Wet" : "Dry";

        _allowance = _sH._time.GetQuarterlyBudget;
        _incentive = _sH._gMods._tourBudget;

        for (int i = 0; i < _events.Length; i++)
            _events[i] = _sH._gM._incidents[i];
        
        _f1 = _sH._fM.Tourist;
        _f2 = _sH._fM.Citizen;
        _f3 = _sH._fM.Govt;
    }

    public void UpdateDispatchCount()
    {
        _dispatches++;
    }
}
