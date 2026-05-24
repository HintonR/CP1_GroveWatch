using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public enum ForestState
{
    Idle,
    Fire,
    Camping,
    Drought,
    Construction,
    Flooded,
    Logging,
    Dead
}

public enum Season
{
    Dry,
    Wet,
    Both
}


[CreateAssetMenu(menuName = "Forest State")]
public class ForestStateData : ScriptableObject
{
    [SerializeField, TextArea(2, 2)] string _name;
    [SerializeField] ForestState _stateType;
    [SerializeField] Texture2D _image;

    public string Name => _name;
    public ForestState StateType => _stateType;
    public Texture Image => _image;

}
