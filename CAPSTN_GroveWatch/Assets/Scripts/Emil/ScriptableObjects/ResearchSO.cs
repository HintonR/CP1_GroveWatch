using UnityEngine;

[CreateAssetMenu(menuName = "Research")]
public class ResearchSO : ScriptableObject
{
    [SerializeField] string _name;
    [SerializeField, TextArea(2,2)] string _desc;
    [SerializeField] int _id, _cap, _c1, _c2;

    public string ResearchName => _name;
    public string Description => _desc;
    public int ID => _id;
    public int Cap => _cap;
    public int Cost1 => _c1;
    public int Cost2 =>_c2;

    public virtual int GetCostForLevel(int currentLevel)
    {
        if (currentLevel < 0 || currentLevel >= _cap)
            return -1;

        return currentLevel switch
        {
            0 => _c1,
            1 => _c2,
            _ => -1
        };
    }
}
