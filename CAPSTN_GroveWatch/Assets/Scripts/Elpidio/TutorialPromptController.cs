using EasyTransition;
using UnityEngine;
using UnityEngine.UI;
public class TutorialPromptController : MonoBehaviour
{
    [Header("Prompt")]
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    [Header("Where To Go Next")]
    [SerializeField] private CutsceneData introCutscene;
    [SerializeField] private TransitionSettings _transition;
    [SerializeField] private float transitionDelay = 0.2f;

    ServiceHub _sHCached;
    ServiceHub _sH => _sHCached != null ? _sHCached : (_sHCached = ServiceHub.Instance);

    void Start()
    {
        if (yesButton != null) yesButton.onClick.AddListener(OnPromptYes);
        if (noButton != null) noButton.onClick.AddListener(OnPromptNo);
    }

    void OnPromptYes()
    {
        if (_sH != null && _sH._aM != null) _sH._aM.PlaySFX(SFX.Generic);
        TutorialState.ShouldShowTutorial = true;
        ContinueToIntro();
    }

    void OnPromptNo()
    {
        if (_sH != null && _sH._aM != null) _sH._aM.PlaySFX(SFX.Back);
        TutorialState.ShouldShowTutorial = false;
        ContinueToIntro();
    }

    void ContinueToIntro()
    {
        CutsceneSceneDirector.PlayInCutsceneScene(introCutscene, _transition, transitionDelay);
    }
}
