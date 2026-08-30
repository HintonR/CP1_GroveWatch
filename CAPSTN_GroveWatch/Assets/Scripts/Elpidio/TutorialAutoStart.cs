using UnityEngine;

public class TutorialAutoStart : MonoBehaviour
{
    [SerializeField] private CutsceneData introTutorial;

    void Start()
    {
        if (!TutorialState.Consume()) return;
        TutorialLoader.Show(introTutorial);
    }
}
