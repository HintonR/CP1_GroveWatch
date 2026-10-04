using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    ServiceHub _sH;

    [SerializeField] float _dayLength = 24f;
    [SerializeField] float _rSpeed = 30f;

    float _time;

    void Awake()
    {
        _sH = ServiceHub.Instance;
    }

    void Update()
    {
        if (_sH._gM._isPaused )
            return;

        _time += Time.deltaTime;

        float yRotation = _time * _rSpeed;
        transform.rotation = Quaternion.Euler(60, yRotation, 0f);

        if (_time >= _dayLength)
            _time = 0f;
    }
}