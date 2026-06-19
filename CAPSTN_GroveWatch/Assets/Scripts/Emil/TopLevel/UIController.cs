using System.Collections;
using System.Collections.Generic;
using EasyTransition;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    ServiceHub _sH;
    [SerializeField] TransitionSettings _transition;

    [SerializeField] Image _repBar, _progBar, _season;
    [SerializeField] TextMeshProUGUI _money, _month, _year;
    [SerializeField] Sprite _dry, _wet, _dryBG, _wetBG;
    [SerializeField] SpriteRenderer _bg;
    [SerializeField] GameObject _researchScreen;
    [SerializeField] Button _play, _pause;
    [SerializeField] GameObject _funit1, _funit2, _runit1, _runit2, _punit1, _punit2;

    Coroutine _repFill, _progFill;

    public Image RepBar => _repBar;
    public Image ProgBar => _progBar;
    public Coroutine RepFill => _repFill;
    public Coroutine ProgFill => _progFill;

    void Awake()
    {
        ServiceHub.Instance._UI = this;
        _sH = ServiceHub.Instance;
    }

    void Start()
    {
        
        UpdateBar(_sH._gM._progress, _sH._gM._maxProgress, _progBar, _progFill);
        UpdateBar(_sH._gM._reputation, _sH._gM._maxReputation, _repBar, _repFill);
        UpdateMoney();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !_sH._gM._inScreen)
        {         
            if (_sH._gM._isPaused)
                PlayGameplay();
            else   
                PauseGameplay();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (_sH._gM._inScreen)
                CloseResearch();
            else
                OpenResearch();
        }
    }

    public void UpdateMoney()
    {
        _money.text = _sH._gM._money + "php";
    }

    public void UpdateMonth(string month)
    {
        _month.text = month;
    }
    public void UpdateYear(int year)
    {
        _year.text = "Year " + year;
    }

    public void UpdateSeason(bool isWet)
    {
        _season.sprite = isWet ? _wet : _dry;
        _bg.sprite = isWet? _wetBG : _dryBG;
    }

    public void UpdateBar(float current, float max, Image i, Coroutine c)
    {
        float targetFill = current / max;

        if (c != null)
            StopCoroutine(c);

        c = StartCoroutine(UpdateFill(i, targetFill));
    }

    IEnumerator UpdateFill(Image bar, float targetFillAmount)
    {
        float initialFill = bar.fillAmount;
        float elapsedTime = 0f;
        float duration = 0.8f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            bar.fillAmount = Mathf.Lerp(initialFill, targetFillAmount, elapsedTime / duration);
            yield return null;
        }

        bar.fillAmount = targetFillAmount;
    }

    public void OpenResearch()
    {
        _sH._gM._inScreen = true;
        _sH._aM.PlayMusic(Music.Research);
        _sH._aM.PlaySFX(SFX.Research);
        var _tM = TransitionManager.Instance();
        _tM.onTransitionCutPointReached += ActivateResearch;
        _tM.Transition(_transition, 0.2f);
    }

    public void CloseResearch()
    {
        _sH._aM.PlayMusic(Music.Gameplay);
        _sH._aM.PlaySFX(SFX.Back);
        var _tM = TransitionManager.Instance();
        _tM.onTransitionCutPointReached += DeactivateResearch;
        _tM.Transition(_transition, 0.2f);
    }

    void ActivateResearch()
    {
        _researchScreen.SetActive(true);

        var _tM = TransitionManager.Instance();
        _tM.onTransitionCutPointReached -= ActivateResearch;
    }

    void DeactivateResearch()
    {
        _sH._gM._inScreen = false;
        _researchScreen.SetActive(false);

        var _tM = TransitionManager.Instance();
        _tM.onTransitionCutPointReached -= DeactivateResearch;
    }

    public void PlayGameplay()
    {
        _sH._aM.PlaySFX(SFX.Rune);
        _sH._gM._isPaused = false;
        _play.gameObject.SetActive(true);
        _pause.gameObject.SetActive(false);
    }

    public void PauseGameplay()
    {
        _sH._aM.PlaySFX(SFX.Rune);
        _sH._gM._isPaused = true;
        _pause.gameObject.SetActive(true);
        _play.gameObject.SetActive(false);
    }

    public void ActivateUnit (UnitType uType, int uID)
    {
        switch (uType)
        {
            case UnitType.Firefighter:
                if (uID == 1)
                    _funit1.SetActive(true);
                if (uID == 2)
                    _funit2.SetActive(true);
                break;
            case UnitType.Ranger:
                if (uID == 1)
                    _runit1.SetActive(true);
                if (uID == 2)
                    _runit2.SetActive(true);
                break;
            case UnitType.Police:
                if (uID == 1)
                    _punit1.SetActive(true);
                if (uID == 2)
                    _punit2.SetActive(true);
                break;
        }
    }
}
