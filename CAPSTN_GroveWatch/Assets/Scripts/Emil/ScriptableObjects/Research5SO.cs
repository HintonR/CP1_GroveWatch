using UnityEngine;

[CreateAssetMenu(menuName = "Research5")]
public class Research5SO : Research3SO
{
    [SerializeField] int _c4, _c5;

    public int Cost4 => _c4;
    public int Cost5 => _c5;

    public override int GetCostForLevel(int currentLevel)
    {
        return currentLevel switch
        {
            3 => _c4,
            4 => _c5,
            _ => base.GetCostForLevel(currentLevel)
        };
    }
}
