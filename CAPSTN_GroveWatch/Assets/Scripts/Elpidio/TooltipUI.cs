using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TooltipUI : MonoBehaviour
{
    //put this on an empty GameObject in Canvas topmost
    [SerializeField] private RectTransform box;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float offset = 16f;

    void Awake()
    {
        ServiceHub.Instance._tUI = this;
        box.gameObject.SetActive(false);
        foreach (var g in box.GetComponentsInChildren<Graphic>(true)) g.raycastTarget = false;
    }
    public void Show(string description)
    {
        text.text = description;
        box.gameObject.SetActive(true);
        Follow();
    }
    public void Hide()
    {
        box.gameObject.SetActive(false);
    }
    void LateUpdate()
    {
        if (box.gameObject.activeSelf) Follow();
    }
    void Follow()
    {
        Vector2 mouse = Input.mousePosition;

        //math to flip near screen edges so tooltip stays on screen
        float px = mouse.x > Screen.width * 0.7f ? 1f : 0f;
        float py = mouse.y < Screen.height * 0.3f ? 0f : 1f;

        box.pivot = new Vector2(px, py);
        box.position = mouse + new Vector2(px == 0f ? offset : -offset, py == 1f ? -offset : offset);
    }
}
