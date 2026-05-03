using UnityEngine;
using UnityEngine.UI;

public class AudioSlidersUI : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider masterSlider;

    private const string MusicKey = "MusicVolume";
    private const string SFXKey = "SFXVolume";
    private const string MasterKey = "MasterVolume";

    private void Start()
    {
        LoadSavedVolumes();
        SetupSliders();
    }

    void LoadSavedVolumes()
    {
        float music = PlayerPrefs.GetFloat(MusicKey, AudioManager.I.GetMusicVolume01());
        float sfx = PlayerPrefs.GetFloat(SFXKey, AudioManager.I.GetSFXVolume01());
        float master = PlayerPrefs.GetFloat(MasterKey, AudioManager.I.GetMasterVolume01());

        musicSlider.SetValueWithoutNotify(music);
        sfxSlider.SetValueWithoutNotify(sfx);
        masterSlider.SetValueWithoutNotify(master);

        AudioManager.I.SetMusicVolume01(music);
        AudioManager.I.SetSFXVolume01(sfx);
        AudioManager.I.SetMasterVolume01(master);
    }

    void SetupSliders()
    {
        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXChanged);
        masterSlider.onValueChanged.AddListener(OnMasterChanged);
    }

    public void OnMusicChanged(float value)
    {
        AudioManager.I.SetMusicVolume01(value);
        PlayerPrefs.SetFloat(MusicKey, value);
    }

    public void OnSFXChanged(float value)
    {
        AudioManager.I.SetSFXVolume01(value);
        PlayerPrefs.SetFloat(SFXKey, value);
    }

    public void OnMasterChanged(float value)
    {
        AudioManager.I.SetMasterVolume01(value);
        PlayerPrefs.SetFloat(MasterKey, value);
    }

    private void OnDestroy()
    {
        PlayerPrefs.Save();
    }
}