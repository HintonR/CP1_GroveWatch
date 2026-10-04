using UnityEngine;

public class CarBody : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;

    [Header("Body Roll")]
    [SerializeField] private float maxRoll = 8f;
    [SerializeField] private float rollSpeed = 8f;

    [Header("Acceleration Pitch")]
    [SerializeField] private float maxPitch = 5f;
    [SerializeField] private float pitchSpeed = 8f;

    float currentRoll;
    float currentPitch;

    void Update()
    {
        if (ServiceHub.Instance._dUI._inScreen)
            return;
        
        float steer = Input.GetAxis("Horizontal");
        float throttle = Input.GetAxis("Vertical");

        float targetRoll = steer * maxRoll;
        float targetPitch = -throttle * maxPitch;

        currentRoll = Mathf.Lerp(currentRoll, targetRoll, rollSpeed * Time.deltaTime);
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, pitchSpeed * Time.deltaTime);

        transform.localRotation = Quaternion.Euler(
            currentPitch,
            0f,
            currentRoll);
    }
}