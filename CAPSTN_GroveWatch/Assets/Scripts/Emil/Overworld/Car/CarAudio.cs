using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarAudio : MonoBehaviour
{
    ServiceHub _sH;

    [Header("References")]
    [SerializeField] AudioSource engineSource;
    [SerializeField] AudioSource skidSource;
    [SerializeField] AudioSource sirenSource;

    [Header("Engine")]
    [SerializeField] float maxSpeed = 15f;

    [SerializeField] float idlePitch = 0.8f;
    [SerializeField] float maxPitch = 1.6f;

    [SerializeField] float idleVolume = 0.35f;
    [SerializeField] float maxVolume = 0.6f;

    [SerializeField] float revUpSpeed = 3f;
    [SerializeField] float revDownSpeed = 2f;

    Rigidbody rb;

    float currentRPM;
    float skidVolume;
    float sirenVolume;
    bool sirenOn;

    float CurrentSFXVolume => _sH._aM != null
        ? _sH._aM.MasterVolume * _sH._aM.SFXVolume
        : 1f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _sH = ServiceHub.Instance;

        engineSource.loop = true;
        skidSource.loop = true;

        skidVolume = skidSource.volume;
        sirenVolume = sirenSource.volume;
    }

    void Start()
    {
        engineSource.Play();
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (ServiceHub.Instance._dUI._inScreen)
            return;
            
            sirenOn = !sirenOn;

            if (sirenOn)
                sirenSource.Play();
            else
                sirenSource.Stop();
        }

        UpdateEngine();
        UpdateSkid();
        UpdateSiren();
    }

    void UpdateEngine()
    {
        float speedPercent = Mathf.Clamp01(rb.velocity.magnitude / maxSpeed);
        float throttle = Mathf.Max(0f, Input.GetAxis("Vertical"));
        float targetRPM = Mathf.Max(speedPercent, throttle);
        float response =
            targetRPM > currentRPM
            ? revUpSpeed
            : revDownSpeed;

        currentRPM = Mathf.MoveTowards(
            currentRPM,
            targetRPM,
            response * Time.deltaTime);

        engineSource.pitch = Mathf.Lerp(
            idlePitch,
            maxPitch,
            currentRPM);

        float engineVolume = Mathf.Lerp(
            idleVolume,
            maxVolume,
            currentRPM);

        engineSource.volume = engineVolume * CurrentSFXVolume;
    }

    void UpdateSkid()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);

        bool skidding =
        Input.GetKey(KeyCode.Space) &&
        localVelocity.z > 2f;

        if (skidding)
        {
            if (!skidSource.isPlaying)
                skidSource.Play();

            skidVolume = Mathf.MoveTowards(
                skidVolume,
                maxVolume,
                4f * Time.deltaTime);

            skidSource.volume = skidVolume * CurrentSFXVolume;

            skidSource.pitch = Mathf.Lerp(
                0.9f,
                1.2f,
                rb.velocity.magnitude / maxSpeed);
        }
        else
        {
            skidVolume = Mathf.MoveTowards(
                skidVolume,
                0f,
                6f * Time.deltaTime);

            skidSource.volume = skidVolume * CurrentSFXVolume;

            if (skidVolume <= 0.01f && skidSource.isPlaying)
                skidSource.Stop();
        }
    }

    void UpdateSiren()
    {
        float targetVolume = sirenOn ? maxVolume : 0f;

        sirenVolume = Mathf.MoveTowards(
            sirenVolume,
            targetVolume / 2,
            2f * Time.deltaTime);

        sirenSource.volume = sirenVolume * CurrentSFXVolume;

        if (sirenOn)
        {
            if (!sirenSource.isPlaying)
                sirenSource.Play();
        }
        else
        {
            if (sirenVolume <= 0.01f && sirenSource.isPlaying)
                sirenSource.Stop();
        }
    }



}
