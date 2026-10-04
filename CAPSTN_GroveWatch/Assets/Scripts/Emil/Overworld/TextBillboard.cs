using UnityEngine;
using TMPro;

public class TextBillboard : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Camera targetCamera;
    [SerializeField] Transform player;
    [SerializeField] TextMeshProUGUI distanceText;

    void LateUpdate()
    {
        // Face the camera
        transform.forward = targetCamera.transform.forward;

        // Update distance
        float distance = Vector3.Distance(player.position, transform.position);
        distanceText.text = $"{Mathf.RoundToInt(distance)}m";

    }
}