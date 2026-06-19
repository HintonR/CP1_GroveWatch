using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public enum ResearchId
{
    FireUnit = 1,
    RangerUnit = 2,
    PoliceUnit = 3,
    FireCdr = 4,
    RangerCdr = 5,
    PoliceCdr = 6,
    FireEffect = 7,
    RangerEffect = 8,
    PoliceEffect = 9,
    ForestHealth = 10,
    Reputation = 11
}

public class Research : MonoBehaviour
{
    const float CHARACTER_SPEED = 30f;
    ServiceHub _sH;

    [SerializeField] TextMeshProUGUI _funit, _runit, _punit, _fcdr, _rcdr, _pcdr, _feff, _reff, _peff, _forest, _rep;
    [SerializeField] TextMeshProUGUI _finc, _rinc, _pinc;
    [SerializeField] TextMeshProUGUI _name, _desc, _cost, _money;
    [SerializeField] UnitData _funitd, _runitd, _punitd;
    [SerializeField] GameObject _nameP, _purchase, _descP, _costP;

    [SerializeField] List<ResearchSO> _research;
    
    Coroutine _nameRoutine;
    Coroutine _descRoutine;
    Coroutine _costRoutine;

    ResearchSO _selectedResearch;

    Dictionary<ResearchId, ResearchSO> _researchById = new Dictionary<ResearchId, ResearchSO>();
    Dictionary<ResearchId, int> _researchLevels = new Dictionary<ResearchId, int>();
    Dictionary<ResearchId, TextMeshProUGUI> _progressTextsById = new Dictionary<ResearchId, TextMeshProUGUI>();
    Dictionary<ResearchId, UnitType> _incomeUnitsByResearchId = new Dictionary<ResearchId, UnitType>();

    void OnEnable()
    {
        UpdateMoney();
    }

    void OnDisable()
    {
        _selectedResearch = null;
        ClearText();
        _purchase.SetActive(false);
        _nameP.SetActive(false);
        _descP.SetActive(false);
        _costP.SetActive(false);
    }

    void Awake()
    {
        _sH = ServiceHub.Instance;
    }

    void Start()
    {
        ClearText();
        CacheProgressTexts();
        CacheIncomeUpgradeUnits();
        CacheResearchLevels();
        CacheResearch();
        RefreshAllProgressTexts();
        RefreshIncomeTexts();
    }

    void StartTypewriter(TextMeshProUGUI textComponent, string content, float speed, ref Coroutine routine)
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(TypewriterRoutine(textComponent, content, speed));
    }

    IEnumerator TypewriterRoutine(TextMeshProUGUI textComponent, string fullText, float speed)
    {
        textComponent.text = fullText;
        textComponent.maxVisibleCharacters = 0;

        int total = fullText.Length;
        float interval = 1f / speed;
        float timer = 0f;
        int visible = 0;

        while (visible < total)
        {
            timer += Time.deltaTime;

            while (timer >= interval && visible < total)
            {
                timer -= interval;
                visible++;
                textComponent.maxVisibleCharacters = visible;
            }

            yield return null;
        }

        textComponent.maxVisibleCharacters = total;
    }

    public void SelectUpgrade(int i)
    {
        _sH._aM.PlaySFX(SFX.Rune);
        SelectUpgrade((ResearchId)i);
    }

    public void SelectUpgrade(ResearchId researchId)
    {
        if (!_researchById.TryGetValue(researchId, out ResearchSO research))
            return;

        _selectedResearch = research;

        StartTypewriter(_name, research.ResearchName, CHARACTER_SPEED, ref _nameRoutine);        
        _desc.text = research.Description;
        RefreshSelectedUpgrade();
        _purchase.SetActive(true);
        _nameP.SetActive(true);
        _descP.SetActive(true);
        _costP.SetActive(true);
    }

    public void PurchaseUpgrade()
    {
        if (!TryGetPurchaseData(out ResearchSO research, out int cost))
            return;

        if (_sH._gM._money < cost)
        {
            _sH._aM.PlaySFX(SFX.Invalid);
            //return;
        }

        _sH._gM.ChangeMoney(-cost);
        _sH._aM.PlaySFX(SFX.Purchase);
        IncreaseResearchLevel(research.ID);
        RefreshProgressText(research);
        ApplyUpgrade(research.ID);
        RefreshIncomeTexts();
        RefreshSelectedUpgrade();
    }

    void ClearText()
    {
        if (_nameRoutine != null) StopCoroutine(_nameRoutine);
        if (_descRoutine != null) StopCoroutine(_descRoutine);
        if (_costRoutine != null) StopCoroutine(_costRoutine);

        _name.text = "";
        _desc.text = "";
        _cost.text = "";
    }

    void IncreaseResearchLevel(ResearchId researchId)
    {
        if (!_researchById.TryGetValue(researchId, out ResearchSO research))
            return;

        int currentLevel = GetCurrentLevel(researchId);

        if (currentLevel >= research.Cap)
            return;

        SetCurrentLevel(researchId, currentLevel + 1);
        RefreshProgressText(research);

        if (_selectedResearch != null && _selectedResearch.ID == researchId)
            RefreshSelectedUpgrade();
    }

    void RefreshIncomeTexts()
    {
        _finc.text = _sH._iM.GetIncomeForUnit(_funitd) + "php";
        _rinc.text = _sH._iM.GetIncomeForUnit(_runitd) + "php";
        _pinc.text = _sH._iM.GetIncomeForUnit(_punitd) + "php";
    }

    void CacheResearch()
    {
        _researchById.Clear();

        foreach (ResearchSO research in _research)
            _researchById[research.ID] = research;
    }

    void CacheResearchLevels()
    {
        _researchLevels.Clear();

        foreach (ResearchId researchId in Enum.GetValues(typeof(ResearchId)))
            _researchLevels[researchId] = 0;
    }

    void CacheProgressTexts()
    {
        _progressTextsById = new Dictionary<ResearchId, TextMeshProUGUI>
        {
            { ResearchId.FireUnit, _funit },
            { ResearchId.FireCdr, _fcdr },
            { ResearchId.FireEffect, _feff },
            { ResearchId.RangerUnit, _runit },
            { ResearchId.RangerCdr, _rcdr },
            { ResearchId.RangerEffect, _reff },
            { ResearchId.PoliceUnit, _punit },
            { ResearchId.PoliceCdr, _pcdr },
            { ResearchId.PoliceEffect, _peff },
            { ResearchId.ForestHealth, _forest },
            { ResearchId.Reputation, _rep }
        };
    }

    void CacheIncomeUpgradeUnits()
    {
        _incomeUnitsByResearchId = new Dictionary<ResearchId, UnitType>
        {
            { ResearchId.FireUnit, UnitType.Firefighter },
            { ResearchId.FireCdr, UnitType.Firefighter },
            { ResearchId.FireEffect, UnitType.Firefighter },
            { ResearchId.RangerUnit, UnitType.Ranger },
            { ResearchId.RangerCdr, UnitType.Ranger },
            { ResearchId.RangerEffect, UnitType.Ranger },
            { ResearchId.PoliceUnit, UnitType.Police },
            { ResearchId.PoliceCdr, UnitType.Police },
            { ResearchId.PoliceEffect, UnitType.Police }
        };
    }

    void RefreshAllProgressTexts()
    {
        foreach (ResearchSO research in _research)
            RefreshProgressText(research);
    }

    void RefreshProgressText(ResearchSO research)
    {
        if (!_progressTextsById.TryGetValue(research.ID, out TextMeshProUGUI progressText) || progressText == null)
            return;

        progressText.text = $"{GetCurrentLevel(research.ID)}/{research.Cap}";
    }

    void RefreshSelectedUpgrade()
    {
        if (_selectedResearch == null)
            return;

        int currentLevel = GetCurrentLevel(_selectedResearch.ID);
        int nextCost = _selectedResearch.GetCostForLevel(currentLevel);

        string costText = nextCost < 0 ? "<color=#FFD700>MAXED</color>" : nextCost + "php";

        _cost.text = costText;
        UpdateMoney();
    }

    void UpdateMoney()
    {
        _money.text = _sH._gM._money + "php";
    }

    bool TryGetPurchaseData(out ResearchSO research, out int cost)
    {
        research = _selectedResearch;
        cost = -1;

        int currentLevel = GetCurrentLevel(research.ID);

        if (currentLevel >= research.Cap)
            return false;

        cost = research.GetCostForLevel(currentLevel);
        return cost >= 0;
    }

    void ApplyIncomeUpgrade(ResearchId researchId)
    {
        if (!_incomeUnitsByResearchId.TryGetValue(researchId, out UnitType unitType))
            return;

        _sH._iM.IncreaseUpgrade(unitType);
    }

    int GetCurrentLevel(ResearchId researchId)
    {
        return _researchLevels.TryGetValue(researchId, out int currentLevel) ? currentLevel : 0;
    }

    void SetCurrentLevel(ResearchId researchId, int value)
    {
        _researchLevels[researchId] = value;
    }

    void ApplyUpgrade(ResearchId researchId)
    {
        int currentLevel = GetCurrentLevel(researchId);
        float modifier;

        switch (researchId)
        {
            case ResearchId.FireUnit:
                _sH._UI.ActivateUnit(UnitType.Firefighter, currentLevel);
                ApplyIncomeUpgrade(researchId);
                break;

            case ResearchId.RangerUnit:
                _sH._UI.ActivateUnit(UnitType.Ranger, currentLevel);
                ApplyIncomeUpgrade(researchId);
                break;

            case ResearchId.PoliceUnit:
                _sH._UI.ActivateUnit(UnitType.Police, currentLevel);
                ApplyIncomeUpgrade(researchId);
                break;

            case ResearchId.FireCdr:
                modifier = GetUnitModifierValue(UnitType.Firefighter, ModifierType.CDR, currentLevel);
                _sH._gMods.SetResearchMod(ModifierType.CDR, UnitType.Firefighter, modifier);
                ApplyIncomeUpgrade(researchId);
                break;

            case ResearchId.RangerCdr:
                modifier = GetUnitModifierValue(UnitType.Ranger, ModifierType.CDR, currentLevel);
                _sH._gMods.SetResearchMod(ModifierType.CDR, UnitType.Ranger, modifier);
                ApplyIncomeUpgrade(researchId);
                break;

            case ResearchId.PoliceCdr:
                modifier = GetUnitModifierValue(UnitType.Police, ModifierType.CDR, currentLevel);
                _sH._gMods.SetResearchMod(ModifierType.CDR, UnitType.Police, modifier);
                ApplyIncomeUpgrade(researchId);
                break;

            case ResearchId.FireEffect:
                modifier = GetUnitModifierValue(UnitType.Firefighter, ModifierType.EFF, currentLevel);
                _sH._gMods.SetResearchMod(ModifierType.EFF, UnitType.Firefighter, modifier);
                ApplyIncomeUpgrade(researchId);
                break;

            case ResearchId.RangerEffect:
                modifier = GetUnitModifierValue(UnitType.Ranger, ModifierType.EFF, currentLevel);
                _sH._gMods.SetResearchMod(ModifierType.EFF, UnitType.Ranger, modifier);
                ApplyIncomeUpgrade(researchId);
                break;

            case ResearchId.PoliceEffect:
                modifier = GetUnitModifierValue(UnitType.Police, ModifierType.EFF, currentLevel);
                _sH._gMods.SetResearchMod(ModifierType.EFF, UnitType.Police, modifier);
                ApplyIncomeUpgrade(researchId);
                break;

            case ResearchId.ForestHealth:
                modifier = GetHealthModifier(currentLevel);           
                _sH._gMods.SetForestHealthMod(modifier);
                break;

            case ResearchId.Reputation:
                modifier = GetReputationModifier(currentLevel);
                _sH._gMods.SetReputationBonus(modifier);
                break;
        }
    }

    float GetHealthModifier(int cLevel)
    {
        return cLevel switch
        {
            1 => 1.2f,
            2 => 1.4f,
            3 => 1.6f,
            4 => 1.8f,
            5 => 2f,
            _ => 0f
        };
    }

    float GetReputationModifier(int cLevel)
    {
        return cLevel switch
        {
            1 => 1.1f,
            2 => 1.2f,
            3 => 1.3f,
            4 => 1.4f,
            5 => 1.5f,
            _ => 0f
        };
    }

    float GetUnitModifierValue(UnitType uType, ModifierType mType, int cLevel)
    {
        return (uType, mType, cLevel) switch
        {
            (UnitType.Firefighter, ModifierType.CDR, 1) => 0.8f,
            (UnitType.Firefighter, ModifierType.CDR, 2) => 0.6f,
            (UnitType.Firefighter, ModifierType.CDR, 3) => 0.4f,
            (UnitType.Firefighter, ModifierType.EFF, 1) => 1.5f,
            (UnitType.Firefighter, ModifierType.EFF, 2) => 2f,
            (UnitType.Firefighter, ModifierType.EFF, 3) => 2.5f,

            (UnitType.Ranger, ModifierType.CDR, 1) => 0.9f,
            (UnitType.Ranger, ModifierType.CDR, 2) => 0.7f,
            (UnitType.Ranger, ModifierType.CDR, 3) => 0.5f,
            (UnitType.Ranger, ModifierType.EFF, 1) => 1.2f,
            (UnitType.Ranger, ModifierType.EFF, 2) => 1.8f,
            (UnitType.Ranger, ModifierType.EFF, 3) => 2.1f,

            (UnitType.Police, ModifierType.CDR, 1) => 0.9f,
            (UnitType.Police, ModifierType.CDR, 2) => 0.8f,
            (UnitType.Police, ModifierType.CDR, 3) => 0.7f,
            (UnitType.Police, ModifierType.EFF, 1) => 1.5f,
            (UnitType.Police, ModifierType.EFF, 2) => 2f,
            (UnitType.Police, ModifierType.EFF, 3) => 2.75f,
            _ => 0f
        };
    }
}
