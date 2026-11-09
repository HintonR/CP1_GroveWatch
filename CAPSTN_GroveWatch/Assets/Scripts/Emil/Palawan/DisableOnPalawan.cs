using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisableOnPalawan : MonoBehaviour
{
    void Awake()
    {
        gameObject.SetActive(SceneManager.GetActiveScene().name != "Palawan");
    }
}
