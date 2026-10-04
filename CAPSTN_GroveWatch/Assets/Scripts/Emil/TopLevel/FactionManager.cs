using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public enum Faction
{
    Tourist,
    Citizen,
    Government
}

public class FactionManager : MonoBehaviour
{
    const int FACTION_MAX = 20;
    const int FACTION_MIN = 5;
    ServiceHub _sH;

    [SerializeField] int _tourist, _citizen, _government;

    public int Tourist => _tourist;
    public int Citizen => _citizen;
    public int Govt => _government;

    void Awake()
    {
        _sH = ServiceHub.Instance;
        _sH._fM = this;
    }

    public void UpdateFactionStanding(Faction faction, int value)
    {
        switch(faction)
        {
            case Faction.Tourist:    
                _tourist += value;
                CheckStanding(Faction.Tourist, _tourist);                     
                break;
            case Faction.Citizen:   
                _citizen += value; 
                CheckStanding(Faction.Citizen, _citizen);
                break;
            case Faction.Government: 
                _government += value; 
                CheckStanding(Faction.Government, _government);
                break;
        }

        _tourist = Mathf.Clamp(_tourist, 0, 25);
        _citizen = Mathf.Clamp(_citizen, 0, 25);
        _government = Mathf.Clamp(_government, 0, 25);
    }

    void CheckStanding(Faction faction, int value)
    {
        if (value >= FACTION_MAX)
            _sH._gMods.SetFactionMods(faction, Standing.Good);
        if (value < FACTION_MAX && value > FACTION_MIN)
            _sH._gMods.SetFactionMods(faction, Standing.Neutral);
        if (value <= FACTION_MIN)
            _sH._gMods.SetFactionMods(faction, Standing.Bad);
    }
   
}
