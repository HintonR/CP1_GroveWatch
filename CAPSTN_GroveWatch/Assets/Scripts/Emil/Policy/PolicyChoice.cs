using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PolicyChoice : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _title, _reputation, _budget;
    [SerializeField] Button _select;
    [SerializeField] Image _cdIcon, _efIcon;

    public TextMeshProUGUI Title => _title;
    public TextMeshProUGUI Rep => _reputation;
    public TextMeshProUGUI Budget => _budget;

    public Image CDIcon => _cdIcon;
    public Image EFIcon => _efIcon;

    public Button Select => _select;
}
