using System;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    //Audio Sources
    public static AudioManager Instance;
    private GameManager _gameManager;

    [Header("Audio Source")]

    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;


    //Audio Clips
    [Header("Audio Clip")]

    public AudioClip buttonClickSfx;
    public AudioClip backgroundMusic;
    public AudioClip plantSfx;
    public AudioClip waterSfx;
    public AudioClip harvestSfx;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _gameManager= GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

    private void Update()
    {
        if (_gameManager == null)
        {
            _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        }
    }


    public void PlaySfx(AudioClip sfx)
    {
        sfxSource.PlayOneShot(sfx);
    }

    public void PlayFarmSfx(AudioClip effect)
    {
        if(_gameManager.GetActiveCamera()!=_gameManager.gameCam) return;
        GameObject temp = new GameObject("TempAudio");
        AudioSource source = temp.AddComponent<AudioSource>();
        source.clip = effect;
        source.Play();
        Destroy(temp, effect.length);
    }
}
