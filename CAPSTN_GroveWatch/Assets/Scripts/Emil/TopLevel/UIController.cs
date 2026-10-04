using System.Collections;
using System.Collections.Generic;
using EasyTransition;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    ServiceHub _sH;
    [SerializeField] TransitionSettings _transition;

    [SerializeField] Image _repBar, _progBar, _season;
    [SerializeField] TextMeshProUGUI _money, _month, _year, _repPercent, _progPercent;
    [SerializeField] Sprite _dry, _wet, _dryBG, _wetBG, _pausedBG;
    [SerializeField] SpriteRenderer _bg;
    [SerializeField] GameObject _researchScreen, _policyScreen, _pauseMenu, _pauseVolume;
    [SerializeField] Button _play, _pause, _settings;
    [SerializeField] GameObject _funit1, _funit2, _runit1, _runit2, _punit1, _punit2;
    [SerializeField] GameObject _newsBulletin;

    [SerializeField] Animator _left, _top, _bottom, _right;

    [SerializeField] RectTransform _fbn;

    bool _inPauseScreen, _inResearchScreen, _inNewsScreen;

    Coroutine _repFill, _progFill;

    public Image RepBar => _repBar;
    public Image ProgBar => _progBar;
    public Coroutine RepFill => _repFill;
    public Coroutine ProgFill => _progFill;

    void Awake()
    {
        _sH = ServiceHub.Instance;
        _sH._UI = this;
    }

    void Start()
    {
        _settings.onClick.AddListener(() => _sH._gM.OpenSettings());
        _settings.onClick.AddListener(() => _sH._aM.PlaySFX(SFX.Generic));

        UpdateBar(_sH._gM._progress, _sH._gM._maxProgress, _progBar, _progFill);
        UpdateProgPercent();
        UpdateBar(_sH._gM._reputation, _sH._gM._maxReputation, _repBar, _repFill);
        UpdateRepPercent();
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

        if (_sH._gM._inScreen && Input.GetKeyDown(KeyCode.Escape))
        {
            if (_inPauseScreen)
            {
                if (_sH._gM._inSettings)
                {
                    _sH._gM._inSettings = false;
                    _sH._aM.PlaySFX(SFX.Back);
                    SceneManager.UnloadSceneAsync("Settings");
                    return;
                }
                ClosePauseMenu();
            }
            else if (_inResearchScreen)
                CloseResearch();
            else if (_inNewsScreen)
                CloseNews();
        }


        if (_sH._gM._inScreen)
            return;

        if (TransitionManager.Instance().isBusy)
            return;
        
        if (Input.GetKeyDown(KeyCode.N))
            OpenNews();

        if (Input.GetKeyDown(KeyCode.Tab))
            OpenResearch();

        if (Input.GetKeyDown(KeyCode.Escape))
            OpenPauseMenu();
    }

    public void UpdateMoney()
    {
        _money.text = _sH._gM._money + "php";
    }

    public void SpawnFloatingMoney(int value)
    {
        var floatingMoney = new GameObject("Floating Money", typeof(RectTransform), typeof(TextMeshProUGUI), typeof(CanvasGroup));
        var rectTransform = floatingMoney.GetComponent<RectTransform>();
        var label = floatingMoney.GetComponent<TextMeshProUGUI>();
        var canvasGroup = floatingMoney.GetComponent<CanvasGroup>();

        rectTransform.SetParent(_fbn, false);
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(200f, 50f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.localScale = Vector3.one;

        label.text = value >= 0 ? $"+{value}php" : $"-{Mathf.Abs(value)}php";
        if (value == 0)
            label.text = string.Empty;
        
        label.font = _money.font;
        label.fontSharedMaterial = _money.fontSharedMaterial;
        label.fontSize = _money.fontSize;
        label.color = value >= 0 ? new Color(0.35f, 0.85f, 0.35f, 1f) : new Color(0.95f, 0.35f, 0.35f, 1f);
        label.alignment = TMPro.TextAlignmentOptions.Left;
        label.raycastTarget = false;

        canvasGroup.alpha = 1f;

        StartCoroutine(AnimateFloatingMoney(rectTransform, canvasGroup, value >= 0));
    }

    IEnumerator AnimateFloatingMoney(RectTransform target, CanvasGroup canvasGroup, bool floatUp)
    {
        float duration = 5f;
        float elapsed = 0f;
        Vector2 startPos = target.anchoredPosition;
        Vector2 endPos = startPos + (floatUp ? Vector2.up : Vector2.down) * 60f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            target.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        Destroy(target.gameObject);
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

    public void UpdateRepPercent()
    {
        float percent = _sH._gM._reputation / _sH._gM._maxReputation * 100f;
        _repPercent.text = Mathf.CeilToInt(percent) + "%";
    }

    public void UpdateProgPercent()
    {
        float percent = _sH._gM._progress / _sH._gM._maxProgress * 100f;
        _progPercent.text = Mathf.FloorToInt(percent) + "%";
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
        _inResearchScreen = true;
        _sH._gM._inScreen = true;
        _sH._aM.PlayMusic(Music.Research);
        _sH._aM.PlaySFX(SFX.Research);
        ToggleHUD(false);
        var _tM = TransitionManager.Instance();
        _tM.onTransitionCutPointReached += ActivateResearch;
        _tM.Transition(_transition, 0.1f);
    }

    public void CloseResearch()
    {
        var _tM = TransitionManager.Instance();
        if (_tM.isBusy)
            return;

        _sH._aM.PlayMusic(Music.Gameplay);
        _sH._aM.PlaySFX(SFX.Back);
        _tM.onTransitionCutPointReached += DeactivateResearch;
        _tM.Transition(_transition, 0.1f);
    }

    void ActivateResearch()
    {
        _researchScreen.SetActive(true);
        var _tM = TransitionManager.Instance();
        _tM.onTransitionCutPointReached -= ActivateResearch;
    }

    void DeactivateResearch()
    {
        _inResearchScreen = false;
        _sH._gM._inScreen = false;
        _researchScreen.SetActive(false);
        ToggleHUD(true);
        var _tM = TransitionManager.Instance();
        _tM.onTransitionCutPointReached -= DeactivateResearch;
    }

    public void PlayGameplay()
    {
        _sH._aM.PlaySFX(SFX.Rune);
        _sH._gM._isPaused = false;
        _play.gameObject.SetActive(true);
        _pause.gameObject.SetActive(false);
        _pauseVolume.SetActive(false);
        UpdateSeason(_sH._time.IsWet);
    }

    public void PauseGameplay()
    {
        _sH._aM.PlaySFX(SFX.Rune);
        _sH._gM._isPaused = true;
        _pause.gameObject.SetActive(true);
        _play.gameObject.SetActive(false);
        _pauseVolume.SetActive(true);
        _bg.sprite = _pausedBG;
    }

    void ToggleHUD(bool value)
    {
        _left.SetBool("In", value);
        _top.SetBool("In", value);
        _bottom.SetBool("In", value);
        _right.SetBool("In", value);
    }

    public void OpenPauseMenu()
    {
        _inPauseScreen = true;
        _sH._gM._inScreen = true;
        _sH._aM.PlaySFX(SFX.Generic);
        ToggleHUD(false);
        var _tM = TransitionManager.Instance();
        _tM.onTransitionCutPointReached += ActivatePause;
        _tM.Transition(_transition, 0.1f);
    }

    public void ClosePauseMenu()
    {
        var _tM = TransitionManager.Instance();
        if (_tM.isBusy)
            return;

        _sH._aM.PlaySFX(SFX.Back);    
        _tM.onTransitionCutPointReached += DeactivatePause;
        _tM.Transition(_transition, 0.1f);
    }

    void ActivatePause()
    {
        _pauseMenu.SetActive(true);
        var _tM = TransitionManager.Instance();
        _tM.onTransitionCutPointReached -= ActivatePause;
    }

    void DeactivatePause()
    {
        _inPauseScreen = false;
        _sH._gM._inScreen = false;
        _pauseMenu.SetActive(false);
        ToggleHUD(true);
        var _tM = TransitionManager.Instance();
        _tM.onTransitionCutPointReached -= DeactivatePause;
    }

    public void QuitToMenu()
    {
        var _tM = TransitionManager.Instance();
        if (_tM.isBusy)
            return;

        _sH._aM.PlaySFX(SFX.Back);
        _tM.Transition("TitleScreen", _transition, 0.2f);
    }

    public void OpenPolicyScreen()
    {
        if (_sH._gM._inScreen)
            return;
        
        var _tM = TransitionManager.Instance();
        if (_tM.isBusy)
            return;

        _sH._time._needPolicy = false;
        _sH._gM._inScreen = true;
        _sH._aM.PlayMusic(Music.Policy);
        ToggleHUD(false);
        _tM.onTransitionCutPointReached += ActivatePolicy;
        _tM.Transition(_transition, 0.1f);
    }

    public void ClosePolicyScreen()
    {
        var _tM = TransitionManager.Instance();
        if (_tM.isBusy)
            return;
     
        _sH._aM.PlayMusic(Music.Gameplay);
        _tM.onTransitionCutPointReached += DeactivatePolicy;
        _tM.Transition(_transition, 0.1f);
    }

    void ActivatePolicy()
    {
        _policyScreen.SetActive(true);
        var _tM = TransitionManager.Instance();
        _tM.onTransitionCutPointReached -= ActivatePolicy;
    }

    void DeactivatePolicy()
    {
        _sH._gM._inScreen = false;
        _policyScreen.SetActive(false);
        ToggleHUD(true);
        var _tM = TransitionManager.Instance();
        _tM.onTransitionCutPointReached -= DeactivatePolicy;
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

    public void OpenNews()
    {
        _inNewsScreen = true;
        _sH._gM._inScreen = true;
        _sH._aM.PlaySFX(SFX.Generic);
        ToggleHUD(false);
        var _tM = TransitionManager.Instance();
        _tM.onTransitionCutPointReached += ActivateNews;
        _tM.Transition(_transition, 0.1f);
    }

    public void CloseNews()
    {
        var _tM = TransitionManager.Instance();
        if (_tM.isBusy)
            return;

        _sH._aM.PlaySFX(SFX.Back);    
        _tM.onTransitionCutPointReached += DeactivateNews;
        _tM.Transition(_transition, 0.1f);
    }

    void ActivateNews()
    {
        _newsBulletin.SetActive(true);
        var _tM = TransitionManager.Instance();
        _tM.onTransitionCutPointReached -= ActivateNews;
    }

    void DeactivateNews()
    {
        _inNewsScreen = false;
        _sH._gM._inScreen = false;
        _newsBulletin.SetActive(false);
        ToggleHUD(true);
        var _tM = TransitionManager.Instance();
        _tM.onTransitionCutPointReached -= DeactivateNews;
    }
}
