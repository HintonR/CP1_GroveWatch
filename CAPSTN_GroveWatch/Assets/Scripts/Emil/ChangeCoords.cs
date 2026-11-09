using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChangeCoords : MonoBehaviour
{
    [SerializeField] Image _coords;
    [SerializeField] Sprite _cebu, _baguio, _palawan;

    void OnEnable()
    {
        switch (SceneManager.GetActiveScene().name.ToString())
        {
            case "Cebu"    : _coords.sprite = _cebu; break;
            case "Bagiuo"  : _coords.sprite = _baguio; break;
            case "Palawan" : _coords.sprite = _palawan; break;
            default        : _coords.sprite = _cebu; break;
        }
    }
}
