using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class QuadrantManager : MonoBehaviour
{
    ServiceHub _sH;

    [SerializeField] Animator _q1, _q2, _q3, _q4;
    [SerializeField] List<Forest> _q1Forests, _q2Forests, _q3Forests, _q4Forests;
    public event Action<int, bool> QuadrantEventStatusChanged;
    public event Action<int> ActiveQuadrantChanged;

    int _activeQuadrant = 1;
    public int ActiveQuadrant => _activeQuadrant;
    bool _q1HasEvent;
    bool _q2HasEvent;
    bool _q3HasEvent;
    bool _q4HasEvent;
    readonly List<Forest> _trackedForests = new List<Forest>();
    readonly List<Forest> _readyForests = new List<Forest>();

    void Awake()
    {
        _sH = ServiceHub.Instance;
        _sH._qM = this;
    }

    void OnEnable()
    {
        RebuildTrackedForests();
        SubscribeToForestEvents(_trackedForests);
        RefreshQuadrantEventStates();
    }

    void OnDisable()
    {
        UnsubscribeFromForestEvents(_trackedForests);
    }

    void Update()
    {
        if (!_sH._time.CanSpawnForestEvent())
            return;

        TrySpawnReadyForestEvent();
    }

    public void SetActiveQuadrant(int q)
    {
        bool changed = _activeQuadrant != q;
        _activeQuadrant = q;

        _q1.SetBool("Out", true);
        _q2.SetBool("Out", true);
        _q3.SetBool("Out", true);
        _q4.SetBool("Out", true);

        switch (q)
        {
            case 1: 
                _q1.SetBool("Out", false); break;
            case 2: 
                _q2.SetBool("Out", false); break;
            case 3: 
                _q3.SetBool("Out", false); break;
            case 4: 
                _q4.SetBool("Out", false); break;
        }

        if (changed)
            ActiveQuadrantChanged?.Invoke(_activeQuadrant);
    }

    public bool HasEventInQuadrant(int quadrant)
    {
        switch (quadrant)
        {
            case 1:
                return _q1HasEvent;
            case 2:
                return _q2HasEvent;
            case 3:
                return _q3HasEvent;
            case 4:
                return _q4HasEvent;
            default:
                return false;
        }
    }

    void SubscribeToForestEvents(List<Forest> forests)
    {
        foreach (Forest forest in forests)
            forest.EventStatusChanged += HandleForestEventStatusChanged;
    }

    void UnsubscribeFromForestEvents(List<Forest> forests)
    {
        foreach (Forest forest in forests)
            forest.EventStatusChanged -= HandleForestEventStatusChanged;
    }

    void HandleForestEventStatusChanged(Forest forest, bool hasEvent)
    {
        RefreshQuadrantEventStates();
    }

    void RefreshQuadrantEventStates()
    {
        UpdateQuadrantEventState(1, _q1Forests);
        UpdateQuadrantEventState(2, _q2Forests);
        UpdateQuadrantEventState(3, _q3Forests);
        UpdateQuadrantEventState(4, _q4Forests);
    }

    void UpdateQuadrantEventState(int quadrant, List<Forest> forests)
    {
        bool hasEvent = QuadrantHasEvent(forests);

        switch (quadrant)
        {
            case 1:
                if (_q1HasEvent == hasEvent)
                    return;

                _q1HasEvent = hasEvent;
                break;
            case 2:
                if (_q2HasEvent == hasEvent)
                    return;

                _q2HasEvent = hasEvent;
                break;
            case 3:
                if (_q3HasEvent == hasEvent)
                    return;

                _q3HasEvent = hasEvent;
                break;
            case 4:
                if (_q4HasEvent == hasEvent)
                    return;

                _q4HasEvent = hasEvent;
                break;
            default:
                return;
        }

        QuadrantEventStatusChanged?.Invoke(quadrant, hasEvent);
    }

    bool QuadrantHasEvent(List<Forest> forests)
    {
        foreach (Forest forest in forests)
        {
            if (forest.HasActiveEvent)
                return true;
        }

        return false;
    }

    void RebuildTrackedForests()
    {
        _trackedForests.Clear();

        AddUniqueForests(_q1Forests);
        AddUniqueForests(_q2Forests);
        AddUniqueForests(_q3Forests);
        AddUniqueForests(_q4Forests);
    }

    void AddUniqueForests(List<Forest> forests)
    {
        foreach (Forest forest in forests)
        {
            if (_trackedForests.Contains(forest))
                continue;

            _trackedForests.Add(forest);
        }
    }

    void TrySpawnReadyForestEvent()
    {
        _readyForests.Clear();

        foreach (Forest forest in _trackedForests)
        {
            if (forest.IsReadyForEventSpawn())
                _readyForests.Add(forest);
        }

        if (_readyForests.Count == 0)
            return;

        Forest selectedForest = _readyForests[UnityEngine.Random.Range(0, _readyForests.Count)];
        selectedForest.TrySpawnEvent();
    }
}
