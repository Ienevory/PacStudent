using UnityEngine;

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
        }

        // Set up audio sources
        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true; // Background music loops by default
    }

    private void Start()
    {
        PlayMusic(introMusic);
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

    // Functions to switch between different music states
    public void PlayNormalStateMusic() => PlayMusic(normalStateMusic);

    public void PlayScaredStateMusic() => PlayMusic(scaredStateMusic);

    // Functions to play specific sound effects
    public void PlayPelletCollectionSFX() => PlaySFX(pelletCollection);

    public void PlayPowerPelletCollectionSFX() => PlaySFX(powerPelletCollection);

    public void PlayPacStudentMovementSFX() => PlaySFX(pacStudentMovement);

    public void PlayPacStudentDeathSFX() => PlaySFX(pacStudentDeath);
}
