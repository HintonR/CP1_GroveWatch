using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModifierType
{
    CDR,
    EFF
}

public enum Standing
{
    Good,
    Neutral,
    Bad
}

public class GameModifiers : MonoBehaviour
{
    const int NOT_MODIFIED = 1;
    ServiceHub _sH;

    public float _researchFCDR, _researchFEFF, _researchRCDR, _researchREFF, _researchPCDR, _researchPEFF;
    public float _policyCDR, _policyEFF;
    public float _health, _rep;
    
    public float _govCDR, _govEFF;
    public int _tourBudget; 
    public float _tourRepPenalty, _tourRepPassive;
    public float _citizenRepBonus, _citizenRepPenalty, _citizenRepPassive;
     
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
        _researchFCDR = NOT_MODIFIED;
        _researchFEFF = NOT_MODIFIED;
        _researchPCDR = NOT_MODIFIED;
        _researchPEFF = NOT_MODIFIED;
        _researchRCDR = NOT_MODIFIED;
        _researchREFF = NOT_MODIFIED;

        _policyCDR = NOT_MODIFIED;
        _policyEFF = NOT_MODIFIED;

        _health = NOT_MODIFIED;
        _rep = NOT_MODIFIED;

        _govCDR = NOT_MODIFIED;
        _govEFF = NOT_MODIFIED;
        
        _tourRepPenalty = NOT_MODIFIED;
        _tourRepPassive = NOT_MODIFIED;
        _citizenRepBonus = NOT_MODIFIED;
        _citizenRepPenalty = NOT_MODIFIED;
        _citizenRepPassive = NOT_MODIFIED;

        _tourBudget = 0;
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

    public void SetFactionMods (Faction faction, Standing standing)
    {
        switch (faction)
        {
            case Faction.Tourist:
                if (standing == Standing.Good)
                {
                    _tourBudget = 15000;
                    _tourRepPassive = NOT_MODIFIED;
                    _tourRepPenalty = NOT_MODIFIED;
                }
                else if (standing == Standing.Neutral)
                {   
                    _tourBudget = 0;
                    _tourRepPassive = NOT_MODIFIED;
                    _tourRepPenalty = NOT_MODIFIED;
                }
                else if (standing == Standing.Bad)
                {
                    _tourBudget = 0;
                    _tourRepPassive = 1.1f;
                    _tourRepPenalty = 1.25f;
                }
                break;
            case Faction.Citizen:
                if (standing == Standing.Good)
                {
                    _citizenRepBonus = 1.5f;
                    _citizenRepPassive = 0.9f;
                    _citizenRepPenalty = 0.9f;
                }
                else if (standing == Standing.Neutral)
                {
                    _citizenRepBonus = NOT_MODIFIED;
                    _citizenRepPassive = NOT_MODIFIED;
                    _citizenRepPenalty = NOT_MODIFIED;
                }
                else if (standing == Standing.Bad)
                {
                    _citizenRepBonus = NOT_MODIFIED;
                    _citizenRepPassive = 1.2f;
                    _citizenRepPenalty = 1.5f;
                }
                break;
            case Faction.Government:
                if (standing == Standing.Good)
                {
                    _govCDR = 0.8f;
                    _govEFF = 1.2f;
                }
                else if (standing == Standing.Neutral)
                {
                    _govCDR = NOT_MODIFIED;
                    _govEFF = NOT_MODIFIED;
                }
                else if (standing == Standing.Bad)
                {
                    _govCDR = 1.15f;
                    _govEFF = 0.9f;
                }
                break;
        }
    }
}
