using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UVScroll : MonoBehaviour
{
    [SerializeField] RawImage _img;
    [SerializeField] float _x, _y;
    void Update()
    {
        Vector2 newPos = _img.uvRect.position + new Vector2(_x, _y) * Time.deltaTime;

        newPos.x = Mathf.Repeat(newPos.x, 1f);
        newPos.y = Mathf.Repeat(newPos.y, 1f);

        _img.uvRect = new Rect(newPos, _img.uvRect.size);
    }
}

