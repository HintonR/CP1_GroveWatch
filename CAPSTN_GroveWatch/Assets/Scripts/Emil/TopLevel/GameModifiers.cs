using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModifierType
{
    CDR,
    EFF
}

public class GameModifiers : MonoBehaviour
{
    ServiceHub _sH;

    public float _researchFCDR, _researchFEFF, _researchRCDR, _researchREFF, _researchPCDR, _researchPEFF;
    public float _policyCDR, _policyEFF;
    public float _health, _rep;
     
    void Awake()
    {
        _sH = ServiceHub.Instance;
        _sH._gMods = this;
    }

    void Start()
    {
        Init();
    }

    void Init()
    {
        _researchFCDR = 1;
        _researchFEFF = 1;
        _researchPCDR = 1;
        _researchPEFF = 1;
        _researchRCDR = 1;
        _researchREFF = 1;

        _policyCDR = 1;
        _policyEFF = 1;

        _health = 1;
        _rep = 1;
    }

    public void SetResearchMod (ModifierType mType, UnitType uType, float value)
    {
        switch (mType)
        {
            case ModifierType.CDR:
                SetResearchCDRMod(uType, value);
                break;

            case ModifierType.EFF:
                SetResearchDamageMod(uType, value);
                break;
        }
    }

    void SetResearchCDRMod (UnitType uType, float value)
    {
        switch (uType)
        {
            case UnitType.Firefighter:
                _researchFCDR = value;
                break;

            case UnitType.Ranger:
                _researchRCDR = value;
                break;

            case UnitType.Police:
                _researchPCDR = value;
                break;
        }
    }

    void SetResearchDamageMod (UnitType uType, float value)
    {
        switch (uType)
        {
            case UnitType.Firefighter:
                _researchFEFF = value;
                break;

            case UnitType.Ranger:
                _researchREFF = value;
                break;

            case UnitType.Police:
                _researchPEFF = value;
                break;
        }
    }

    public void SetPolicyMod (ModifierType mType, float value)
    {
        switch (mType)
        {
            case ModifierType.CDR: _policyCDR = value; break;
            case ModifierType.EFF: _policyEFF = value; break;
        }
    }

    public void SetForestHealthMod (float value)
    {
        _health = value;
    }

    public void SetReputationBonus (float value)
    {
        _rep = value;
    }
}
