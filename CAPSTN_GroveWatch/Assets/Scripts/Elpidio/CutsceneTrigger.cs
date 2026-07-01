using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneTrigger : MonoBehaviour
{
    [SerializeField] private CutsceneData cutsceneToPlay;
    [SerializeField] private string cutsceneSceneName = "CutsceneScene";
    public void PlayCutscene()
    {
        CutsceneState.SelectedCutscene = cutsceneToPlay;
        SceneManager.LoadScene(cutsceneSceneName);
    }
}