using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField, TextArea] private string description;
    [SerializeField] private float delay = 0.5f;

    bool hovered;
    Coroutine waiting;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Hide();
        hovered = true;
        waiting = StartCoroutine(ShowAfterDelay());
    }
    public void OnPointerExit(PointerEventData eventData) { Hide(); }
    void OnDisable() { Hide(); }
    IEnumerator ShowAfterDelay()
    {
        yield return new WaitForSecondsRealtime(delay); //realtime so it still works while paused

        var tooltip = ServiceHub.Instance._tUI;
        if (tooltip != null) tooltip.Show(description); //no HUD in this scene: do nothing
    }

    void Hide()
    {
        if (!hovered) return; //only the hovered trigger may hide the box
        hovered = false;

        if (waiting != null) StopCoroutine(waiting);
        waiting = null;
        var hub = ServiceHub.Instance;
        if (hub != null && hub._tUI != null) hub._tUI.Hide();
    }
}
