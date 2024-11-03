using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [Header("UI Elements")]
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI gameTimerText;
    public TextMeshProUGUI scoreText;

    private int score = 0;
    private float gameTime = 0f;
    private bool gameStarted = false;

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
        Debug.Log("GameManager Start called"); // Debugging line
        StartCoroutine(RoundStart());
    }

    private IEnumerator RoundStart()
    {
        Debug.Log("Starting countdown...");
        countdownText.gameObject.SetActive(true);
        countdownText.text = "3";
        yield return new WaitForSeconds(1f);
        countdownText.text = "2";
        yield return new WaitForSeconds(1f);
        countdownText.text = "1";
        yield return new WaitForSeconds(1f);
        countdownText.text = "GO!";
        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false);
        gameStarted = true;
        Debug.Log("Countdown finished, game started.");
    }

    private void Update()
    {
        if (gameStarted)
        {
            gameTime += Time.deltaTime;
            gameTimerText.text = FormatTime(gameTime);
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        int milliseconds = Mathf.FloorToInt((time * 1000F) % 1000F);
        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = "Score: " + score;
        Debug.Log("Score updated: " + score); // Debugging line
    }

    public void StartScaredState()
    {
        // Placeholder for starting the scared state on ghosts
    }

    public void LoseLife()
    {
        // Placeholder for lose life and respawn logic
    }
}
