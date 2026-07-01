using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonPressScale : MonoBehaviour, IPointerDownHandler, IPointerUpHandler //needed for onPointerStuff
{
    private Vector3 originalScale;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOScale(originalScale * 0.9f, 0.1f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOScale(originalScale, 0.1f);
    }
}