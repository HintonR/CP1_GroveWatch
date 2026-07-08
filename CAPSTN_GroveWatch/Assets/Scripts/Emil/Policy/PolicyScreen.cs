using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PolicyScreen : MonoBehaviour
{
    ServiceHub _sH;
    
    [SerializeField] TextMeshProUGUI _prompt;

    [SerializeField] List<PolicyDataSO> _policies;

    [SerializeField] PolicyChoice _p1, _p2, _p3;

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
        StartCoroutine(CardsAnim(true));
    }

    void OnDisable()
    {
            DeactivateAnim(_p1);
            DeactivateAnim(_p2);
            DeactivateAnim(_p3);
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

    void UpdateChoices(int pIndex)
    {
        var policy = _policies[pIndex];

        UpdateChoice(_p1, policy.Choice1);
        UpdateChoice(_p2, policy.Choice2);
        UpdateChoice(_p3, policy.Choice3);
    }

    private void UpdateChoice(PolicyChoice choice, PolicyChoiceDataSO data)
    {
        choice.Title.text = data.Title;

        choice.Rep.text = data.Rep == 0
            ? string.Empty
            : FormatValue(data.Rep, "%");

        choice.Budget.text = data.Budget == 0
            ? string.Empty
            : FormatValue(data.Budget, " php");

        choice.CD.text = data.GetSign(BonusType.CD);
        choice.EF.text = data.GetSign(BonusType.EF);

        choice.Select.onClick.RemoveAllListeners();
        choice.Select.onClick.AddListener(() => _sH._aM.PlaySFX(SFX.Rune));
        choice.Select.onClick.AddListener(() => _sH._UI.ClosePolicyScreen());
        choice.Select.onClick.AddListener(data.ApplyChoice);
    }

    private string FormatValue(int value, string suffix)
    {
        return $"{(value > 0 ? "+" : "")}{value}{suffix}";
    }

    IEnumerator CardsAnim(bool value)
    {
        if (value)
        {
            _p1.gameObject.SetActive(value);
            ActivateAnim(_p1);
            yield return new WaitForSeconds(0.4f);
            _p2.gameObject.SetActive(value);
            ActivateAnim(_p2);
            yield return new WaitForSeconds(0.4f);
            _p3.gameObject.SetActive(value);
            ActivateAnim(_p3);
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
