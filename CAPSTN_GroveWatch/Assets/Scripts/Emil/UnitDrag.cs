using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    ServiceHub _sH;

    [SerializeField] UnitData _unitData;
    [SerializeField] Image _cd;

    Vector2 _originalPos;
    RectTransform _rect;
    Canvas _canvas;
    CanvasGroup _canvasGroup;
    Coroutine _cooldownRoutine;
    bool _isBusy;

    public UnitData UnitData => _unitData;
    public bool IsAvailable => !_isBusy;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (_cd != null)
        {
            _cd.fillAmount = 0f;
            _cd.gameObject.SetActive(false);
        }

        UpdateAvailabilityVisual();
    }

    void Start()
    {
        _sH = ServiceHub.Instance;
        _originalPos = _rect.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsAvailable) return;

        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsAvailable) return;

        _rect.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsAvailable)
        {
            _rect.anchoredPosition = _originalPos;
            return;
        }

        _canvasGroup.blocksRaycasts = true;

        TryDropOnWorld(eventData);
    }

    void TryDropOnWorld(PointerEventData eventData)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null)
        {
            Forest forest = hit.collider.GetComponent<Forest>();

            if (forest != null)
                _sH._unit.TryResolveEvent(this, forest);
        }

        _rect.anchoredPosition = _originalPos;
    }

    public bool TryCommitToResolution()
    {
        if (_isBusy) return false;

        _isBusy = true;
        UpdateAvailabilityVisual();
        return true;
    }

    public void CancelResolutionCommit()
    {
        if (!_isBusy) return;

        _isBusy = false;
        UpdateAvailabilityVisual();
    }

    public void StartCooldown()
    {
        if (_cooldownRoutine != null)
            StopCoroutine(_cooldownRoutine);

        _cd.gameObject.SetActive(true);
        _cooldownRoutine = StartCoroutine(CooldownRoutine());
    }

    IEnumerator CooldownRoutine()
    {
        float modifier = UnitData.Type switch
        {
            UnitType.Firefighter => _sH._gMods._researchFCDR,
            UnitType.Ranger      => _sH._gMods._researchRCDR,
            UnitType.Police      => _sH._gMods._researchPCDR,
            _ => 0f
        };

        float cooldown = _unitData.Cooldown * modifier * _sH._gMods._policyCDR;

        if (cooldown <= 0f)
        {
            FinishCooldown();
            yield break;
        }

        float elapsedTime = 0f;

        while (elapsedTime < cooldown)
        {
            if (_sH._gM._isPaused || _sH._gM._inScreen)
            {
                yield return null;
                continue;
            }
            elapsedTime += Time.deltaTime;

            _cd.fillAmount = Mathf.Lerp(1f, 0f, elapsedTime / cooldown);

            yield return null;
        }

        FinishCooldown();
    }

    void FinishCooldown()
    {
        _cd.fillAmount = 0f;
        _cd.gameObject.SetActive(false);

        _cooldownRoutine = null;
        _isBusy = false;
        UpdateAvailabilityVisual();
    }

    void UpdateAvailabilityVisual()
    {
        if (_canvasGroup == null) return;

        _canvasGroup.alpha = _isBusy ? 0.6f : 1f;
    }
}
