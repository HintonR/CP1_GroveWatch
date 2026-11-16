using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;

public class CreditsScroller : MonoBehaviour
{
    [SerializeField] RectTransform _credits;
    [SerializeField] float _duration = 50f;
    [SerializeField] float _endEarly = 0f;
    [SerializeField] string _titleScene = "TitleScreen";
    [SerializeField] CanvasGroup _thanks;
    [SerializeField] float _fadeTime = 1.5f;
    [SerializeField] float _holdTime = 3f;
    Sequence _seq;

    void Start()
    {
        float h = _credits.rect.height * _credits.localScale.y;
        float screenH = ((RectTransform)_credits.parent).rect.height;
        float endY = screenH + h / 2f - _endEarly;
        _thanks.alpha = 0f;

        _seq = DOTween.Sequence();
        _seq.Append(_credits.DOAnchorPosY(endY, _duration).SetEase(Ease.Linear));

        _seq.Append(_thanks.DOFade(1f, _fadeTime));
        _seq.AppendInterval(_holdTime);
        _seq.Append(_thanks.DOFade(0f, _fadeTime));

        _seq.OnComplete(() => SceneManager.LoadScene(_titleScene));
    }

    IEnumerator LoadTitleRoutine()
    {
        yield return new WaitForSeconds(_duration / 2);
        SceneManager.LoadScene(_titleScene);
    }

    void OnDestroy()
    {
        _seq?.Kill();
    }
}