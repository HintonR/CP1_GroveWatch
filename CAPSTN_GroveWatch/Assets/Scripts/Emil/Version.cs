using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Version : MonoBehaviour
{
    
    void Awake()
    {
        TextMeshProUGUI version = GetComponent<TextMeshProUGUI>();
        version.text = Application.version;
    }
}
