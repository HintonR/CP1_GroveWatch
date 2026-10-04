using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOn3D : MonoBehaviour
{
    ServiceHub _sH;

    void Awake()
    {
        _sH = ServiceHub.Instance;
    }
    void Start()
    {
        if (_sH._gM._is3D)
            gameObject.SetActive(false);
    }

}
