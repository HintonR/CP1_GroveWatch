using UnityEngine;
using DG.Tweening;

public class BGScroll : MonoBehaviour
{
    [SerializeField] float _panDistance = 200f;
    [SerializeField] float _duration = 15f;

    RectTransform _rect;
    Vector2 _originalPos;

    void Start()
    {
        _rect = GetComponent<RectTransform>();
        _originalPos = _rect.anchoredPosition;

        _rect.DOAnchorPosX(_originalPos.x - _panDistance, _duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    void OnDestroy()
    {
        _rect.DOKill();
    }
}