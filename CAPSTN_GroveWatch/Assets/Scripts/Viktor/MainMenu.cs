using EasyTransition;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    const float Q_LENGTH = 75;
    const float M_LENGTH = 200;
    const float L_LENGTH = 500;
    const float E_LENGTH = 999999999;
    ServiceHub _sH;

    [SerializeField] TransitionSettings _transition;

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameModePanel;
    public GameObject durationPanel;

    [Header("Main Menu Buttons")]
    public Button newGameButton;
    public Button optionsButton;
    public Button quitButton;

    [Header("Game Mode Buttons")]
    public Button classicButton;
    public Button endlessButton;
    public Button backMain;

    [Header("Duration Buttons")]
    public Button quickButton;
    public Button mediumButton;
    public Button longButton;
    public Button backMode;

    //cutscene here
    [SerializeField] private CutsceneData cutsceneToPlay;

    void Awake()
    {
        _sH = ServiceHub.Instance;
    }

    void Start()
    {
        newGameButton.onClick.AddListener(ShowGameModeMenu);
        newGameButton.onClick.AddListener(PlayGenericSFX);

        quitButton.onClick.AddListener(QuitGame);
       
        classicButton.onClick.AddListener(ShowDurationMenu);
        classicButton.onClick.AddListener(PlayGenericSFX);
        
        endlessButton.onClick.AddListener(() => OnDurationSelected(E_LENGTH, true));
 
        quickButton.onClick.AddListener(() => OnDurationSelected(Q_LENGTH));
        mediumButton.onClick.AddListener(() => OnDurationSelected(M_LENGTH));
        longButton.onClick.AddListener(() => OnDurationSelected(L_LENGTH));

        backMain.onClick.AddListener(ShowMainMenu);
        backMain.onClick.AddListener(PlayBackSFX);
        backMode.onClick.AddListener(ShowGameModeMenu);
        backMode.onClick.AddListener(PlayBackSFX);
   
        ShowMainMenu();
    }

    void PlayGenericSFX()
    {
        _sH._aM.PlaySFX(SFX.Generic);
    }

    void PlayBackSFX()
    {
        _sH._aM.PlaySFX(SFX.Back);
    }

    void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        gameModePanel.SetActive(false);
        durationPanel.SetActive(false);
    }

    void ShowGameModeMenu()
    {
        mainMenuPanel.SetActive(false);
        gameModePanel.SetActive(true);
        durationPanel.SetActive(false);
    }

    void ShowDurationMenu()
    {
        mainMenuPanel.SetActive(false);
        gameModePanel.SetActive(false);
        durationPanel.SetActive(true);
    }

    void OnDurationSelected(float progressValue, bool p)
    {
        _sH._aM.PlaySFX(SFX.Generic);
        _sH._gM._isEndless = true;
        _sH._gM.SetMaxProgress(progressValue);

        //cutscene
        var _tM = TransitionManager.Instance();
        _tM.Transition("TutorialScene", _transition, 0.2f);
        CutsceneState.SelectedCutscene = cutsceneToPlay;
    }

    void OnDurationSelected(float progressValue)
    {
        _sH._aM.PlaySFX(SFX.Generic);
        _sH._gM.SetMaxProgress(progressValue);

        //cutscene
        var _tM = TransitionManager.Instance();
        _tM.Transition("TutorialScene", _transition, 0.2f);
        CutsceneState.SelectedCutscene = cutsceneToPlay;
    }

    void QuitGame()
    {
        Application.Quit();
    }
}