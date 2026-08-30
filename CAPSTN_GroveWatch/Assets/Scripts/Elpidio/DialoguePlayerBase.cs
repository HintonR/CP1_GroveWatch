using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public abstract class DialoguePlayerBase : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] protected Image displayImage;
    [SerializeField] protected TextMeshProUGUI dialogueText;

    [Header("Typewriter")]
    [SerializeField] private float charactersPerSecond = 40f;

    [Header("Image Fade")]
    [SerializeField] private float fadeOutDuration = 0.35f;
    [SerializeField] private float fadeInDuration = 0.35f;

    [Header("Events")]
    public UnityEvent onFinished;

    ServiceHub _sHCached;
    protected ServiceHub _sH => _sHCached != null ? _sHCached : (_sHCached = ServiceHub.Instance);

    const int CharsPerBlip = 3; //sfx murders your ears if it plays on every char

    protected CutsceneData sequence;
    protected int currentLine;

    Music musicOnEntry;
    bool capturedMusic;

    Coroutine lineRoutine;
    Coroutine typingRoutine;
    string currentFullText;

    public bool IsPlaying { get; private set; }
    public bool IsTyping { get; private set; }
    public bool IsFading { get; private set; }

    protected int LastLineIndex => sequence != null && sequence.lines != null ? sequence.lines.Length - 1 : -1;
    protected DialogueLine CurrentLine => sequence.lines[currentLine];

    protected virtual bool UseTypewriter => true;

    protected virtual void Awake() { }

    protected virtual void OnDisable()
    {
        StopRoutines();
    }
    public virtual void Play(CutsceneData data)
    {
        if (data == null || data.lines == null || data.lines.Length == 0)
        {
            Debug.Log("CutsceneData null, stopping");
            Finish();
            return;
        }

        sequence = data;
        currentLine = 0;
        IsPlaying = true;

        CaptureMusic();

        ShowLine(isFirstLine: true);
    }

    protected virtual void ShowLine(bool isFirstLine = false)
    {
        var line = CurrentLine;
        currentFullText = line.text;

        if (line.changeMusic && _sH != null && _sH._aM != null)
            _sH._aM.PlayMusic(line.music);

        if (dialogueText != null)
        {
            dialogueText.maxVisibleCharacters = int.MaxValue;
            dialogueText.text = UseTypewriter ? "" : currentFullText;
        }

        bool spriteChanged = displayImage != null
            && (isFirstLine || (line.image != null && displayImage.sprite != line.image));

        OnLineShown(line);

        StopRoutines();
        lineRoutine = StartCoroutine(PlayLineRoutine(line.image, spriteChanged));
    }

    protected virtual void OnLineShown(DialogueLine line) { }

    protected virtual void OnTypingStarted() { }
    protected virtual void OnTypingFinished() { }

    IEnumerator PlayLineRoutine(Sprite newSprite, bool fade)
    {
        if (displayImage != null && newSprite != null)
        {
            if (fade)
            {
                IsFading = true;

                if (displayImage.sprite != null && displayImage.color.a > 0f)
                    yield return FadeImage(1f, 0f, fadeOutDuration);

                displayImage.sprite = newSprite;

                yield return FadeImage(0f, 1f, fadeInDuration);

                IsFading = false;
            }
            else
            {
                displayImage.sprite = newSprite;
            }
        }

        if (UseTypewriter)
            typingRoutine = StartCoroutine(TypewriterRoutine());
    }

    IEnumerator FadeImage(float from, float to, float duration)
    {
        Color c = displayImage.color;
        c.a = from;
        displayImage.color = c;

        if (duration <= 0f)
        {
            c.a = to;
            displayImage.color = c;
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(from, to, t / duration);
            displayImage.color = c;
            yield return null;
        }

        c.a = to;
        displayImage.color = c;
    }

    IEnumerator TypewriterRoutine()
    {
        IsTyping = true;
        OnTypingStarted();

        dialogueText.text = currentFullText;
        dialogueText.maxVisibleCharacters = 0;

        int total = currentFullText.Length;
        float interval = charactersPerSecond > 0f ? 1f / charactersPerSecond : 0f;
        float timer = 0f;
        int visible = 0;
        int charsSinceBlip = 0;

        while (visible < total)
        {
            timer += Time.unscaledDeltaTime;
            while (timer >= interval && visible < total)
            {
                timer -= interval;
                visible++;
                dialogueText.maxVisibleCharacters = visible;
                charsSinceBlip++;
                if (charsSinceBlip >= CharsPerBlip)
                {
                    if (_sH != null && _sH._aM != null) _sH._aM.PlaySFX(SFX.Text);
                    charsSinceBlip = 0;
                }
            }
            yield return null;
        }

        IsTyping = false;
        typingRoutine = null;
        OnTypingFinished();
    }

    protected void CompleteTyping()
    {
        if (!IsTyping) return;

        if (typingRoutine != null) StopCoroutine(typingRoutine);
        typingRoutine = null;

        dialogueText.text = currentFullText;
        dialogueText.maxVisibleCharacters = currentFullText.Length;
        IsTyping = false;
        OnTypingFinished();
    }

    protected void AdvanceLine()
    {
        if (!IsPlaying) return;

        currentLine++;
        if (currentLine > LastLineIndex)
            Finish();
        else
            ShowLine();
    }

    protected void PreviousLine()
    {
        if (!IsPlaying || currentLine <= 0) return;

        currentLine--;
        ShowLine();
    }
    protected void Finish()
    {
        StopRoutines();
        IsPlaying = false;
        IsTyping = false;
        IsFading = false;

        ApplyMusicOnEnd();

        onFinished?.Invoke();
        OnSequenceComplete();
    }

    void CaptureMusic()
    {
        capturedMusic = false;
        if (_sH == null || _sH._aM == null) return;

        musicOnEntry = _sH._aM.Current;
        capturedMusic = true;
    }
    void ApplyMusicOnEnd()
    {
        if (sequence == null) return;
        if (_sH == null || _sH._aM == null) return;

        switch (sequence.musicOnEnd)
        {
            case MusicEndAction.RestorePrevious:
                if (capturedMusic && _sH._aM.Current != musicOnEntry)
                    _sH._aM.PlayMusic(musicOnEntry);
                break;

            case MusicEndAction.PlaySpecific:
                _sH._aM.PlayMusic(sequence.endMusic);
                break;
        }
    }
    protected abstract void OnSequenceComplete();

    protected void StopRoutines()
    {
        if (lineRoutine != null) StopCoroutine(lineRoutine);
        if (typingRoutine != null) StopCoroutine(typingRoutine);
        lineRoutine = null;
        typingRoutine = null;
    }
}
