using UnityEngine;
using static Unity.VisualScripting.Member;

public class AudioManager : MonoBehaviour
{
    //Audio Sources
    public static AudioManager instance;

    [Header("Audio Source")]

    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;


    //Audio Clips
    [Header("Audio Clip")]

    public AudioClip buttonClickSFX;
    public AudioClip backgroundMusic;
    public AudioClip plantSFX;
    public AudioClip waterSFX;
    public AudioClip harvestSFX;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }




    private void Start()
    {
        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }


    public void playSFX(AudioClip SFX)
    {
        SFXSource.PlayOneShot(SFX);
    }

    public void playEffects(AudioClip Effect)
    {
        GameObject temp = new GameObject("TempAudio");
        AudioSource source = temp.AddComponent<AudioSource>();
        source.clip = Effect;
        source.Play();
        Destroy(temp, Effect.length);
    }
}
