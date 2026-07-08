using DG.Tweening;
using UnityEngine;

public class BobbleEffect : MonoBehaviour
{
    [Header("Float")]
    [SerializeField] float floatHeight = 0.15f;
    [SerializeField] float floatDuration = 1f;

    [Header("Wobble")]
    [SerializeField] float rotationAngle = 5f;
    [SerializeField] float rotationDuration = 0.5f;

    Sequence _sequence;
    bool _wasPaused;

    ServiceHub _sH;
    
    void Awake()
    {
        _sH = ServiceHub.Instance;
    }

    void OnEnable()
    {
        transform.localRotation = Quaternion.identity;

        _sequence = DOTween.Sequence();

        _sequence.Join(
            transform.DOLocalMoveY(floatHeight, floatDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(2, LoopType.Yoyo));

        _sequence.Join(
            transform.DOLocalRotate(
                new Vector3(0, 0, rotationAngle),
                rotationDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(2, LoopType.Yoyo));

        _sequence.SetLoops(-1);
        _sequence.Play();
    }

    private void Update()
    {
        if (_sH._gM._isPaused == _wasPaused)
            return;

        _wasPaused = _sH._gM._isPaused;

        if (_wasPaused)
            _sequence.Pause();
        else
            _sequence.Play();
    }

    private void OnDisable()
    {
        _sequence?.Kill();
    }
}
