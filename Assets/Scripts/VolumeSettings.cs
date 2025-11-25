using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    private bool _isInitializing = true;

    private void Start()
    {
        LoadVolume();
        _isInitializing = false;
    }

    private float VolumeToDecibels(float volume)
    {
        const float minLinear = 0.0001f; 
        const float minDecibels = -80f;  
        if (volume <= 0f)
            return minDecibels;
        volume = Mathf.Max(volume, minLinear);
        return Mathf.Log10(volume) * 20f;
    }
    public void SetMasterVolume()
    {
        if(!_isInitializing) playVolumeChangeSound();
        float volume = masterSlider.value;
        mixer.SetFloat("Master", VolumeToDecibels(volume));
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume()
    {
        if(!_isInitializing) playVolumeChangeSound();
        float volume = musicSlider.value;
        mixer.SetFloat("Music", VolumeToDecibels(volume));
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume()
    {
        if(!_isInitializing) playVolumeChangeSound();
        float volume = sfxSlider.value;
        mixer.SetFloat("SFX", VolumeToDecibels(volume));
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    private void LoadVolume()
    {    
        masterSlider.onValueChanged.RemoveAllListeners();
        musicSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();
        // load saved values
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            float masterVolume = PlayerPrefs.GetFloat("MasterVolume");
            masterSlider.value = masterVolume;
            mixer.SetFloat("Master", VolumeToDecibels(masterVolume));
        }
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            float musicVolume = PlayerPrefs.GetFloat("MusicVolume");
            musicSlider.value = musicVolume;
            mixer.SetFloat("Music", VolumeToDecibels(musicVolume));
        }
        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            float sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
            sfxSlider.value = sfxVolume;
            mixer.SetFloat("SFX", VolumeToDecibels(sfxVolume));
        }
         
    }
    public void playVolumeChangeSound()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Instance.buttonClickSfx);
    }

}
