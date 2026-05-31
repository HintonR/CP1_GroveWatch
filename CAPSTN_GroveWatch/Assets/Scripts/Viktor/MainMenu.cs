using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
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

    [Header("Duration Buttons")]
    public Button quickButton;
    public Button mediumButton;
    public Button longButton;

    
    [HideInInspector]
    public float chosenMaxProgress;

    private void Start()
    {
   
        newGameButton.onClick.AddListener(ShowGameModeMenu);
        quitButton.onClick.AddListener(QuitGame);

       
        classicButton.onClick.AddListener(ShowDurationMenu);

        
        quickButton.onClick.AddListener(() => OnDurationSelected("Quick", 30f));
        mediumButton.onClick.AddListener(() => OnDurationSelected("Medium", 100f));
        longButton.onClick.AddListener(() => OnDurationSelected("Long", 150f));

        
        ShowMainMenu();
    }

  

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        gameModePanel.SetActive(false);
        durationPanel.SetActive(false);
    }

    public void ShowGameModeMenu()
    {
        mainMenuPanel.SetActive(false);
        gameModePanel.SetActive(true);
        durationPanel.SetActive(false);
    }

    public void ShowDurationMenu()
    {
        mainMenuPanel.SetActive(false);
        gameModePanel.SetActive(false);
        durationPanel.SetActive(true);
    }

    

    private void OnDurationSelected(string durationName, float progressValue)
    {
       
        chosenMaxProgress = progressValue;

        Debug.Log(durationName + " mode selected! Logic ready. Value stored: " + chosenMaxProgress);

       
    }

    public void QuitGame()
    {
        Debug.Log("Player Has Quit The Game");
        Application.Quit();
    }
}