using EasyTransition;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneSceneDirector : MonoBehaviour
{
    public const string CutsceneSceneName = "CutsceneScene";

    [Header("Playback")]
    [SerializeField] private CutscenePlayer player;

    [Header("Where To Go After")]
    [SerializeField] private TransitionSettings _transition;
    [SerializeField] private float transitionDelay = 0.2f;
    [SerializeField] private string fallbackSceneName = "TutorialLevel"; //default "Main" so it'll go back to main gameplay

    [Header("Testing")]
    [SerializeField] private CutsceneData testCutscene; //plays when editor is in CutsceneScene, empty during real builds

    CutsceneData current;

    public static void PlayInCutsceneScene(CutsceneData data, TransitionSettings transition, float startDelay = 0.2f)
    {
        if (data == null)
        {
            Debug.LogError("CutsceneSceneDirector asked to play a null CutsceneData");
            return;
        }

        CutsceneState.SelectedCutscene = data;
        LoadScene(CutsceneSceneName, transition, startDelay);
    }

    public static void GoToCutsceneScene(TransitionSettings transition, float startDelay = 0.2f)
    {
        CutsceneState.LastLevel = SceneManager.GetActiveScene().name;
        LoadScene(CutsceneSceneName, transition, startDelay);
    }
    public static void PlayLevel(string levelScene)
    {
        MainMenu.ResetVariables();

        CutsceneState.PendingLevel = levelScene;
        LoadScene(CutsceneSceneName, Resources.Load<TransitionSettings>("Transitions/Brush/Brush"), 0.2f);
    }
    static void LoadScene(string sceneName, TransitionSettings transition, float startDelay)
    {
        if (string.IsNullOrEmpty(sceneName)) return;

        //no transition asset or no manager in this scene, just load instantly
        if (transition == null)
        {
            SceneManager.LoadScene(sceneName);
            return;
        }

        var _tM = TransitionManager.Instance();
        if (_tM == null)
        {
            SceneManager.LoadScene(sceneName);
            return;
        }

        if (_tM.isBusy) return;
        _tM.Transition(sceneName, transition, startDelay);
    }

    void Start()
    {
        current = CutsceneState.Consume();

        if (current == null) current = testCutscene; //for editor

        if (current == null)
        {
            Debug.LogError("CutsceneSceneDirector entered CutsceneScene with no cutscene queued", this);
            return;
        }

        if (player == null) player = GetComponentInChildren<CutscenePlayer>(true);
        if (player == null) player = FindObjectOfType<CutscenePlayer>(true);

        if (player == null)
        {
            Debug.LogError("CutsceneSceneDirector no CutscenePlayer in the scene", this);
            return;
        }

        player.onFinished.AddListener(GoToNextScene);
        player.gameObject.SetActive(true);
        player.Play(current);
    }

    void GoToNextScene()
    {
        if (player != null) player.onFinished.RemoveListener(GoToNextScene);

        string next = !string.IsNullOrEmpty(current.nextSceneName) ? current.nextSceneName : fallbackSceneName;

        if (string.IsNullOrEmpty(next))
        {
            Debug.Log("cutscene finished, nowhere to go (no nextSceneName and no fallback)");
            return;
        }

        LoadScene(next, _transition, transitionDelay);
    }
}
