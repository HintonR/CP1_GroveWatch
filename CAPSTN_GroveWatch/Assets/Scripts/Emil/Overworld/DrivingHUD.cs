using System.Collections;
using System.Collections.Generic;
using EasyTransition;
using TMPro;
using UnityEngine;

public class DrivingHUD : MonoBehaviour
{
    ServiceHub _sH;
    TransitionManager _tM;

    [SerializeField] GameObject _levelInfo, _transportInfo;
    [SerializeField] TextMeshProUGUI _levelName, _levelDesc;
    [SerializeField] TextMeshProUGUI _transportName;

    public string _levelToLoad;
    public Transform _tpDestination;
    public bool _inScreen;

    GameObject _player;

    void Awake()
    {
        _sH = ServiceHub.Instance;
        _tM = TransitionManager.Instance();

        _sH._dUI = this;
    }

    public void OpenLevelInfo()
    {
        _inScreen = true;
        _levelInfo.gameObject.SetActive(true);
    }

    public void CloseLevelInfo()
    {
        _inScreen = false;
        _levelInfo.gameObject.SetActive(false);
    }

    public void OpenTransportMenu(GameObject p)
    {
        _inScreen = true;
        _transportInfo.gameObject.SetActive(true);
        _player = p;
    }

    public void CloseTransportMenu()
    {
        _inScreen = false;
        _transportInfo.gameObject.SetActive(false);
    }

    public void UpdateLevelName(string value)
    {
        _levelName.text = value;
    }
    public void UpdateLevelDesc(string value)
    {
        _levelDesc.text = value;
    }
    public void UpdateTransportName(string value)
    {
        _transportName.text = value;
    }

    public void MoveToDestination()
    {
        if (_player.TryGetComponent(out Rigidbody playerBody))
        {
            playerBody.velocity = Vector3.zero;
            playerBody.angularVelocity = Vector3.zero;
            playerBody.position = _tpDestination.position;
            playerBody.rotation = _tpDestination.rotation;
            playerBody.Sleep();
            Physics.SyncTransforms();
        }
        else
        {
            _player.transform.SetPositionAndRotation(
                _tpDestination.position,
                _tpDestination.rotation);
        }

        CloseTransportMenu();
    }

    public void StartLevel()
    {
        Debug.Log("Load " + _levelToLoad);
        //For Trey - Use _levelToLoad string for TransitionManager/SceneManagement to load or save next scene
        //Because this scene is the level to be loaded AFTER a cutscene.
    }

}
