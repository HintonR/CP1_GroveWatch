using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioSettings : MonoBehaviour
{
    ServiceHub _sH;
    const string _masterVolumeKey = "Audio.MasterVolume";
    const string _bgmVolumeKey = "Audio.BGMVolume";
    const string _sfxVolumeKey = "Audio.SFXVolume";

    [SerializeField] Slider _master, _bgm, _sfx;

    void Awake()
    {
        _sH = ServiceHub.Instance;
    }

    void OnEnable()
    {
        SetupSlider(_master);
        SetupSlider(_bgm);
        SetupSlider(_sfx);

        LoadVolumes();

        if (_master != null)
            _master.onValueChanged.AddListener(OnMasterVolumeChanged);

        if (_bgm != null)
            _bgm.onValueChanged.AddListener(OnBGMVolumeChanged);

        if (_sfx != null)
            _sfx.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    void OnDisable()
    {
        if (_master != null)
            _master.onValueChanged.RemoveListener(OnMasterVolumeChanged);

        if (_bgm != null)
            _bgm.onValueChanged.RemoveListener(OnBGMVolumeChanged);

        if (_sfx != null)
            _sfx.onValueChanged.RemoveListener(OnSFXVolumeChanged);

        PlayerPrefs.Save();
    }

    void SetupSlider(Slider slider)
    {
        if (slider == null)
            return;

        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;
    }

    void LoadVolumes()
    {
        AudioManager audioManager = _sH._aM;

        float masterVolume = PlayerPrefs.GetFloat(_masterVolumeKey, audioManager.MasterVolume);
        float bgmVolume = PlayerPrefs.GetFloat(_bgmVolumeKey, audioManager.BGMVolume);
        float sfxVolume = PlayerPrefs.GetFloat(_sfxVolumeKey, audioManager.SFXVolume);

        SetSliderValue(_master, masterVolume);
        SetSliderValue(_bgm, bgmVolume);
        SetSliderValue(_sfx, sfxVolume);

        audioManager.SetMasterVolume(masterVolume);
        audioManager.SetBGMVolume(bgmVolume);
        audioManager.SetSFXVolume(sfxVolume);
    }

    void SetSliderValue(Slider slider, float value)
    {
        if (slider == null)
            return;

        slider.SetValueWithoutNotify(Mathf.Clamp01(value));
    }

    void OnMasterVolumeChanged(float value)
    {
        _sH._aM.SetMasterVolume(value);
        PlayerPrefs.SetFloat(_masterVolumeKey, Mathf.Clamp01(value));
    }

    void OnBGMVolumeChanged(float value)
    {
        _sH._aM.SetBGMVolume(value);
        PlayerPrefs.SetFloat(_bgmVolumeKey, Mathf.Clamp01(value));
    }

    void OnSFXVolumeChanged(float value)
    {
        _sH._aM.SetSFXVolume(value);
        PlayerPrefs.SetFloat(_sfxVolumeKey, Mathf.Clamp01(value));
    }

    public void CloseSettings()
    {
        _sH._aM.PlaySFX(SFX.Back);
        SceneManager.UnloadSceneAsync("Settings");
    }
}
