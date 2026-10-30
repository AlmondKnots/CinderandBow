using UnityEngine;
using UnityEngine.UI;

// Put this on your pause panel (or any settings panel) and assign the three sliders.
// Set all sliders to Min 0, Max 1 in the Inspector.
public class AudioSettingsUI : MonoBehaviour
{
    public Slider masterSlider;   // lowers both music and sound effects
    public Slider musicSlider;
    public Slider sfxSlider;

    void Awake()
    {
        if (masterSlider != null) masterSlider.onValueChanged.AddListener(OnMasterChanged);
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(OnMusicChanged);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(OnSfxChanged);
    }

    // Runs every time the panel opens, so the sliders always match the real volume
    void OnEnable()
    {
        if (masterSlider != null)
            masterSlider.SetValueWithoutNotify(AudioListener.volume);
        if (musicSlider != null && MusicManager.Instance != null)
            musicSlider.SetValueWithoutNotify(MusicManager.Instance.volume);
        if (sfxSlider != null && SoundManager.Instance != null)
            sfxSlider.SetValueWithoutNotify(SoundManager.Instance.masterVolume);
    }

    // AudioListener.volume scales everything that plays, so music and SFX
    // both get quieter while keeping their own balance
    void OnMasterChanged(float v)
    {
        AudioListener.volume = v;
    }

    void OnMusicChanged(float v)
    {
        if (MusicManager.Instance != null) MusicManager.Instance.SetVolume(v);
    }

    void OnSfxChanged(float v)
    {
        if (SoundManager.Instance != null) SoundManager.Instance.SetVolume(v);
    }
}