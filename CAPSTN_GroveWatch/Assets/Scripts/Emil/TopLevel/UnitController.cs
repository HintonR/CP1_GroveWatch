using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    ServiceHub _sH;

    float _penalty = -15f;
    void Awake()
    {
        ServiceHub.Instance._unit = this;
        _sH = ServiceHub.Instance;
    }
    public bool TryResolveEvent(UnitDrag unitDrag, Forest forest)
    {
        if (unitDrag == null || forest == null || forest.IsResolving) return false;
        if (!unitDrag.IsAvailable) return false;

        NegativeForestState nfs = forest.CurrentState as NegativeForestState;

        if (nfs == null)
            return false;
        
        if (nfs.RequiredUnit != unitDrag.UnitData.Type)
        {
            _sH._gM.ChangeReputation(_penalty);
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
