using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Background Music")]
    public AudioClip introMusic;
    public AudioClip normalStateMusic;
    public AudioClip scaredStateMusic;

    [Header("Sound Effects")]
    public AudioClip pelletCollection;
    public AudioClip powerPelletCollection;
    public AudioClip pacStudentMovement;
    public AudioClip pacStudentDeath;
    public AudioClip cherryCollection;

    private AudioSource musicSource;
    private AudioSource sfxSource;

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
            return;
        }

        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
    }

    private void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "StartScene")
        {
            PlayMusic(introMusic);
        }
        else if (currentScene == "SampleScene")
        {
            PlayMusic(normalStateMusic);
        }
        else if (currentScene == "Level2")
        {
            PlayMusic(normalStateMusic);
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip) return;

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayPacStudentMovementSFX()
    {
        if (!sfxSource.isPlaying)
        {
            sfxSource.clip = pacStudentMovement;
            sfxSource.loop = true;
            sfxSource.Play();
        }
    }

    public void StopPacStudentMovementSFX()
    {
        if (sfxSource.clip == pacStudentMovement)
        {
            sfxSource.Stop();
            sfxSource.loop = false;
        }
    }

    public void PlayPelletCollectionSFX() => PlaySFX(pelletCollection);
    public void PlayPowerPelletCollectionSFX() => PlaySFX(powerPelletCollection);
    public void PlayPacStudentDeathSFX() => PlaySFX(pacStudentDeath);
    public void PlayCherryCollectionSFX() => PlaySFX(cherryCollection);
}
