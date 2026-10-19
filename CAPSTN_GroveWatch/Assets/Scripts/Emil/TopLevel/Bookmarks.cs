using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bookmarks : MonoBehaviour
{
    ServiceHub _sH;

    public Transform _b1, _b2, _b3, _b4;

    void Awake()
    {
        _sH = ServiceHub.Instance;
        _sH._bM = this;
    }
}
