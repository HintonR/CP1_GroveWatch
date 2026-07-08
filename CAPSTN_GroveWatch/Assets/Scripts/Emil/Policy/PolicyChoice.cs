using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PolicyChoice : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _title, _reputation, _budget, _cooldown, _effectiveness;
    [SerializeField] Button _select;

    public TextMeshProUGUI Title => _title;
    public TextMeshProUGUI Rep => _reputation;
    public TextMeshProUGUI Budget => _budget;
    public TextMeshProUGUI CD => _cooldown;
    public TextMeshProUGUI EF => _effectiveness;

    public Button Select => _select;
}
