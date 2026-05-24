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
    ServiceHub _sH;

    [SerializeField] TextMeshProUGUI _funit, _runit, _punit, _fcdr, _rcdr, _pcdr, _feff, _reff, _peff, _forest, _rep;
    [SerializeField] TextMeshProUGUI _finc, _rinc, _pinc;
    [SerializeField] TextMeshProUGUI _name, _desc, _cost, _money;
    [SerializeField] UnitData _funitd, _runitd, _punitd;

    [SerializeField] List<ResearchSO> _research;

    ResearchSO _selectedResearch;

    int _funitc, _runitc, _punitc, _fcdrc, _rcdrc, _pcdrc, _feffc, _reffc, _peffc, _forestc, _repc;
    Dictionary<int, ResearchSO> _researchById = new Dictionary<int, ResearchSO>();

    void OnEnable()
    {
        UpdateMoney();
    }

    void OnDisable()
    {
        _selectedResearch = null;
        ClearText();
    }

    void Awake()
    {
        _sH = ServiceHub.Instance;
    }

    void Start()
    {
        ClearText();
        CacheResearch();
        RefreshAllProgressTexts();
        RefreshIncomeTexts();
    }

    public void SelectUpgrade(int i)
    {
        if (!_researchById.TryGetValue(i, out ResearchSO research))
            return;

        _selectedResearch = research;

        _name.text = research.ResearchName;
        _desc.text = research.Description;
        RefreshSelectedUpgrade();
    }

    public void PurchaseUpgrade()
    {
        if (!TryGetPurchaseData(out ResearchSO research, out int currentLevel, out int cost))
            return;

        if (_sH._gM._money < cost) { Debug.Log("Can't Purchase"); } //Play Sound Effect

        _sH._gM.ChangeMoney(-cost);
        IncreaseResearchLevel(research.ID);
        RefreshProgressText(research);
        ApplyIncomeUpgrade(research.ID);
        RefreshIncomeTexts();
        RefreshSelectedUpgrade();
    }

    void ClearText()
    {
        _name.text = "";
        _desc.text = "";
        _cost.text = "";
    }

    void IncreaseResearchLevel(int researchId)
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
        _finc.text = "Income: \n" + _sH._iM.GetIncomeForUnit(_funitd);
        _rinc.text = "Income: \n" + _sH._iM.GetIncomeForUnit(_runitd);
        _pinc.text = "Income: \n" + _sH._iM.GetIncomeForUnit(_punitd);
    }

    void CacheResearch()
    {
        _researchById.Clear();

        foreach (ResearchSO research in _research)
            _researchById[research.ID] = research;
    }

    void RefreshAllProgressTexts()
    {
        foreach (ResearchSO research in _research)
            RefreshProgressText(research);
    }

    void RefreshProgressText(ResearchSO research)
    {
        TextMeshProUGUI progressText = GetProgressText(research.ID);
        progressText.text = $"{GetCurrentLevel(research.ID)}/{research.Cap}";
    }

    void RefreshSelectedUpgrade()
    {
        if (_selectedResearch == null)
            return;

        int currentLevel = GetCurrentLevel(_selectedResearch.ID);
        int nextCost = _selectedResearch.GetCostForLevel(currentLevel);

        _cost.text = nextCost < 0 ? "MAXED" : nextCost.ToString();
        UpdateMoney();
    }

    void UpdateMoney()
    {
        _money.text = _sH._gM._money + "php";
    }

    bool TryGetPurchaseData(out ResearchSO research, out int currentLevel, out int cost)
    {
        research = _selectedResearch;
        currentLevel = 0;
        cost = -1;

        if (research == null)
            return false;

        currentLevel = GetCurrentLevel(research.ID);

        if (currentLevel >= research.Cap)
            return false;

        cost = research.GetCostForLevel(currentLevel);
        return cost >= 0;
    }

    void ApplyIncomeUpgrade(int researchId)
    {

        if (!TryGetIncomeUnitType(researchId, out UnitType unitType))
            return;

        _sH._iM.IncreaseUpgrade(unitType);
    }

    bool TryGetIncomeUnitType(int researchId, out UnitType unitType)
    {
        switch ((ResearchId)researchId)
        {
            case ResearchId.FireUnit:
            case ResearchId.FireCdr:
            case ResearchId.FireEffect:
                unitType = UnitType.Firefighter;
                return true;

            case ResearchId.RangerUnit:
            case ResearchId.RangerCdr:
            case ResearchId.RangerEffect:
                unitType = UnitType.Ranger;
                return true;

            case ResearchId.PoliceUnit:
            case ResearchId.PoliceCdr:
            case ResearchId.PoliceEffect:
                unitType = UnitType.Police;
                return true;

            default:
                unitType = default;
                return false;
        }
    }

    int GetCurrentLevel(int researchId)
    {
        return (ResearchId)researchId switch
        {
            ResearchId.FireUnit => _funitc,
            ResearchId.RangerUnit => _runitc,
            ResearchId.PoliceUnit => _punitc,

            ResearchId.FireCdr => _fcdrc,
            ResearchId.RangerCdr => _rcdrc,
            ResearchId.PoliceCdr => _pcdrc,
            
            ResearchId.FireEffect => _feffc,
            ResearchId.PoliceEffect => _peffc,
            ResearchId.RangerEffect => _reffc,
            
            ResearchId.ForestHealth => _forestc,
            ResearchId.Reputation => _repc,
            _ => 0
        };
    }

    void SetCurrentLevel(int researchId, int value)
    {
        switch ((ResearchId)researchId)
        {
            case ResearchId.FireUnit:      _funitc  = value; break;
            case ResearchId.RangerUnit:    _runitc  = value; break;
            case ResearchId.PoliceUnit:    _punitc  = value; break;

            case ResearchId.FireCdr:       _fcdrc   = value; break;
            case ResearchId.RangerCdr:     _rcdrc   = value; break;
            case ResearchId.PoliceCdr:     _pcdrc   = value; break;

            case ResearchId.FireEffect:    _feffc   = value; break;
            case ResearchId.RangerEffect:  _reffc   = value; break;
            case ResearchId.PoliceEffect:  _peffc   = value; break;

            case ResearchId.ForestHealth:  _forestc = value; break;
            case ResearchId.Reputation:    _repc    = value; break;
        }
    }

    TextMeshProUGUI GetProgressText(int researchId)
    {
        return (ResearchId)researchId switch
        {
            ResearchId.FireUnit => _funit,
            ResearchId.RangerUnit => _runit,
            ResearchId.PoliceUnit => _punit,

            ResearchId.FireCdr => _fcdr,
            ResearchId.RangerCdr => _rcdr,
            ResearchId.PoliceCdr => _pcdr,
            
            ResearchId.FireEffect => _feff,
            ResearchId.RangerEffect => _reff,
            ResearchId.PoliceEffect => _peff,
            
            ResearchId.ForestHealth => _forest,
            ResearchId.Reputation => _rep,
            _ => null
        };
    }
}
