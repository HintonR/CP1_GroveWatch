using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PolicyScreen : MonoBehaviour
{
    ServiceHub _sH;
    
    [SerializeField] TextMeshProUGUI _prompt, _rep, _bud;
    [SerializeField] List<PolicyDataSO> _policies;
    [SerializeField] PolicyChoice _p1, _p2, _p3;
    [SerializeField] Sprite _g1, _g2, _r1, _r2;
    [SerializeField] Sprite _smile, _frown;
    [SerializeField] Animator _info;

    Coroutine _promptRoutine;
    
    void Awake()
    {
        _sH = ServiceHub.Instance;
    }

    void OnEnable()
    {
        int pIndex = GetRandomPromptIndex();

        UpdatePrompt(pIndex);
        UpdateChoices(pIndex); 
        UpdateInfo();
        StartCoroutine(PolicyAnim(true));
    }

    void OnDisable()
    {
            DeactivateAnim(_p1);
            DeactivateAnim(_p2);
            DeactivateAnim(_p3);
            _info.SetBool("In", false);
            _p1.Select.gameObject.SetActive(false);
            _p2.Select.gameObject.SetActive(false);
            _p3.Select.gameObject.SetActive(false);
            _p1.gameObject.SetActive(false);
            _p2.gameObject.SetActive(false);
            _p3.gameObject.SetActive(false);
    }

    int GetRandomPromptIndex()
    {
        return Random.Range(0, _policies.Count);
    }

    void UpdatePrompt(int pIndex)
    {
        var textSpeed = 40f;
        StartTypewriter(_prompt, _policies[pIndex].Prompt, textSpeed, ref _promptRoutine);

        _sH._nM._policy = _policies[pIndex].Prompt;
    }

    void StartTypewriter(TextMeshProUGUI textComponent, string content, float speed, ref Coroutine routine)
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(TypewriterRoutine(textComponent, content, speed));
    }

    IEnumerator TypewriterRoutine(TextMeshProUGUI textComponent, string fullText, float speed)
    {
        textComponent.text = fullText;
        textComponent.maxVisibleCharacters = 0;

        int total = fullText.Length;
        float interval = 1f / speed;
        float timer = 0f;
        int visible = 0;
        int charsSinceBlip = 0;

        while (visible < total)
        {
            timer += Time.deltaTime;

            while (timer >= interval && visible < total)
            {
                timer -= interval;
                visible++;
                textComponent.maxVisibleCharacters = visible;
                charsSinceBlip++;
                if (charsSinceBlip >= 3) //spaced by 3 chars because the sfx murders your ears if it plays on each text
                {
                    _sH._aM.PlaySFX(SFX.Text);
                    charsSinceBlip = 0;
                }
            }

            yield return null;
        }

        textComponent.maxVisibleCharacters = total;
    }

    void UpdateInfo()
    {
        _rep.text = (_sH._gM._reputation / _sH._gM._maxReputation * 100).ToString("F0") + "%";
        _bud.text = _sH._gM._money + "php";
    }

    void UpdateChoices(int pIndex)
    {
        var policy = _policies[pIndex];

        UpdateChoice(_p1, policy.Choice1);
        UpdateChoice(_p2, policy.Choice2);
        UpdateChoice(_p3, policy.Choice3);
    }

    void UpdateChoice(PolicyChoice choice, PolicyChoiceDataSO data)
    {
        choice.Title.text = data.Title;

        choice.Rep.text = data.Rep == 0
            ? string.Empty
            : FormatValue(data.Rep, "%");

        choice.Budget.text = data.Budget == 0
            ? string.Empty
            : FormatValue(data.Budget, " php");

        choice.CDIcon.gameObject.SetActive(true);
        var cdValue = data.GetModifierValue(BonusType.CD);
        if (cdValue < 1f && cdValue >= News.UNIT_FLOOR)
            choice.CDIcon.sprite = _g1;
        if (cdValue < News.UNIT_FLOOR)
            choice.CDIcon.sprite = _g2;
        if (cdValue > 1f && cdValue <= News.UNIT_CEILING)
            choice.CDIcon.sprite = _r1;
        if (cdValue > News.UNIT_CEILING)
            choice.CDIcon.sprite = _r2;
        if (cdValue == 1f)
            choice.CDIcon.gameObject.SetActive(false);

        choice.EFIcon.gameObject.SetActive(true);
        var efValue = data.GetModifierValue(BonusType.EF);
        if (efValue < 1f && efValue >= News.UNIT_FLOOR)
            choice.EFIcon.sprite = _r1;
        if (efValue < News.UNIT_FLOOR)
            choice.EFIcon.sprite = _r2;
        if (efValue > 1f && efValue <= News.UNIT_CEILING)
            choice.EFIcon.sprite = _g1;
        if (efValue > News.UNIT_CEILING)
            choice.EFIcon.sprite = _g2;
        if (efValue == 1f)
            choice.EFIcon.gameObject.SetActive(false);

        DrawFactionStatus(choice, data, Faction.Tourist);
        DrawFactionStatus(choice, data, Faction.Citizen);
        DrawFactionStatus(choice, data, Faction.Government);

        choice.Select.onClick.RemoveAllListeners();
        choice.Select.onClick.AddListener(() => _sH._aM.PlaySFX(SFX.Rune));
        choice.Select.onClick.AddListener(() => _sH._UI.ClosePolicyScreen());
        choice.Select.onClick.AddListener(() => UpdateInfo());
        choice.Select.onClick.AddListener(data.ApplyChoice);
    }

    void DrawFactionStatus(PolicyChoice choice, PolicyChoiceDataSO data, Faction faction)
    {
        var img = choice.F1;
        if (faction == Faction.Tourist)
            img = choice.F1;
        if (faction == Faction.Citizen)
            img = choice.F2;
        if (faction == Faction.Government)
            img = choice.F3;

        img.gameObject.SetActive(true);
        var value = data.GetFactionValue(faction);
        if (value > 0)
            choice.F1.sprite = _smile;
        if (value < 0)
            choice.F1.sprite = _frown;
        if (value == 0)
            img.gameObject.SetActive(false);
    }

    string FormatValue(int value, string suffix)
    {
        return $"{(value > 0 ? "+" : "")}{value}{suffix}";
    }

    IEnumerator PolicyAnim(bool value)
    {
        if (value)
        {
            yield return new WaitForSeconds(0.3f);
            _p1.gameObject.SetActive(value);
            ActivateAnim(_p1);
            yield return new WaitForSeconds(0.3f);
            _p2.gameObject.SetActive(value);
            ActivateAnim(_p2);
            yield return new WaitForSeconds(0.3f);
            _p3.gameObject.SetActive(value);
            ActivateAnim(_p3);
            yield return new WaitForSeconds(0.2f);
            _p1.Select.gameObject.SetActive(true);
            _p2.Select.gameObject.SetActive(true);
            _p3.Select.gameObject.SetActive(true);
            _info.SetBool("In", true);
        }  
    }

    void ActivateAnim(PolicyChoice p)
    {
        p.gameObject.GetComponent<Animator>().SetBool("In", true);
    }

    void DeactivateAnim(PolicyChoice p)
    {
        p.gameObject.GetComponent<Animator>().SetBool("In", false);
    }
}
