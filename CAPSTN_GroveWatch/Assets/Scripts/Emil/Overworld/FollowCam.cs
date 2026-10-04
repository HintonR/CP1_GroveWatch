using UnityEngine;

public class FollowCam : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Header("Position")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -8f);
    [SerializeField] private float followSpeed = 5f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 5f;

    private void LateUpdate()
    {
        if (target == null)
            return;

        // Desired position relative to the truck
        Vector3 desiredPosition = target.TransformPoint(offset);

        // Smoothly move the camera
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime);

        // Smoothly look at the truck
        Quaternion desiredRotation = Quaternion.LookRotation(
            target.position - transform.position,
            Vector3.up);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRotation,
            rotationSpeed * Time.deltaTime);
    }
}