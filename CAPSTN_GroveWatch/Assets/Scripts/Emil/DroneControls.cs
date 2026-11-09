using UnityEngine;

public class DroneControls : MonoBehaviour
{
    [SerializeField] Camera _camera;
    [SerializeField] float _moveSpeed = 10f;
    [SerializeField] BoxCollider _bounds;

    [SerializeField] float _edgeSize = 30f;
    [SerializeField] float _dragSensitivity = 1.5f;

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        AddEdgeScroll(ref x, ref z);

        if (Input.GetMouseButton(1))
            AddRightClickDrag(ref x, ref z);

        Vector3 forward = _camera.transform.forward;
        Vector3 right = _camera.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 move = (forward * z + right * x).normalized;

        transform.position += move * _moveSpeed * Time.deltaTime;
    }

    void LateUpdate()
    {
        Bounds bounds = _bounds.bounds;
        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, bounds.min.x, bounds.max.x);
        pos.z = Mathf.Clamp(pos.z, bounds.min.z, bounds.max.z);

        transform.position = pos;
    }

    void AddEdgeScroll(ref float x, ref float z)
    {
        var gM = ServiceHub.Instance._gM;
        if (gM._isPaused || gM._inScreen || !Application.isFocused || Input.GetMouseButton(1)) return;

        Vector3 mouse = Input.mousePosition;
        if (mouse.x < 0 || mouse.y < 0 || mouse.x > Screen.width || mouse.y > Screen.height) return;

        if (mouse.x <= _edgeSize) x -= 1f;
        else if (mouse.x >= Screen.width - _edgeSize) x += 1f;

        if (mouse.y <= _edgeSize) z -= 1f;
        else if (mouse.y >= Screen.height - _edgeSize) z += 1f;
    }

    void AddRightClickDrag(ref float x, ref float z)
    {
        if (ServiceHub.Instance._gM._isPaused || ServiceHub.Instance._gM._inScreen || !Application.isFocused)
            return;

        x -= Input.GetAxis("Mouse X") * _dragSensitivity;
        z -= Input.GetAxis("Mouse Y") * _dragSensitivity;
    }
}