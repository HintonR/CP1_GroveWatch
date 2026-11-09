using UnityEngine;
using DG.Tweening;

public class Pulse : MonoBehaviour
{
    [SerializeField] float _scale = 1.15f;
    [SerializeField] float _duration = 0.5f;

    void Start()
    {
        transform.DOScale(_scale, _duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    void OnDestroy()
    {
        transform.DOKill();
    }
}