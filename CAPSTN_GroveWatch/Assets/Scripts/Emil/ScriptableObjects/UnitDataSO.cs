using UnityEngine;

public enum UnitType
{
    Ranger,
    Police,
    Firefighter
}

[CreateAssetMenu(menuName = "Unit")]
public class UnitData : ScriptableObject
{
    [SerializeField] UnitType _type;
    [SerializeField] float _cooldown;
    [SerializeField] float _damage;
    [SerializeField] int _t1Inc, _t2Inc, _t3Inc;

    public UnitType Type => _type;
    public float Cooldown => _cooldown;
    public float Damage => _damage;
    public int T1 => _t1Inc;
    public int T2 => _t2Inc;
    public int T3 => _t3Inc;
}
