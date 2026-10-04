using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestEventManager : MonoBehaviour
{
    [SerializeField] GameObject _normal, _dead, _fire, _drought, _flood, _camp, _logging, _construction;

    GameObject[] _events;

    void Awake()
    {
        _events = new GameObject[] { _normal, _dead, _fire, _drought, _flood, _camp, _logging, _construction };
    }

    public void Set3DEvent(ForestState state)
    {
        foreach (GameObject ev in _events)
            ev.SetActive(false);

        switch(state)
        {
            case ForestState.Idle: 
                _normal.SetActive(true);
            break;
            case ForestState.Dead: 
                _dead.SetActive(true);
            break;
            case ForestState.Fire: 
                _fire.SetActive(true);
            break;
            case ForestState.Drought: 
                _drought.SetActive(true);
            break;
            case ForestState.Flooded: 
                _flood.SetActive(true);
            break;
            case ForestState.Camping: 
                _camp.SetActive(true);
            break;
            case ForestState.Construction: 
                _construction.SetActive(true);
            break;
            case ForestState.Logging: 
                _logging.SetActive(true);
            break;

        }
    }

}
