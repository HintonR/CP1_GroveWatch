using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Policy Data")]
public class PolicyDataSO : ScriptableObject
{
    [SerializeField, TextArea(2,2)] string _prompt;

    [SerializeField] PolicyChoiceDataSO _c1;
    [SerializeField] PolicyChoiceDataSO _c2;
    [SerializeField] PolicyChoiceDataSO _c3;

    public string Prompt => _prompt;

    public PolicyChoiceDataSO Choice1 => _c1;
    public PolicyChoiceDataSO Choice2 => _c2;
    public PolicyChoiceDataSO Choice3 => _c3;

}
