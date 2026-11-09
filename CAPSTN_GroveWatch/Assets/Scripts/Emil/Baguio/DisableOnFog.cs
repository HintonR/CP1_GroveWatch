using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnFog : MonoBehaviour
{
    void OnEnable()
    {
        gameObject.SetActive(!ServiceHub.Instance._gM._hasFog);
    }
}
