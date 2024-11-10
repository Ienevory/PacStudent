using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // For scene management

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI gameTimerText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI livesText;

    private int score = 0;
    private float gameTime = 0f;
    private bool gameStarted = false;
    private int lives = 3;
    private Vector2 pacStudentStartPos = new Vector2(1, -2);

    private List<GhostMovement> ghosts = new List<GhostMovement>();
    public float scaredStateDuration = 10f;
    private float scaredStateTimer = 0f;
    private bool isScaredState = false;

    // Invincibility settings
    private bool isInvincible = false;
    private float invincibilityDuration = 1f;
    private float invincibilityTimer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keep GameManager alive across scenes
            Debug.Log("GameManager instance created and set to DontDestroyOnLoad.");
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate GameManagers
            Debug.Log("Duplicate GameManager destroyed.");
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to sceneLoaded event
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe from sceneLoaded event
    }

    private void Start()
    {
        Debug.Log("GameManager Start called.");
        // If starting in SampleScene, assign UI elements and start the round
        if (SceneManager.GetActiveScene().name == "SampleScene")
        {
            Debug.Log("Starting in SampleScene.");
            AssignUIElements();
            StartCoroutine(RoundStart());
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);
        if (scene.name == "SampleScene") // Ensure this matches your exact scene name
        {
            Debug.Log("Assigning UI elements for SampleScene.");
            AssignUIElements();
            StartCoroutine(RoundStart());
        }
        else if (scene.name == "Level2")
        {
            Debug.Log("Assigning UI elements for Level2.");
            AssignUIElements();
            StartCoroutine(RoundStart());
        }
    }

    private void AssignUIElements()
    {
        Debug.Log("AssignUIElements called.");
        // Find the UI elements in the scene with correct hierarchy
        countdownText = GameObject.Find("CountdownText")?.GetComponent<TextMeshProUGUI>();
        gameTimerText = GameObject.Find("GameTimerText")?.GetComponent<TextMeshProUGUI>();
        scoreText = GameObject.Find("highScoreText")?.GetComponent<TextMeshProUGUI>();
        livesText = GameObject.Find("Lives")?.GetComponent<TextMeshProUGUI>();

        // Check for null references and log warnings
        if (countdownText == null) Debug.LogWarning("CountdownText not found in scene.");
        if (gameTimerText == null) Debug.LogWarning("GameTimerText not found in scene.");
        if (scoreText == null) Debug.LogWarning("ScoreText not found in scene.");
        if (livesText == null) Debug.LogWarning("LivesText not found in scene.");

        UpdateLivesUI(); // Update the lives UI after reassigning
    }

    private IEnumerator RoundStart()
    {
        // Re-populate ghosts list after scene load
        ghosts.Clear();
        ghosts.AddRange(Object.FindObjectsByType<GhostMovement>(FindObjectsSortMode.None)); // This was causing error
        Debug.Log("Number of ghosts found: " + ghosts.Count);

        Debug.Log("Starting countdown...");
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
            countdownText.text = "3";
            Debug.Log("Countdown set to 3");
            yield return new WaitForSeconds(1f);
            countdownText.text = "2";
            Debug.Log("Countdown set to 2");
            yield return new WaitForSeconds(1f);
            countdownText.text = "1";
            Debug.Log("Countdown set to 1");
            yield return new WaitForSeconds(1f);
            countdownText.text = "GO!";
            Debug.Log("Countdown set to GO!");
            yield return new WaitForSeconds(1f);
            countdownText.gameObject.SetActive(false);
            Debug.Log("Countdown complete.");
        }
        else
        {
            Debug.LogWarning("CountdownText is null. Skipping countdown.");
        }

        gameStarted = true;
        Debug.Log("Countdown finished, game started.");
    }


    private void Update()
    {
        if (gameStarted)
        {
            gameTime += Time.deltaTime;
            if (gameTimerText != null)
            {
                gameTimerText.text = FormatTime(gameTime);
                Debug.Log($"Game Timer updated to: {FormatTime(gameTime)}");
            }

            if (isScaredState)
            {
                scaredStateTimer -= Time.deltaTime;
                Debug.Log($"[GameManager] Scared State Timer: {scaredStateTimer}");
                if (scaredStateTimer <= 0f)
                {
                    EndScaredState();
                }
            }

            // Update invincibility timer
            if (isInvincible)
            {
                invincibilityTimer -= Time.deltaTime;
                if (invincibilityTimer <= 0f)
                {
                    isInvincible = false;
                    Debug.Log("Invincibility ended.");
                }
            }
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        int milliseconds = Mathf.FloorToInt((time * 1000F) % 1000F);
        string formattedTime = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        // Debug.Log($"Game Timer formatted: {formattedTime}");
        return formattedTime;
    }

    public void AddScore(int amount)
    {
        score += amount;
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
            Debug.Log("Score updated: " + score);
        }
        else
        {
            Debug.LogWarning("scoreText is null. Cannot update score UI.");
        }
    }

    public void StartScaredState()
    {
        // **Prevent Multiple Scared State Calls**
        if (isScaredState)
        {
            Debug.Log("Already in Scared State. Ignoring StartScaredState call.");
            return;
        }

        Debug.Log("Starting Scared State");
        isScaredState = true;
        scaredStateTimer = scaredStateDuration;
        Debug.Log($"scaredStateTimer set to: {scaredStateTimer}");

        foreach (GhostMovement ghost in ghosts)
        {
            ghost.SetState(GhostState.Scared);
        }

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayMusic(AudioManager.instance.scaredStateMusic);
            Debug.Log("Scared state music played.");
        }
        else
        {
            Debug.LogWarning("AudioManager instance is null. Could not play scared state music.");
        }
    }

    private void EndScaredState()
    {
        Debug.Log("Ending Scared State");
        isScaredState = false;

        foreach (GhostMovement ghost in ghosts)
        {
            if (ghost.currentState != GhostState.Dead)
            {
                ghost.SetState(GhostState.Normal);
            }
        }

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayMusic(AudioManager.instance.normalStateMusic);
            Debug.Log("Normal state music played.");
        }
        else
        {
            Debug.LogWarning("AudioManager instance is null. Could not play normal state music.");
        }
    }

    public void PacStudentCaught()
    {
        if (!isInvincible)
        {
            Debug.Log("PacStudent caught by a ghost!");
            LoseLife();
        }
        else
        {
            Debug.Log("PacStudent is invincible and was not caught.");
        }
    }

    private void LoseLife()
    {
        if (lives > 1)
        {
            lives--;
            Debug.Log("Life lost. Remaining lives: " + lives);
            UpdateLivesUI();

            // Activate invincibility
            isInvincible = true;
            invincibilityTimer = invincibilityDuration;
            Debug.Log("Invincibility activated for " + invincibilityDuration + " seconds.");
        }
        else
        {
            lives = 0;
            Debug.Log("No lives remaining.");
            UpdateLivesUI();
            GameOver();
        }
    }

    private void UpdateLivesUI()
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + lives;
            Debug.Log("Lives UI updated: " + lives);
        }
        else
        {
            Debug.LogWarning("LivesText is null. Cannot update lives UI.");
        }
    }

    private void ResetPacStudentPosition()
    {
        GameObject pacStudent = GameObject.FindGameObjectWithTag("Player");
        if (pacStudent != null)
        {
            pacStudent.transform.position = pacStudentStartPos;
            Debug.Log("PacStudent position reset to: " + pacStudentStartPos);
        }
        else
        {
            Debug.LogError("PacStudent not found in the scene!");
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over! All lives lost.");
        StartCoroutine(ReturnToStartScene()); // Call coroutine to return to StartScene
    }

    private IEnumerator ReturnToStartScene()
    {
        // Optionally, display a "Game Over" screen or animation here
        yield return new WaitForSeconds(2f); // Optional delay for game over feedback
        Debug.Log("Loading StartScene.");
        SceneManager.LoadScene("StartScene"); // Load the StartScene
    }
}
