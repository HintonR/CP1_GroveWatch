using EasyTransition;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutscenePlayer : DialoguePlayerBase
{
    [Header("Cutscene Mode UI")]
    [SerializeField] private GameObject continueIndicator;

    //used febucci as inspiration
    //https://blog.febucci.com/2019/02/skip-cutscenes-button/ 
    [Header("Hold To Skip")]
    [SerializeField] private bool allowSkip = true;
    [SerializeField] private KeyCode skipKey = KeyCode.C;
    [SerializeField] private float holdToSkipDuration = 1.25f;
    [SerializeField] private float skipReleaseDecay = 2f;
    [SerializeField] private GameObject skipPromptRoot;  
    [SerializeField] private Image skipFillImage;

    [Header("Gameplay")]
    [SerializeField] private bool holdGameplayWhileActive = false;

    float skipProgress; //seconds held, clamped to [0, holdToSkipDuration]
    bool heldGameplay;

    protected override void Awake()
    {
        base.Awake();
        if (continueIndicator) continueIndicator.SetActive(false);
        UpdateSkipUI();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        ReleaseGameplay();
    }

    public override void Play(CutsceneData data)
    {
        skipProgress = 0f;
        UpdateSkipUI();
        HoldGameplay();
        base.Play(data);
    }

    void Update()
    {
        if (!IsPlaying) return;

        if (HandleSkipInput()) return; //skipped this frame, nothing left to do

        if (IsFading) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            OnAdvanceInput();
    }

    bool HandleSkipInput()
    {
        if (!allowSkip || holdToSkipDuration <= 0f) return false;

        bool holding = Input.GetKey(skipKey);
        float rate = holding ? 1f : -Mathf.Max(0.01f, skipReleaseDecay);
        skipProgress = Mathf.Clamp(skipProgress + rate * Time.unscaledDeltaTime, 0f, holdToSkipDuration);

        UpdateSkipUI();

        if (holding && skipProgress >= holdToSkipDuration)
        {
            Skip();
            return true;
        }
        return false;
    }

    void UpdateSkipUI()
    {
        float progress = holdToSkipDuration > 0f ? Mathf.Clamp01(skipProgress / holdToSkipDuration) : 0f;

        if (skipFillImage != null) skipFillImage.fillAmount = progress;

        if (skipPromptRoot != null)
        {
            if (skipPromptRoot.activeSelf != allowSkip) skipPromptRoot.SetActive(allowSkip);
        }
    }

    public void Skip()
    {
        if (!IsPlaying) return;

        ApplyRemainingMusicCues();
        skipProgress = 0f;
        UpdateSkipUI();
        Finish();
    }
    void ApplyRemainingMusicCues()
    {
        if (sequence == null || sequence.lines == null) return;
        if (_sH == null || _sH._aM == null) return;

        for (int i = LastLineIndex; i > currentLine; i--)
        {
            if (sequence.lines[i].changeMusic)
            {
                _sH._aM.PlayMusic(sequence.lines[i].music);
                return;
            }
        }
    }

    void OnAdvanceInput()
    {
        if (IsTyping)
            CompleteTyping();
        else
            AdvanceLine();
    }

    protected override void OnLineShown(DialogueLine line)
    {
        if (continueIndicator) continueIndicator.SetActive(false);
    }

    protected override void OnTypingStarted()
    {
        if (continueIndicator) continueIndicator.SetActive(false);
    }

    protected override void OnTypingFinished()
    {
        if (continueIndicator) continueIndicator.SetActive(true);
    }

    protected override void OnSequenceComplete()
    {
        if (continueIndicator) continueIndicator.SetActive(false);
        if (skipPromptRoot != null) skipPromptRoot.SetActive(false);

        ReleaseGameplay();
        gameObject.SetActive(false);
    }

    void HoldGameplay()
    {
        if (!holdGameplayWhileActive || heldGameplay) return;
        if (_sH == null || _sH._gM == null) return;

        _sH._gM._inScreen = true;
        heldGameplay = true;
    }

    void ReleaseGameplay()
    {
        if (!heldGameplay) return;
        heldGameplay = false;

        if (_sH == null || _sH._gM == null) return;
        _sH._gM._inScreen = false;
    }
}