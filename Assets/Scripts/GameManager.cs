using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public delegate void GameDelegate();
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnGameOverConfirmed;

    public static GameManager instance;

    public GameObject startPage;
    public GameObject gameOverPage;
    public GameObject countdownPage;
    public Text scoreText;

    private int score = 0;
    private bool gameOver = false;

    private enum PageState
    {
        None, Start, GameOver, Countdown
    }

    private void Awake()
    {
        instance = this;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    public bool GameOver { get { return gameOver; } }

    public int Score { get { return score; } }

    private void SetPageState(PageState state)
    {
        switch (state)
        {
            case PageState.None:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.Start:
                startPage.SetActive(true);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.GameOver:
                startPage.SetActive(false);
                gameOverPage.SetActive(true);
                countdownPage.SetActive(false);
                break;
            case PageState.Countdown:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(true);
                break;
        }
    }

    public void ConfirmedGameOver()
    {
        // Activated when replay button is hit
        OnGameOverConfirmed(); // Event send TapController
        scoreText.text = "0";
        SetPageState(PageState.Start);
    }

    public void StartGame()
    {
        // Activated when play button is hit
        SetPageState(PageState.Countdown);
    }

    private void OnEnable()
    {
        CountdownText.OnCountdownFinished += OnCountdownFinished;
        TapController.OnPlayerDied += OnPlayerDied;
        TapController.OnPlayerScored += OnPlayerScored;
    }

    private void OnDisable()
    {
        CountdownText.OnCountdownFinished -= OnCountdownFinished;
        TapController.OnPlayerDied -= OnPlayerDied;
        TapController.OnPlayerScored -= OnPlayerScored;
    }

    private void OnCountdownFinished()
    {
        SetPageState(PageState.None);
        OnGameStarted(); // Event send TapController
        score = 0;
        gameOver = false;
        scoreText.enabled = true;
    }

    private void OnPlayerDied()
    {
        gameOver = true;
        int saveScore = PlayerPrefs.GetInt("HighScore");
        if (score > saveScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
        SetPageState(PageState.GameOver);
        scoreText.enabled = false;
    }

    private void OnPlayerScored()
    {
        score++;
        scoreText.text = score.ToString();
    }
}
