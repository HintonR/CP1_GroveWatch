using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Set2D : MonoBehaviour
{
    void Awake()
    {
        ServiceHub.Instance._gM._is3D = false;
    }
}
