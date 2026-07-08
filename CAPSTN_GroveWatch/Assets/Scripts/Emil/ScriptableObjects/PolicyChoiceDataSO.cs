using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BonusType
{
    CD,
    EF
}

[CreateAssetMenu(menuName = "Policy Choice Data")]
public class PolicyChoiceDataSO : ScriptableObject
{
    [SerializeField] string _title;
    [SerializeField] int _rep, _budget;
    [SerializeField] float _cooldown, _effectiveness;

    public string Title => _title;
    public int Rep => _rep;
    public int Budget => _budget;
    
    public string GetSign(BonusType type)
    {
        string ToReturn = "";
        if (type == BonusType.CD)
        {
            if (_cooldown > 1)
                ToReturn = "<color=#F25959>+</color>";
            else if (_cooldown < 1)
                ToReturn = "<color=#59D959>--</color>";
        }
        else
        {
            if (_effectiveness > 1)
                ToReturn = "<color=#59D959>+</color>";
            else if (_effectiveness < 1)
                ToReturn = "<color=#F25959>--</color>";
        }

        return ToReturn;
    }

    public void ApplyChoice()
    {
        float repBonusPercent = _rep / 100f;
        ServiceHub.Instance._gM.ChangeReputation(ServiceHub.Instance._gM._maxReputation * repBonusPercent);
        ServiceHub.Instance._gM.ChangeMoney(_budget);
        ServiceHub.Instance._gMods._policyCDR = _cooldown;
        ServiceHub.Instance._gMods._policyEFF = _effectiveness;
    }


}
