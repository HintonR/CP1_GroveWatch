using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;

public class CreditsScroller : MonoBehaviour
{
    [SerializeField] RectTransform _credits;
    [SerializeField] float _duration = 30f;
    [SerializeField] string _titleScene = "TitleScreen";

    void Start()
    {
        _credits.DOAnchorPosY(_credits.anchoredPosition.y + _credits.rect.height, _duration)
            .SetEase(Ease.Linear);
        
        StartCoroutine(LoadTitleRoutine());
    }

    IEnumerator LoadTitleRoutine()
    {
        yield return new WaitForSeconds(_duration / 2);
        SceneManager.LoadScene(_titleScene);
    }

    void OnDestroy()
    {
        _credits.DOKill();
    }
}