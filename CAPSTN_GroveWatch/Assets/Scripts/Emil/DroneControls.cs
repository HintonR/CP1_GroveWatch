using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneControls : MonoBehaviour
{
    [SerializeField] Camera _camera;
    [SerializeField] float _moveSpeed = 10f;
    [SerializeField] BoxCollider _bounds;

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

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
}
