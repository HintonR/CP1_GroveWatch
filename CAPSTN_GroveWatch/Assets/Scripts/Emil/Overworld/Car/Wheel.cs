using UnityEngine;

public class Wheel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Rigidbody rb;

    [Header("Spin")]
    [SerializeField] float spinMultiplier = 40f;

    protected float currentSteer;
    protected float currentSpin;

    protected virtual bool CanBurnout => false;

    protected virtual void Update()
    {
        float forwardSpeed = Vector3.Dot(rb.velocity, rb.transform.forward);

        bool burnout =
            CanBurnout &&
            Input.GetKey(KeyCode.Space) &&
            Input.GetAxis("Vertical") > 0.1f;

        if (burnout)
            currentSpin += 1440f * Time.deltaTime;
        else
            currentSpin += forwardSpeed * spinMultiplier * Time.deltaTime;

        transform.localRotation = Quaternion.Euler(
            currentSpin,
            currentSteer,
            0f);
    }
}