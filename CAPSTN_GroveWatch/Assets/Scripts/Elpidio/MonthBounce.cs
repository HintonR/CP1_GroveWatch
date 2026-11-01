using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class MonthBounce : MonoBehaviour
{
    [SerializeField] private float height = 12f;
    [SerializeField] private float duration = 0.4f;
    TMP_Text text;
    RectTransform rect;
    Vector2 home;
    string last;
    void Awake()
    {
        text = GetComponent<TMP_Text>();
        rect = (RectTransform)transform;
        home = rect.anchoredPosition;
    }
    void LateUpdate()
    {
        if (text.text == last) return;

        bool first = last == null;
        last = text.text;
        if (first) return;

        rect.DOKill();
        rect.anchoredPosition = home;
        rect.DOJumpAnchorPos(home, height, 1, duration);
    }
    void OnDestroy()
    {
        rect.DOKill();
    }
}
