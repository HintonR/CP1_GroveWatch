using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogManager : MonoBehaviour
{
    [SerializeField] GameObject _parent;
    [SerializeField] float _interval = 30f;

    Camera _mainCamera;
    ParticleSystem[] _fogFX;

    void Start()
    {
        _mainCamera = Camera.main;
        _fogFX = _parent.GetComponentsInChildren<ParticleSystem>(true);
        StartCoroutine(FogRoutine());
    }

    IEnumerator FogRoutine()
    {
    while (true)
    {
        float elapsed = 0f;

        while (elapsed < _interval)
        {
            if (!ServiceHub.Instance._gM._isPaused)
                elapsed += Time.deltaTime;

            yield return null;
        }

        bool activate = Random.Range(0, 3) == 0;

        foreach (ParticleSystem fx in _fogFX)
        {
            if (activate) fx.Play();
            else fx.Stop();
        }

        ServiceHub.Instance._gM._hasFog = activate;
        _mainCamera.fieldOfView = activate ? 45f : 55f;
    }
    }
}
