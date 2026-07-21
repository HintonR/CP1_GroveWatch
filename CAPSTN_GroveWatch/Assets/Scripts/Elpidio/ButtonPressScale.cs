using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonPressScale : MonoBehaviour, IPointerDownHandler, IPointerUpHandler //needed for onPointerStuff
{
    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(originalScale * 0.9f, 0.08f)
            .SetEase(Ease.OutQuad);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(originalScale, 0.2f)
            .SetEase(Ease.OutBack);
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}