using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Negative Forest State")]
public class NegativeForestState : ForestStateData
{
    [SerializeField] UnitType _requiredUnit;
    [SerializeField] Season _availableSeason;

    [SerializeField] float _health;
    [SerializeField] float _chance;  
    [SerializeField] float _damage;

    public UnitType RequiredUnit => _requiredUnit;
    public Season AvailableSeason => _availableSeason;
    public float Health => _health;
    public float Chance => _chance;
    public float Damage => _damage;

}
