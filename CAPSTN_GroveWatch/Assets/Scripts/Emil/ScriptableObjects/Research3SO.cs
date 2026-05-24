using UnityEngine;

[CreateAssetMenu(menuName = "Research3")]
public class Research3SO : ResearchSO
{
    [SerializeField] int _c3;

    public int Cost3 => _c3;

    public override int GetCostForLevel(int currentLevel)
    {
        if (currentLevel == 2)
            return _c3;

        return base.GetCostForLevel(currentLevel);
    }
}
