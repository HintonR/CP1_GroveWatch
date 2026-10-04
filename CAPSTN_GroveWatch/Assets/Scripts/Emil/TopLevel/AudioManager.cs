using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Music
{
    Title,
    Gameplay,
    Research,
    Policy,
    GameOver,
    Victory,
    Overworld,
    Cebu,
    Bagiuo,
    Pawalan
}
public enum SFX
{
    Rune,
    Purchase,
    Research,
    Back,
    Generic,
    Invalid,
    Text,
    Dropped,
    Switch,
    Deforestation,
    Policy,
    Success,
    Money
}

public class AudioManager : Singleton<AudioManager>
{

    ServiceHub _sH;
    const float _maxBGMVolume = .3f;

    [Header("Audio Sources")]
    public AudioSource _bgm1;
    public AudioSource _bgm2;
    public AudioSource _sfx;

    [Header("Volume")]
    [SerializeField, Range(0f, 1f)] float _masterVolume = 1f;
    [SerializeField, Range(0f, 1f)] float _bgmVolume = 1f;
    [SerializeField, Range(0f, 1f)] float _sfxVolume = 1f;

    AudioClip _titleBGM, _gameplayBGM, _researchBGM, _policyBGM, _gameoverBGM, _victoryBGM;

    AudioClip _runeBTN, _purchaseBTN, _researchBTN, _backBTN, _genericBTN, 
            _invalidSFX, _textSFX, _droppedSFX, _switchSFX, _deforSFX, 
            _policySFX, _successSFX, _moneySFX;

    AudioClip _drivingBGM, _cebuBGM, _bagiuoBGM, _palawanBGM;

    Music _current;
    Coroutine _currentCrossfade;

    public Music Current => _current;
    public float MasterVolume => _masterVolume;
    public float BGMVolume => _bgmVolume;
    public float SFXVolume => _sfxVolume;
    float CurrentBGMVolume => _maxBGMVolume * _masterVolume * _bgmVolume;
    float CurrentSFXVolume => _masterVolume * _sfxVolume;
    
    void Awake()
    {
        _sH = ServiceHub.Instance;
        _sH._aM = this;

        _bgm1 = gameObject.AddComponent<AudioSource>();
        _bgm2 = gameObject.AddComponent<AudioSource>();
        _sfx = gameObject.AddComponent<AudioSource>();

        _bgm1.loop = true;
        _bgm2.loop = true;
        _sfx.loop = false;

        _bgm1.playOnAwake = false;
        _bgm2.playOnAwake = false;
        _sfx.playOnAwake = false;
        ApplyVolumes();

        //ADD RESOURCES HERE
        _titleBGM     = Resources.Load<AudioClip>("Audio/TitleScreen"); //To Add New
        _gameplayBGM  = Resources.Load<AudioClip>("Audio/GameplayBGM");
        _researchBGM  = Resources.Load<AudioClip>("Audio/UpgradesBGM");
        _policyBGM    = Resources.Load<AudioClip>("Audio/PolicyBGM");
        _gameoverBGM  = Resources.Load<AudioClip>("Audio/GameOverBGM");
        _victoryBGM   = Resources.Load<AudioClip>("Audio/TitleScreen"); //To Add New
        
        _runeBTN      = Resources.Load<AudioClip>("Audio/Rune"); 
        _purchaseBTN  = Resources.Load<AudioClip>("Audio/Purchase");
        _researchBTN  = Resources.Load<AudioClip>("Audio/Pause"); //To Add New
        _backBTN      = Resources.Load<AudioClip>("Audio/Back");
        _genericBTN   = Resources.Load<AudioClip>("Audio/Generic");
        _invalidSFX   = Resources.Load<AudioClip>("Audio/Invalid");
        _textSFX      = Resources.Load<AudioClip>("Audio/Text");
        _droppedSFX   = Resources.Load<AudioClip>("Audio/Dropped");
        _switchSFX    = Resources.Load<AudioClip>("Audio/Switch");
        _deforSFX     = Resources.Load<AudioClip>("Audio/Deforestation");
        _policySFX    = Resources.Load<AudioClip>("Audio/PolicySFX");
        _successSFX   = Resources.Load<AudioClip>("Audio/Success");
        _moneySFX     = Resources.Load<AudioClip>("Audio/Money");

        _drivingBGM   = Resources.Load<AudioClip>("Audio/Driving");

        PreloadMusic();
    }

    void PreloadMusic()
    {
        AudioClip[] clips = { _titleBGM, _gameplayBGM, _researchBGM, _policyBGM, _gameoverBGM, _victoryBGM, _drivingBGM };

        foreach (var c in clips)
            c.LoadAudioData();
    }

    public void SetMasterVolume(float volume)
    {
        _masterVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
    }

    public void SetBGMVolume(float volume)
    {
        _bgmVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
    }

    public void SetSFXVolume(float volume)
    {
        _sfxVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
    }

    void ApplyVolumes()
    {
        if (_currentCrossfade == null)
        {
            if (_bgm1 != null)
                _bgm1.volume = CurrentBGMVolume;

            if (_bgm2 != null)
                _bgm2.volume = CurrentBGMVolume;
        }

        if (_sfx != null)
            _sfx.volume = CurrentSFXVolume;
    }

    public void PlayMusic(Music _music)
    {
        AudioClip _nextMusic = null;
        switch (_music)
        {
            case Music.Title     : _nextMusic = _titleBGM;     break;
            case Music.Gameplay  : _nextMusic = _gameplayBGM;  break;
            case Music.Research  : _nextMusic = _researchBGM;  break;
            case Music.Policy    : _nextMusic = _policyBGM;    break;
            case Music.Victory   : _nextMusic = _victoryBGM;   break;
            case Music.GameOver  : _nextMusic = _gameoverBGM;  break;
            case Music.Overworld : _nextMusic = _drivingBGM;   break;
        }

        _current = _music;
        CrossfadeTo(_nextMusic);
    }

    public void CrossfadeTo(AudioClip newClip)
    {

        if ((_bgm1.clip == newClip && _bgm1.isPlaying) || 
            (_bgm2.clip == newClip && _bgm2.isPlaying))
            return;

        float _cfd = 2f;
        AudioSource activeSource = _bgm1.isPlaying ? _bgm1 : _bgm2;
        AudioSource inactiveSource = activeSource == _bgm1 ? _bgm2 : _bgm1;

        if (_currentCrossfade != null)
            StopCoroutine(_currentCrossfade);

        _currentCrossfade = StartCoroutine(Crossfade(activeSource, inactiveSource, newClip, _cfd));
    }


    private IEnumerator Crossfade(AudioSource sourceOut, AudioSource sourceIn, AudioClip newClip, float duration)
    {

        float elapsedTime = 0f;

        sourceIn.clip = newClip;
        sourceIn.volume = 0f;
        sourceIn.Play();

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            sourceOut.volume = Mathf.Lerp(CurrentBGMVolume, 0f, t);
            sourceIn.volume = Mathf.Lerp(0f, CurrentBGMVolume, t);

            yield return null;
        }

        sourceOut.volume = 0f;
        sourceIn.volume = CurrentBGMVolume;

        sourceOut.Stop();

        _currentCrossfade = null;
    }

    public void PlaySFX(SFX sfx)
    {
        switch (sfx)
        {
            case SFX.Rune:          _sfx.PlayOneShot(_runeBTN,     0.25f); break;
            case SFX.Purchase:      _sfx.PlayOneShot(_purchaseBTN, 0.55f); break;
            case SFX.Research:      _sfx.PlayOneShot(_researchBTN,  0.3f); break;
            case SFX.Back:          _sfx.PlayOneShot(_backBTN,      0.4f); break;
            case SFX.Generic:       _sfx.PlayOneShot(_genericBTN,   0.3f); break;
            case SFX.Invalid:       _sfx.PlayOneShot(_invalidSFX,  0.35f); break;
            case SFX.Text:          _sfx.PlayOneShot(_textSFX,     0.08f); break;
            case SFX.Dropped:       _sfx.PlayOneShot(_droppedSFX,  0.25f); break;
            case SFX.Switch:        _sfx.PlayOneShot(_switchSFX,   0.08f); break;
            case SFX.Deforestation: _sfx.PlayOneShot(_deforSFX,     0.2f); break;
            case SFX.Policy:        _sfx.PlayOneShot(_policySFX,    0.5f); break;
            case SFX.Success:       _sfx.PlayOneShot(_successSFX,   0.3f); break;
            case SFX.Money:         _sfx.PlayOneShot(_moneySFX,    0.15f); break;
        }
        _sfx.pitch = 1.0f;
    }
}
