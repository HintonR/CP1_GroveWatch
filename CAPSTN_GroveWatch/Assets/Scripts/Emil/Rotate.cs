using UnityEngine;

public class Rotate : MonoBehaviour
{
    ServiceHub _sH;
    [SerializeField] float rotationSpeed;

    void Awake()
    {
        _sH = ServiceHub.Instance;
    }

    private void Update()
    {
        if (!_sH._gM._isPaused && !_sH._gM._inScreen)
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}