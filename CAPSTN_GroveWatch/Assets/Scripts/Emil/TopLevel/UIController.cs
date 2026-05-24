using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    ServiceHub _sH;

    [SerializeField] Image _repBar, _progBar, _season;
    [SerializeField] TextMeshProUGUI _money, _month, _year;
    [SerializeField] Sprite _dry, _wet;

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
}
