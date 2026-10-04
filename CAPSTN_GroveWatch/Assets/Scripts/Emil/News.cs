using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class News : MonoBehaviour
{
    public const float UNIT_CEILING = 1.2f;
    public const float UNIT_FLOOR = 0.8f; 
    ServiceHub _sH;
    [SerializeField] TextMeshProUGUI _policy, _decision, _year, _season, _dispatches, 
                    _allowance, _deployment, _tourIncentive;

    [SerializeField] Image _CD, _EF;
    [SerializeField] Sprite _g1, _g2, _r1, _r2;
    [SerializeField] Slider _tour, _citi, _govt;

    [SerializeField] TextMeshProUGUI[] _e = new TextMeshProUGUI[6];
    
    void Awake()
    {
        _sH = ServiceHub.Instance;
    }

    void OnEnable()
    {
        _sH._nM.UpdateValues();
        UpdatePolicyInfo();
        UpdateSeasonalReport();
        UpdateBudgetInfo();
        UpdateIncidentCounters();
        UpdateFactions();
    }

    void UpdatePolicyInfo()
    {
        _policy.text = _sH._nM._policy;
        _decision.text = _sH._nM._decision;

        var cdValue = _sH._gMods._policyCDR;
        _CD.gameObject.SetActive(true);
        if (cdValue < 1f && cdValue >= UNIT_FLOOR)
            _CD.sprite = _g1;
        if (cdValue < UNIT_FLOOR)
            _CD.sprite = _g2;
        if (cdValue > 1f && cdValue <= UNIT_CEILING)
            _CD.sprite = _r1;
        if (cdValue > UNIT_CEILING)
            _CD.sprite = _r2;
        if (cdValue == 1f)
            _CD.gameObject.SetActive(false);

        var efValue = _sH._gMods._policyEFF;
        _EF.gameObject.SetActive(true);
        if (efValue < 1f && efValue >= UNIT_FLOOR)
            _EF.sprite = _r1;
        if (efValue < UNIT_FLOOR)
            _EF.sprite = _r2;
        if (efValue > 1f && efValue <= UNIT_CEILING)
            _EF.sprite = _g1;
        if (efValue > UNIT_CEILING)
            _EF.sprite = _g2;
        if (efValue == 1f)
            _EF.gameObject.SetActive(false);
    }

    void UpdateSeasonalReport()
    {
        _year.text = _sH._nM._year.ToString();
        _season.text = _sH._nM._season;
        _dispatches.text = _sH._nM._dispatches.ToString();
    }

    void UpdateBudgetInfo()
    {
        _allowance.text = _sH._nM._allowance.ToString();
        _deployment.text = _sH._nM._deployment.ToString();
        _tourIncentive.text = _sH._nM._incentive.ToString();
    }

    void UpdateIncidentCounters()
    {
        for (int i = 0; i < _e.Length; i++)
            _e[i].text = _sH._nM._events[i].ToString();
    }

    void UpdateFactions()
    {
        _tour.value = _sH._nM._f1;
        _citi.value = _sH._nM._f2;
        _govt.value = _sH._nM._f3;
    }

}
