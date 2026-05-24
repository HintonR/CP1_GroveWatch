using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncomeManager : MonoBehaviour
{
    int _ffUpgrades;
    int _frUpgrades;
    int _pfUpgrades;

    void Awake()
    {
        ServiceHub.Instance._iM = this;
    }

    public void IncreaseUpgrade(UnitType unitType)
    {
        switch (unitType)
        {
            case UnitType.Firefighter:
                _ffUpgrades++; break;
            case UnitType.Ranger:
                _frUpgrades++; break;
            case UnitType.Police:
                _pfUpgrades++; break;
        }

        _ffUpgrades = Mathf.Min(_ffUpgrades, 8);
        _frUpgrades = Mathf.Min(_frUpgrades, 8);
        _pfUpgrades = Mathf.Min(_pfUpgrades, 8);
    }

    public int GetIncomeForUnit(UnitData unit)
    {
        int upgradeCount = GetUpgradeCount(unit.Type);

        if (upgradeCount >= 8)
            return unit.T3;

        if (upgradeCount >= 4)
            return unit.T2;

        return unit.T1;
    }

    int GetUpgradeCount(UnitType unitType)
    {
        switch (unitType)
        {
            case UnitType.Firefighter:
                return _ffUpgrades;

            case UnitType.Ranger:
                return _frUpgrades;

            case UnitType.Police:
                return _pfUpgrades;

            default:
                return 0;
        }
    }
}
