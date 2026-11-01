using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonPressScale : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler //needed for onPointerStuff
{
    [SerializeField] private float hoverScale = 1.08f;
    private Vector3 originalScale;
    private Selectable selectable;
    private bool hovered;

    private void Awake()
    {
        originalScale = transform.localScale;
        selectable = GetComponent<Selectable>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectable != null && !selectable.IsInteractable()) return;

        hovered = true;
        ScaleTo(originalScale * hoverScale, 0.12f, Ease.OutQuad);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
        ScaleTo(originalScale, 0.12f, Ease.OutQuad);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ScaleTo(originalScale * 0.9f, 0.08f, Ease.OutQuad);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        ScaleTo(hovered ? originalScale * hoverScale : originalScale, 0.2f, Ease.OutBack);
    }
    private void ScaleTo(Vector3 target, float duration, Ease ease)
    {
        transform.DOKill();
        transform.DOScale(target, duration).SetEase(ease);
    }

    private void OnDisable()
    {
        hovered = false;
        transform.DOKill();
        transform.localScale = originalScale;
    }
    private void OnDestroy()
    {
        transform.DOKill();
    }
}