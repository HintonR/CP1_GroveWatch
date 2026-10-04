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
    [SerializeField] int _tour, _citi, _gov;

    public string Title => _title;
    public int Rep => _rep;
    public int Budget => _budget;
    
    public float GetModifierValue(BonusType type)
    {
        float ToReturn = 0f;
        if (type == BonusType.CD)
            ToReturn = _cooldown;
        if (type == BonusType.EF)
            ToReturn = _effectiveness;

        return ToReturn;
    }

    public int GetFactionValue(Faction faction)
    {
        int ToReturn = 0;
        if (faction == Faction.Tourist)
            ToReturn = _tour;
        if (faction == Faction.Citizen)
            ToReturn = _citi;
        if (faction == Faction.Government)
            ToReturn = _gov;

        return ToReturn;
    }

    public void ApplyChoice()
    {
        float repBonusPercent = _rep / 100f;
        ServiceHub.Instance._gM.ChangeReputation(ServiceHub.Instance._gM._maxReputation * repBonusPercent);
        ServiceHub.Instance._gM.ChangeMoney(_budget);
        ServiceHub.Instance._gMods.SetPolicyMod(ModifierType.CDR, _cooldown);
        ServiceHub.Instance._gMods.SetPolicyMod(ModifierType.EFF, _effectiveness);
        ServiceHub.Instance._fM.UpdateFactionStanding(Faction.Tourist, _tour);
        ServiceHub.Instance._fM.UpdateFactionStanding(Faction.Citizen, _citi);
        ServiceHub.Instance._fM.UpdateFactionStanding(Faction.Government, _gov);
        ServiceHub.Instance._nM._decision = _title;
    }


}
