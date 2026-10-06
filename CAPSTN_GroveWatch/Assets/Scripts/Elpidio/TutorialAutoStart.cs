using UnityEngine;

public class TutorialAutoStart : MonoBehaviour
{
    [SerializeField] private CutsceneData introTutorial;

    void Start()
    {
        if (!TutorialState.ShouldShowTutorial) return;
        if (!TutorialState.FirstTime(introTutorial)) return;
        TutorialLoader.Show(introTutorial);
    }
}
