using UnityEngine;
using static Unity.VisualScripting.Member;

public class AudioManager : MonoBehaviour
{
    //Audio Sources

    [Header("Audio Source")]

    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;


    //Audio Clips

    [Header("Audio Clip")]

    public AudioClip buttonClickSFX;
    public AudioClip backgroundMusic;

    private void Start()
    {
        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }


    public void playSFX(AudioClip SFX)
    {
        SFXSource.PlayOneShot(SFX);
    }
}
