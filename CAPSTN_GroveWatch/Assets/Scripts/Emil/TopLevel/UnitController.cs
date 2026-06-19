using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    ServiceHub _sH;

    const float PENALTY = 15f;
    void Awake()
    {
        ServiceHub.Instance._unit = this;
        _sH = ServiceHub.Instance;
    }
    public bool TryResolveEvent(UnitDrag unitDrag, Forest forest)
    {
        if (forest.IsResolving) return false;
        if (!unitDrag.IsAvailable) return false;
        if (forest.CurrentState.StateType == ForestState.Idle || forest.CurrentState.StateType == ForestState.Dead)
            return false;

        NegativeForestState nfs = forest.CurrentState as NegativeForestState;
        
        if (nfs.RequiredUnit != unitDrag.UnitData.Type)
        {
            _sH._gM.ChangeReputation(-PENALTY);
            _sH._aM.PlaySFX(SFX.Invalid);
            return false;
        }

        if (!unitDrag.TryCommitToResolution())
            return false;

        if (!forest.TryStartResolution(unitDrag.UnitData, unitDrag))
        {
            unitDrag.CancelResolutionCommit();
            return false;
        }

        return true;
    }
}
