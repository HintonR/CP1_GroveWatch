using UnityEngine;

public class CloudRotation : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed;

    void Update()
    {
        // Rotate the object around its Y-axis
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}