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

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private void Awake()
    {
        // Ensure singleton instance
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return; // Exit if duplicate instance
        }

        // Set up audio sources
        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true; // Background music loops by default
    }

    private void Start()
    {
        // Play the appropriate background music based on the current scene
        string currentScene = SceneManager.GetActiveScene().name;
        
        if (currentScene == "StartScene")
        {
            PlayMusic(introMusic);
        }
        else if (currentScene == "SampleScene")
        {
            PlayMusic(normalStateMusic);
        }
    }

    // Function to play background music
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip) return; // Skip if same music is already playing

        musicSource.clip = clip;
        musicSource.Play();
    }

    // Function to play sound effects
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    // Play and stop specific sound effects for movement
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

    // Additional SFX functions for other actions
    public void PlayPelletCollectionSFX() => PlaySFX(pelletCollection);
    public void PlayPowerPelletCollectionSFX() => PlaySFX(powerPelletCollection);
    public void PlayPacStudentDeathSFX() => PlaySFX(pacStudentDeath);
}
