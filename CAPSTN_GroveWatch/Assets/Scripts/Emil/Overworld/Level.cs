using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    ServiceHub _sH;

    [SerializeField] string _levelName, _levelDescription;
    

    void Awake()
    {
        _sH = ServiceHub.Instance;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _sH._dUI.UpdateLevelName(_levelName);
            _sH._dUI.UpdateLevelDesc(_levelDescription);
            _sH._dUI._levelToLoad = _levelName;
            _sH._dUI.OpenLevelInfo();
        }
    }
}
