using UnityEngine;

public class Rotate : MonoBehaviour
{
    ServiceHub _sH;
    [SerializeField] private float rotationSpeed;

    void Awake()
    {
        _sH = ServiceHub.Instance;
    }

    private void Update()
    {
        if (!_sH._gM._isPaused)
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}