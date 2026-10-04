using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    ServiceHub _sH;

    [SerializeField] Transform _destination;
    [SerializeField] string _vehicleName;

    GameObject _player;

    void Awake()
    {
        _sH = ServiceHub.Instance;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _player = other.gameObject;
            _sH._dUI.UpdateTransportName(_vehicleName);
            _sH._dUI._tpDestination = _destination;
            _sH._dUI.OpenTransportMenu(_player);
            
        }
    }

}
