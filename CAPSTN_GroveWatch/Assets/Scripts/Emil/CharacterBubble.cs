using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterBubble : MonoBehaviour
{
    const float TEXT_SPEED = 80f;
   [SerializeField] string _name;
   [SerializeField, TextArea(2,2)] string _text;
   [SerializeField] TextMeshProUGUI _nameUI, _textUI;
   [SerializeField] GameObject _balloon;

   Coroutine _speechRoutine;

   void Start()
    {
        _nameUI.text = _name;
    }

    public void TurnOnBalloon()
    {
        _balloon.SetActive(true);
        StartTypewriter(_textUI, _text, TEXT_SPEED, ref _speechRoutine);
    }

    public void TurnOffBalloon()
    {
        StopAllCoroutines();
        _balloon.SetActive(false);
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
                if (charsSinceBlip >= 6) 
                {
                    ServiceHub.Instance._aM.PlaySFX(SFX.Text);
                    charsSinceBlip = 0;
                }
            }

            yield return null;
        }

        textComponent.maxVisibleCharacters = total;
    }


}
