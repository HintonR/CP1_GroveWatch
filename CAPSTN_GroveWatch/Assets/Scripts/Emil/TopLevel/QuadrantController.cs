using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class QuadrantController : MonoBehaviour
{
    ServiceHub _sH;
    [SerializeField] Image _q1, _q2, _q3, _q4;
    [SerializeField] Sprite _q1A, _q1N, _q2A, _q2N, _q3A, _q3N, _q4A, _q4N;
    [SerializeField] GameObject _a1, _a2, _a3, _a4;

    void Awake()
    {
        _sH = ServiceHub.Instance;
    }

    void OnEnable()
    {
        RegisterQuadrantManagerListener();
        RefreshAlerts();
    }

    void Start()
    {
        RegisterQuadrantManagerListener();
        RefreshAlerts();
    }

    void OnDisable()
    {
        if (_sH != null && _sH._qM != null)
        {
            _sH._qM.QuadrantEventStatusChanged -= HandleQuadrantEventStatusChanged;
            _sH._qM.ActiveQuadrantChanged -= HandleActiveQuadrantChanged;
        }
    }

    void Update()
    {
        if (_sH._gM._inScreen)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) ActivateQ1();
        if (Input.GetKeyDown(KeyCode.Alpha2)) ActivateQ2();
        if (Input.GetKeyDown(KeyCode.Alpha3)) ActivateQ3();
        if (Input.GetKeyDown(KeyCode.Alpha4)) ActivateQ4();
    }

    public void ActivateQ1()
    {
        if (_sH._qM.ActiveQuadrant == 1)
            return;

        _sH._qM.SetActiveQuadrant(1);
        _sH._aM.PlaySFX(SFX.Switch);
        DeactivateAll();
        _q1.sprite = _q1A;
    }
    public void ActivateQ2()
    {
        if (_sH._qM.ActiveQuadrant == 2)
            return;

        _sH._qM.SetActiveQuadrant(2);
        _sH._aM.PlaySFX(SFX.Switch);
        DeactivateAll();
        _q2.sprite = _q2A;
    }
    public void ActivateQ3()
    {
        if (_sH._qM.ActiveQuadrant == 3)
            return;

        _sH._qM.SetActiveQuadrant(3);
        _sH._aM.PlaySFX(SFX.Switch);
        DeactivateAll();
        _q3.sprite = _q3A;
    }
    public void ActivateQ4()
    {
        if (_sH._qM.ActiveQuadrant == 4)
            return;

        _sH._qM.SetActiveQuadrant(4);
        _sH._aM.PlaySFX(SFX.Switch);
        DeactivateAll();
        _q4.sprite = _q4A;
    }

    void DeactivateAll()
    {
        _q1.sprite = _q1N;
        _q2.sprite = _q2N;
        _q3.sprite = _q3N;
        _q4.sprite = _q4N;
    }

    void HandleQuadrantEventStatusChanged(int quadrant, bool hasEvent)
    {
        SetAlert(quadrant, hasEvent);
    }

    void HandleActiveQuadrantChanged(int activeQuadrant)
    {
        RefreshAlerts();
    }

    void RefreshAlerts()
    {
        if (_sH._qM == null)
        {
            SetAlert(1, false);
            SetAlert(2, false);
            SetAlert(3, false);
            SetAlert(4, false);
            return;
        }

        SetAlert(1, _sH._qM.HasEventInQuadrant(1));
        SetAlert(2, _sH._qM.HasEventInQuadrant(2));
        SetAlert(3, _sH._qM.HasEventInQuadrant(3));
        SetAlert(4, _sH._qM.HasEventInQuadrant(4));
    }

    void SetAlert(int quadrant, bool isActive)
    {
        bool shouldShow = isActive && ShouldShowAlertForQuadrant(quadrant);

        switch (quadrant)
        {
            case 1: if (_a1 != null) _a1.SetActive(shouldShow); break;
            case 2: if (_a2 != null) _a2.SetActive(shouldShow); break;
            case 3: if (_a3 != null) _a3.SetActive(shouldShow); break;
            case 4: if (_a4 != null) _a4.SetActive(shouldShow); break;
        }
    }

    void RegisterQuadrantManagerListener()
    {
        if (_sH == null || _sH._qM == null)
            return;

        _sH._qM.QuadrantEventStatusChanged -= HandleQuadrantEventStatusChanged;
        _sH._qM.QuadrantEventStatusChanged += HandleQuadrantEventStatusChanged;
        _sH._qM.ActiveQuadrantChanged -= HandleActiveQuadrantChanged;
        _sH._qM.ActiveQuadrantChanged += HandleActiveQuadrantChanged;
    }

    bool ShouldShowAlertForQuadrant(int quadrant)
    {
        if (_sH == null || _sH._qM == null)
            return false;

        return _sH._qM.ActiveQuadrant != quadrant;
    }
}
