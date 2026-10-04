using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Driving : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float acceleration = 12f;
    [SerializeField] float maxSpeed = 15f;
    [SerializeField] float reverseSpeed = 6f;

    [Header("Steering")]
    [SerializeField] float turnSpeed = 2.5f;
    [SerializeField] float turnThreshold = 3f;
    [SerializeField] float steeringAcceleration = 12f;
    [SerializeField] float steeringDeceleration = 4f;

    [Header("Drag")]
    [SerializeField] float drag = 2f;
    [SerializeField] float handbrakeDrag = 8f;

    [SerializeField] ParticleSystem _smoke1, _smoke2;

    Rigidbody rb;

    float moveInput;
    float turnInput;
    bool handbrake;

    Coroutine _smokeRoutine;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.centerOfMass = new Vector3(0f, -0.5f, 0f);
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        moveInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");
        handbrake = Input.GetKey(KeyCode.Space);
    }

    void FixedUpdate()
    {
        if (ServiceHub.Instance._dUI._inScreen)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }
        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);

        // Steering
        float targetAngularY = 0f;

        if (Mathf.Abs(localVelocity.z) > turnThreshold)
        {
            float steer = turnInput;

            if (localVelocity.z < 0f)
                steer *= -1f;

            targetAngularY = steer * turnSpeed;
        }

        float smooth = Mathf.Abs(targetAngularY) > Mathf.Abs(rb.angularVelocity.y)
            ? steeringAcceleration
            : steeringDeceleration;

        Vector3 angular = rb.angularVelocity;

        angular.y = Mathf.Lerp(
            angular.y,
            targetAngularY,
            smooth * Time.fixedDeltaTime);

        rb.angularVelocity = angular;

        // Accelerate
        if (moveInput > 0f && localVelocity.z < maxSpeed)
            rb.AddForce(transform.forward * acceleration, ForceMode.Acceleration);

        // Reverse
        if (moveInput < 0f && localVelocity.z > -reverseSpeed)
            rb.AddForce(transform.forward * moveInput * acceleration, ForceMode.Acceleration);

        // Brake / Handbrake

        if (handbrake)
            rb.velocity *= 0.9f;

        if (handbrake && moveInput > 0f)
        {
            if (!_smoke1.isPlaying)
            {
                _smoke1.Play();
                _smoke2.Play();
            }
        }
        else
        {
            _smoke1.Stop();
            _smoke2.Stop();
        }
        
        rb.drag = handbrake ? handbrakeDrag : drag;
    }

  
}