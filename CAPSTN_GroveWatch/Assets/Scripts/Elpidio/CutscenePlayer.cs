using EasyTransition;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutscenePlayer : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image displayImage;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Cutscene Mode UI")]
    [SerializeField] private GameObject continueIndicator;

    [Header("Tutorial Mode UI")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button startGameButton;

    [Header("Tutorial Prompts")]
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private Button promptYesButton;
    [SerializeField] private Button promptNoButton;

    [Header("Tutorial Content")]
    [SerializeField] private GameObject panelImage;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject navigationRow;
    [SerializeField] private CutsceneData introCutscene;

    [Header("Typewriter")]
    [SerializeField] private float charactersPerSecond = 40f;

    [Header("Image Fade")]
    [SerializeField] private float fadeOutDuration = 0.35f;
    [SerializeField] private float fadeInDuration = 0.35f;

    [Header("End Fade (Cutscenes)")]
    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private float endFadeDuration = 1.2f;
    [SerializeField] private float endHoldDelay = 0.4f;


    [Header("Testing")]
    [SerializeField] private CutsceneData testCutscene;


    [SerializeField] TransitionSettings _transition;

    private CutsceneData currentCutscene;
    private int currentLine;
    private Coroutine typingRoutine;
    private Coroutine lineRoutine;
    private bool isTyping;
    private bool isTransitioning;
    private string currentFullText;

    void Start()
    {
        //editor testing, pls empty testCutscene on real builds
        currentCutscene = CutsceneState.SelectedCutscene != null
            ? CutsceneState.SelectedCutscene
            : testCutscene;
        CutsceneState.Clear(); //clear so next playthrough needs a fresh assignment

        if (currentCutscene.playMode == CutscenePlayMode.Tutorial)
        {
            if (backButton != null) backButton.onClick.AddListener(GoBack);
            if (nextButton != null) nextButton.onClick.AddListener(Advance);
            if (startGameButton != null) startGameButton.onClick.AddListener(StartGameTutorial);
            if (promptPanel != null)
            {
                promptPanel.SetActive(true);
                SetTutorialContentActive(false);
                if (promptYesButton != null) promptYesButton.onClick.AddListener(OnPromptYes);
                if (promptNoButton != null) promptNoButton.onClick.AddListener(OnPromptNo);
                return;
            }
           
        }

        currentLine = 0;
        ShowLine(isFirstLine: true);
    }

    void OnPromptYes()
    {
        CutsceneState.SelectedCutscene = introCutscene;
        if (promptPanel != null) promptPanel.SetActive(false);
        SetTutorialContentActive(true);
        currentLine = 0;
        ShowLine(isFirstLine: true);
    }

    void OnPromptNo()
    {
        var _tM = TransitionManager.Instance();
        CutsceneState.SelectedCutscene = introCutscene;
        _tM.Transition("CutsceneScene", _transition, 0.2f);
    }
    void StartGameTutorial()
    {
        var _tM = TransitionManager.Instance();
        CutsceneState.SelectedCutscene = introCutscene;
        _tM.Transition("CutsceneScene", _transition, 0.2f);
    }
    void Update()
    {
        if (currentCutscene.playMode == CutscenePlayMode.Cutscene)
        {
            if (isTransitioning) return;
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                OnAdvanceInput();
        }
        else if (currentCutscene.playMode == CutscenePlayMode.Tutorial)
        {
            
        }
    }

    void ShowLine(bool isFirstLine = false)
    {
        var line = currentCutscene.lines[currentLine];
        currentFullText = line.text;
        if (line.changeMusic)
        {
            ServiceHub.Instance._aM.PlayMusic(line.music);
        }
        dialogueText.text = "";
        if (continueIndicator) continueIndicator.SetActive(false);

        bool spriteChanged = isFirstLine || (line.image != null && displayImage.sprite != line.image);

        if (lineRoutine != null) StopCoroutine(lineRoutine);
        lineRoutine = StartCoroutine(PlayLine(line.image, spriteChanged));

        if (currentCutscene.playMode == CutscenePlayMode.Tutorial)
        {
            UpdateTutorialUI();
        }
    }

    void UpdateTutorialUI()
    {
        int lastIndex = currentCutscene.lines.Length - 1;
        if (backButton != null)
        {
            backButton.gameObject.SetActive(currentLine > 0);
        }
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(currentLine < lastIndex);
        }
        if (startGameButton != null)
        {
            startGameButton.gameObject.SetActive(currentLine == lastIndex);
        }
        if (titleText != null)
        {
            titleText.text = currentCutscene.lines[currentLine].title;
        }
    }

    void SetTutorialContentActive(bool active)
    {
        if (panelImage != null) panelImage.SetActive(active);
        if (dialoguePanel != null) dialoguePanel.SetActive(active);
        if (navigationRow != null) navigationRow.SetActive(active);
        if (displayImage != null) displayImage.gameObject.SetActive(active);
        if (titleText != null) titleText.gameObject.SetActive(active);
    }

    IEnumerator PlayLine(Sprite newSprite, bool fade)
    {
        if (fade && newSprite != null)
        {
            isTransitioning = true;

            if (displayImage.sprite != null && displayImage.color.a > 0f)
                yield return FadeImage(1f, 0f, fadeOutDuration);

            displayImage.sprite = newSprite;

            yield return FadeImage(0f, 1f, fadeInDuration);

            isTransitioning = false;
        }
        else if (newSprite != null)
        {
            displayImage.sprite = newSprite;
        }

        if (typingRoutine != null) StopCoroutine(typingRoutine);
        typingRoutine = StartCoroutine(TypewriterRoutine());
        //if (currentCutscene.playMode == CutscenePlayMode.Cutscene)
        //{
        //    if (typingRoutine != null) StopCoroutine(typingRoutine);
        //    typingRoutine = StartCoroutine(TypewriterRoutine());
        //}
        //else
        //{
        //    if (typingRoutine != null) StopCoroutine(typingRoutine);
        //    typingRoutine = StartCoroutine(TypewriterRoutine());
        //}
    }

    IEnumerator FadeImage(float from, float to, float duration)
    {
        float t = 0f;
        Color c = displayImage.color;
        c.a = from;
        displayImage.color = c;

        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / duration);
            displayImage.color = c;
            yield return null;
        }

        c.a = to;
        displayImage.color = c;
    }

    IEnumerator TypewriterRoutine()
    {
        isTyping = true;
        if (continueIndicator) continueIndicator.SetActive(false);

        dialogueText.text = currentFullText;
        dialogueText.maxVisibleCharacters = 0;

        int total = currentFullText.Length;
        float interval = 1f / charactersPerSecond;
        float timer = 0f;
        int visible = 0;
        int charsSinceBlip = 0;

        while (visible < total)
        {
            timer += Time.deltaTime;
            while (timer >= interval && visible < total)
            {
                timer -= interval;
                visible++;
                dialogueText.maxVisibleCharacters = visible;
                charsSinceBlip++;
                if (charsSinceBlip >= 3) //spaced by 3 chars because the sfx murders your ears if it plays on each text
                {
                    ServiceHub.Instance._aM.PlaySFX(SFX.Text);
                    charsSinceBlip = 0;
                }
            }
            yield return null;
        }

        isTyping = false;
        if (continueIndicator) continueIndicator.SetActive(true);
    }

    void OnAdvanceInput()
    {
        if (isTyping)
        {
            if (typingRoutine != null) StopCoroutine(typingRoutine);
            dialogueText.text = currentFullText;
            dialogueText.maxVisibleCharacters = currentFullText.Length;
            isTyping = false;
            if (continueIndicator) continueIndicator.SetActive(true);
        }
        else
        {
            Advance();
        }
    }

    void Advance()
    {
        currentLine++;
        if (currentLine >= currentCutscene.lines.Length)
            OnCutsceneComplete();
        else
            ShowLine();
    }
    void StartGame()
    {
        var _tM = TransitionManager.Instance();
        _tM.Transition("Main", _transition, 0.2f);
    }
    void GoBack()
    {
        if (currentLine <= 0) return;
        currentLine--;
        ShowLine();
    }

    void OnCutsceneComplete()
    {
        if (continueIndicator) continueIndicator.SetActive(false);

        if (currentCutscene.playMode == CutscenePlayMode.Cutscene)
        {
            //StartCoroutine(EndFadeRoutine());
            var _tM = TransitionManager.Instance();
            _tM.Transition(currentCutscene.nextSceneName, _transition, 0.2f);
        }
        else
        {
            HandleCompletion();
        }
    }

    IEnumerator EndFadeRoutine() //mostly unused now, here if needed later
    {
        isTransitioning = true;

        if (endHoldDelay > 0f)
            yield return new WaitForSeconds(endHoldDelay);

        if (fadeGroup != null)
        {
            float t = 0f;
            float start = fadeGroup.alpha;
            while (t < endFadeDuration)
            {
                t += Time.deltaTime;
                fadeGroup.alpha = Mathf.Lerp(start, 0f, t / endFadeDuration);
                yield return null;
            }
            fadeGroup.alpha = 0f;
        }

        HandleCompletion();
    }

    void HandleCompletion()
    {
        switch (currentCutscene.onComplete)
        {
            case CutsceneCompletionAction.LoadScene:
                if (!string.IsNullOrEmpty(currentCutscene.nextSceneName))
                    SceneManager.LoadScene(currentCutscene.nextSceneName);
                else
                    Debug.Log("you can't park here sir (nextSceneName empty)");
                break;
            case CutsceneCompletionAction.DoNothing:
                Debug.Log("cutscene finished");
                break;
        }
    }
}