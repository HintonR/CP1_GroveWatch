using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Forest : MonoBehaviour
{
    const float BASE_MAX_HEALTH = 20f;
    const float DEATH_PENALTY = 35f;
    const float RECOVERY_RATE = 1.25f;
    const float RECOVERY_AMOUNT = 1.5f;
    const float UNIT_DAMAGE_RATE = 0.5f;
    const float EVENT_DAMAGE_RATE = 1.25f;
    const float REP_BONUS = 15;
    const float REP_DAMAGE_MOD = 1.25f;
    
    static int _activeNegativeForestCount;
    public static int ActiveNegativeForestCount => _activeNegativeForestCount;
    public event Action<Forest, bool> EventStatusChanged;

    ServiceHub _sH;

    [SerializeField] ForestStateData _baseState;
    [SerializeField] ForestStateData _deadState;
    [SerializeField] List<NegativeForestState> _possibleStates;
    [SerializeField] TextMeshProUGUI _eventName;
    [SerializeField] Image _eventLife, _forestLife;
    [SerializeField] Sprite _fire, _police, _ranger;
    [SerializeField] SpriteRenderer _forestRenderer;

    float _forestHealth;
    float _forestMaxHealth;


    ForestStateData _currentState;
    public ForestStateData CurrentState => _currentState;
    public bool HasActiveEvent => _currentState is NegativeForestState;
    
    Coroutine _stateRoutine;
    Coroutine _resolveRoutine;
    Coroutine _recoveryRoutine;
    
    bool _isResolving;
    public bool IsResolving => _isResolving;

    float _currentEventHealth;
    float _currentEventMaxHealth;


    float _stateTimer;
    float _changeTime;

    bool _countsAsActiveNegative;
    UnitDrag _resolvingUnitDrag;

    void Awake()
    {
        _forestRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        _sH = ServiceHub.Instance;

        ResetForestHealth();
        SetState(_baseState);
        _changeTime = UnityEngine.Random.Range(12, 24);
    }

    void OnEnable()
    {
        if (_currentState is NegativeForestState)
            NegativeStateTracking(true);
    }

    void OnDisable()
    {
        NegativeStateTracking(false);
    }

    void Update()
    {
        if (_currentState == _deadState)
            StartRecovery();
        else if (_currentState is NegativeForestState)
            HandleStateEffects();
        else if (_currentState == _baseState)
            TrackEventSpawnTimer();
    }

    void StartRecovery()
    {
        if (_recoveryRoutine == null)
            _recoveryRoutine = StartCoroutine(RecoveryRoutine());
    }

    void HandleStateEffects()
    {
        if (_currentState is NegativeForestState && _stateRoutine == null)           
            _stateRoutine = StartCoroutine(StateEffectRoutine());
    }

    public void StopStateEffects()
    {
        if (_stateRoutine != null)
        {
            StopCoroutine(_stateRoutine);
            _stateRoutine = null;
        }
    }

    IEnumerator StateEffectRoutine()
    {
        if (_currentState is NegativeForestState nfs)
        {
            while (_currentState == nfs && _currentEventHealth > 0f && _forestHealth > 0f)
            {

                if (_sH._gM._isPaused || _sH._gM._inScreen)
                {
                    yield return null;
                    continue;
                }

                _sH._gM.ChangeReputation(-nfs.Damage * REP_DAMAGE_MOD);
                DamageForestHealth(-nfs.Damage);

                if (_currentState == _deadState)
                    yield break;

                yield return new WaitForSeconds(EVENT_DAMAGE_RATE);
            }
        }

        _stateRoutine = null;
    }

    void DamageForestHealth(float damage)
    {
        _forestHealth += damage;
        UpdateLifeUI(true, _forestLife);

        if (_forestHealth <= 0f)
            EnterDeadState();
    }

    void ResetForestHealth()
    {
        _forestMaxHealth = BASE_MAX_HEALTH * _sH._gMods._health;
        _forestHealth = _forestMaxHealth;
    }

    void TrackEventSpawnTimer()
    {
        if (_sH._gM._isPaused || _sH._gM._inScreen) return;

        if (_currentState.StateType != ForestState.Idle) return;

        _stateTimer += Time.deltaTime;
    }

    public bool IsReadyForEventSpawn()
    {
        if (_sH._gM._isPaused || _sH._gM._inScreen)
            return false;

        if (_currentState.StateType != ForestState.Idle)
            return false;

        return _stateTimer >= _changeTime;
    }

    public bool TrySpawnEvent()
    {
        if (_sH._gM._isPaused || _sH._gM._inScreen)
            return false;

        if (!IsReadyForEventSpawn())
            return false;

        if (!TryRollEvent())
            return false;

        _stateTimer = 0f;
        return true;
    }

    bool TryRollEvent()
    {
        float totalWeight = 0f;

        foreach (var state in _possibleStates)
        {
            if (!CanSpawnState(state)) continue;
            totalWeight += state.Chance;
        }

        if (totalWeight <= 0f)
            return false;

        float roll = UnityEngine.Random.value * totalWeight;

        float cumulative = 0f;

        foreach (var state in _possibleStates)
        {
            if (!CanSpawnState(state)) continue;
            
            cumulative += state.Chance;
            
            if (roll <= cumulative)
            {
                SetState(state);
                return true;
            }
        }

        return false;
    }

    bool CanSpawnState(NegativeForestState state)
    {
        if (state == null)
            return false;

        return _sH._time.IsStateAvailable(state.AvailableSeason);
    }

    void SetState(ForestStateData data)
    {
        bool hadActiveEvent = HasActiveEvent;
        NegativeStateTracking(data is NegativeForestState);
        _currentState = data;
        NotifyEventStatusChanged(hadActiveEvent);
        _eventName.text = _currentState.Name;
        SetForestSprite();
        SetEventSprite();

        if (data is NegativeForestState nfs)
        {
            if (_recoveryRoutine != null)
            {
                StopCoroutine(_recoveryRoutine);
                _recoveryRoutine = null;
            }

            ResetForestHealth();
            _currentEventMaxHealth = nfs.Health;
            _currentEventHealth = nfs.Health;
            UpdateLifeUI(true, _eventLife);
            UpdateLifeUI(true, _forestLife);
            return;
        }

        _currentEventMaxHealth = 0f;
        _currentEventHealth = 0f;
        UpdateLifeUI(false, _eventLife);

        if (data == _deadState)
        {
            UpdateLifeUI(true, _forestLife);
            return;
        }

        ResetForestHealth();
        UpdateLifeUI(false, _forestLife);
    }

    void NegativeStateTracking(bool isNegativeState)
    {
        if (_countsAsActiveNegative == isNegativeState)
            return;

        _activeNegativeForestCount += isNegativeState ? 1 : -1;
        _activeNegativeForestCount = Mathf.Max(0, _activeNegativeForestCount);
        _countsAsActiveNegative = isNegativeState;
    }

    public bool TryStartResolution(UnitData unit, UnitDrag unitDrag)
    {
        if (_isResolving)
            return false;
        
        if (_resolveRoutine != null)
            return false;

        if (!(_currentState is NegativeForestState))
            return false;

        _isResolving = true;
        _sH._aM.PlaySFX(SFX.Dropped);
        _resolvingUnitDrag = unitDrag;
        _resolveRoutine = StartCoroutine(ResolutionRoutine(unit, unitDrag));
        return true;
    }

    IEnumerator ResolutionRoutine(UnitData unit, UnitDrag unitDrag)
    {   
        while (_currentState is NegativeForestState && _currentEventHealth > 0f)
        {
            if (_sH._gM._isPaused || _sH._gM._inScreen)
            {
                yield return null;
                continue;
            }

            float modifier = unit.Type switch
            {
                UnitType.Firefighter => _sH._gMods._researchFEFF,
                UnitType.Ranger      => _sH._gMods._researchREFF,
                UnitType.Police      => _sH._gMods._researchPEFF,
                _ => 0f  
            };    

            float dmg = unit.Damage * modifier * _sH._gMods._policyEFF;

            _currentEventHealth = Mathf.Max(0f, _currentEventHealth - dmg);
            UpdateLifeUI(true, _eventLife);

            if (_currentEventHealth <= 0f)
                break;

            yield return new WaitForSeconds(UNIT_DAMAGE_RATE);
        }

        _resolveRoutine = null;

        if (_currentState is NegativeForestState && _currentEventHealth <= 0f)
        {
            ResolveEvent(unit, unitDrag);
            yield break;
        }

        ClearActiveResolution(true);
    }

    public void ResolveEvent(UnitData resolvedUnit = null, UnitDrag resolvedBy = null)
    {
        StopResolutionRoutine();

        UnitDrag unitToCooldown = resolvedBy != null ? resolvedBy : _resolvingUnitDrag;
        ClearActiveResolution(false);
        
        _changeTime = UnityEngine.Random.Range(12, 24);
        
        StopStateEffects();
        SetState(_baseState);
        
        var modifiedBonus = REP_BONUS * _sH._gMods._rep;
        _sH._aM.PlaySFX(SFX.Success);
        _sH._gM.ChangeReputation(modifiedBonus);
        _sH._gM.ChangeProgress(1f);
        _sH._gM.ChangeMoney(_sH._iM.GetIncomeForUnit(resolvedUnit));

        unitToCooldown?.StartCooldown();
    }

    void EnterDeadState()
    {
        if (_currentState == _deadState)
            return;

        if (TryGetIncidentType(_currentState, out IncidentType incidentType))
            _sH._gM.IncreaseIncident(incidentType);

        StopStateEffects();
        StopResolutionRoutine();
        ClearActiveResolution(false, true);
        SetState(_deadState);
        _sH._gM.ChangeReputation(-DEATH_PENALTY);
        _sH._aM.PlaySFX(SFX.Deforestation);
    }

    IEnumerator RecoveryRoutine()
    {
        while (_currentState == _deadState && _forestHealth < _forestMaxHealth)
        {
            if (_sH._gM._isPaused || _sH._gM._inScreen)
            {
                yield return null;
                continue;
            }

            _forestHealth = Mathf.Min(_forestMaxHealth, _forestHealth + RECOVERY_AMOUNT);
            UpdateLifeUI(true, _forestLife);
            yield return new WaitForSeconds(RECOVERY_RATE);
        }

        _recoveryRoutine = null;

        if (_currentState == _deadState && _forestHealth >= _forestMaxHealth)
            SetState(_baseState);
    }

    void StopResolutionRoutine()
    {
        if (_resolveRoutine == null)
            return;

        StopCoroutine(_resolveRoutine);
        _resolveRoutine = null;
    }

    void ClearActiveResolution(bool cancelCommittedUnit, bool startCooldown = false)
    {
        _isResolving = false;

        if (_resolvingUnitDrag != null)
        {
            if (startCooldown)
                _resolvingUnitDrag.StartCooldown();
            else if (cancelCommittedUnit)
                _resolvingUnitDrag.CancelResolutionCommit();
        }

        _resolvingUnitDrag = null;
    }

    void NotifyEventStatusChanged(bool hadActiveEvent)
    {
        if (hadActiveEvent == HasActiveEvent)
            return;

        EventStatusChanged?.Invoke(this, HasActiveEvent);
    }

    bool TryGetIncidentType(ForestStateData stateData, out IncidentType incidentType)
    {
        incidentType = default;

        switch (stateData.StateType)
        {
            case ForestState.Fire:
                incidentType = IncidentType.Fire;
                return true;
            case ForestState.Drought:
                incidentType = IncidentType.Drought;
                return true;
            case ForestState.Flooded:
                incidentType = IncidentType.Flood;
                return true;
            case ForestState.Camping:
                incidentType = IncidentType.Camping;
                return true;
            case ForestState.Logging:
                incidentType = IncidentType.Logging;
                return true;
            case ForestState.Construction:
                incidentType = IncidentType.Construction;
                return true;
            default:
                return false;
        }
    }

    void UpdateLifeUI(bool isVisible, Image image)
    {
        if (image == null)
            return;

        image.gameObject.SetActive(isVisible);

        if (!isVisible)
        {
            image.fillAmount = 0f;
            return;
        }

        float current = 0;
        float max = 0;
        if (image == _forestLife)
        {
            current = _forestHealth;
            max = _forestMaxHealth;
        }
        else if (image == _eventLife)
        {
            current = _currentEventHealth;
            max = _currentEventMaxHealth;
        }

        image.fillAmount = max <= 0f ? 0f : current / max;
    }

    void SetEventSprite()
    {
        switch (_currentState.StateType)
        {
            case ForestState.Fire:
                _eventLife.sprite = _fire;
                break;

            case ForestState.Drought:
            case ForestState.Flooded:
                _eventLife.sprite = _ranger;
                break;

            case ForestState.Camping:
            case ForestState.Logging:
            case ForestState.Construction:
                _eventLife.sprite = _police;
                break;
        }
    }

    void SetForestSprite()
    {
        _forestRenderer.sprite = _currentState.Image;
    }
}
