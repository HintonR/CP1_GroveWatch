using EasyTransition;
using SaintsField;
using TMPro;
using UnityEngine;

public class DrivingHUD : MonoBehaviour
{
    ServiceHub _sH;
    TransitionManager _tM;

    [SerializeField] GameObject _levelInfo, _transportInfo;
    [SerializeField] TextMeshProUGUI _levelName, _levelDesc;
    [SerializeField] TextMeshProUGUI _transportName;
    [SerializeField] GameObject _b1, _b2, _b3;

    public string _levelToLoad;
    public Transform _tpDestination;
    public bool _inScreen;
    public int _mp;

    GameObject _player;

    void Awake()
    {
        _sH = ServiceHub.Instance;
        _tM = TransitionManager.Instance();

        _sH._dUI = this;
    }

    void Start()
    {
        if (_sH._gM._CebuWin)
            _b1.gameObject.SetActive(false);

        if (_sH._gM._BagiuoWin)
            _b2.gameObject.SetActive(false);

        if (_sH._gM._PalawanWin)
            _b3.gameObject.SetActive(false);
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

    public void StartLevel(string levelToLoad)
    {
        Debug.Log("Load " + levelToLoad);
        _mp = 30;
        GameManager.Instance._currentLevelSceneName = levelToLoad;
        CutsceneSceneDirector.PlayLevel(levelToLoad, _mp); //added
        //For Trey - Use _levelToLoad string for TransitionManager/SceneManagement to load or save next scene
        //Because this scene is the level to be loaded AFTER a cutscene.
    }

}
