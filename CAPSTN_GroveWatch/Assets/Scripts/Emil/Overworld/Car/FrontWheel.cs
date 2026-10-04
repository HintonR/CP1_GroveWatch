using UnityEngine;

public class FrontWheel : Wheel
{
    
    [Header("Steering")]
    [SerializeField] float maxSteerAngle = 30f;
    [SerializeField] float steerSpeed = 8f;

    protected override void Update()
    {
        if (ServiceHub.Instance._dUI._inScreen)
            return;
            
        float turn = Input.GetAxis("Horizontal");

        currentSteer = Mathf.Lerp(
            currentSteer,
            turn * maxSteerAngle,
            steerSpeed * Time.deltaTime);

        base.Update();
    }
}