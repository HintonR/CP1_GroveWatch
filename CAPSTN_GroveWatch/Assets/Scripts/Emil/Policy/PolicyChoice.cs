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
    [SerializeField] Image _f1Icon, _f2Icon, _f3Icon;

    public TextMeshProUGUI Title => _title;
    public TextMeshProUGUI Rep => _reputation;
    public TextMeshProUGUI Budget => _budget;

    public Image CDIcon => _cdIcon;
    public Image EFIcon => _efIcon;

    public Image F1 => _f1Icon;
    public Image F2 => _f2Icon;
    public Image F3 => _f3Icon;

    public Button Select => _select;
}
