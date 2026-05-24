using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceHub : Singleton<ServiceHub>
{
    public GameManager _gM;
    public IncomeManager _iM;
    public QuadrantManager _qM;

    public UnitController _unit;
    public TimeController _time;
    public UIController _UI;

    void Awake()
    {
        _gM = GameManager.Instance;
    }
}
