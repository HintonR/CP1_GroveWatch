using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnEndless : MonoBehaviour
{
    ServiceHub _sH;
    void Start()
    {
        _sH = ServiceHub.Instance;

        if (_sH._gM._isEndless)
            gameObject.SetActive(false);
    }
}
