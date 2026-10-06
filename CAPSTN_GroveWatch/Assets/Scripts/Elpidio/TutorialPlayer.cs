using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPlayer : DialoguePlayerBase
{
    [Header("Tutorial Content")]
    [SerializeField] private CutsceneData tutorialData;

    [Header("Tutorial Mode UI")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button finishButton;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI _pageNumber;

    [Header("Feedback")]
    [SerializeField] private GameObject _tutorialPanel; //shake target

    [Header("Gameplay")]
    [SerializeField] private bool holdGameplayWhileActive = true;

    Shake shake;
    bool heldGameplay;

    protected override bool UseTypewriter => false;

    protected override void Awake()
    {
        base.Awake();

        if (_tutorialPanel != null) shake = _tutorialPanel.GetComponent<Shake>();

        if (backButton != null) backButton.onClick.AddListener(GoBack);
        if (nextButton != null) nextButton.onClick.AddListener(GoNext);
        if (finishButton != null) finishButton.onClick.AddListener(Close);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        ReleaseGameplay();
    }
    public void Show()
    {
        Show(tutorialData);
    }

    public void Show(CutsceneData data)
    {
        gameObject.SetActive(true);
        HoldGameplay();
        Play(data);
    }

    public void Close()
    {
        if (!IsPlaying) return;

        if (_sH != null && _sH._aM != null) _sH._aM.PlaySFX(SFX.Generic);
        Finish();
    }

    void GoNext()
    {
        if (_sH != null && _sH._aM != null) _sH._aM.PlaySFX(SFX.Generic);
        AdvanceLine();
    }

    void GoBack()
    {
        if (_sH != null && _sH._aM != null) _sH._aM.PlaySFX(SFX.Back);
        PreviousLine();
    }

    protected override void OnLineShown(DialogueLine line)
    {
        int lastIndex = LastLineIndex;

        if (backButton != null) backButton.gameObject.SetActive(currentLine > 0);
        if (nextButton != null) nextButton.gameObject.SetActive(currentLine < lastIndex);
        if (finishButton != null) finishButton.gameObject.SetActive(currentLine == lastIndex);

        if (titleText != null) titleText.text = line.title;
        if (_pageNumber != null) _pageNumber.text = (currentLine + 1) + "/" + sequence.lines.Length;

        if (shake != null) shake.DoShake();
    }

    protected override void OnSequenceComplete()
    {
        ReleaseGameplay();
        gameObject.SetActive(false);
    }

    void HoldGameplay()
    {
        if (!holdGameplayWhileActive || heldGameplay) return;
        if (_sH == null || _sH._gM == null) return;

        _sH._gM._inScreen = true;
        if (_sH._dUI != null) _sH._dUI._inScreen = true;
        heldGameplay = true;
    }

    void ReleaseGameplay()
    {
        if (!heldGameplay) return;
        heldGameplay = false;

        if (_sH == null || _sH._gM == null) return;
        if (_sH._dUI != null) _sH._dUI._inScreen = false;
        _sH._gM._inScreen = false;
    }
}
