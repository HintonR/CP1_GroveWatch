using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Shake : MonoBehaviour
{
    Vector3 _originalLocalPosition;

    void Awake()
    {
        _originalLocalPosition = transform.localPosition;
    }

    public void DoShake()
    {
        transform.DOKill();

        // Ensure Starting Point
        transform.localPosition = _originalLocalPosition;

        transform.DOPunchPosition(
            Vector3.right * 5f,
            0.35f,
            12,
            1f
        ).SetLink(gameObject);
    }

    void OnDestroy()
    {
        transform.DOKill();
    }
}
