using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsController : MonoBehaviour
{
    [SerializeField] List<CharacterBubble> _characters;

    Coroutine _balloonRoutine;
    public void ShowBalloon(int i)
    {
        foreach (CharacterBubble c in _characters)
            c.TurnOffBalloon();

        if (_balloonRoutine != null)
            StopCoroutine(_balloonRoutine);
        
        _characters[i].TurnOnBalloon();

        _balloonRoutine = StartCoroutine(TurnOffSelf(i));
    }

    IEnumerator TurnOffSelf(int i)
    {
        yield return new WaitForSeconds(6f);
        _characters[i].TurnOffBalloon();
    }


}
