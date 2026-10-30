using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    public GameObject pausePanel;

    [Header("Optional")]
    public GameObject gameplayUI;
    public GameObject quitButton;
    public WaveUI waveUI;
    public TMP_Text victoryTimeText;
    public TMP_Text speedText;

    public bool IsPlaying { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsVictory { get; private set; }
    public bool IsPaused { get; private set; }
    public bool IsFinished => IsGameOver || IsVictory;

    private static bool skipMenuNext;

    private static readonly float[] speeds = { 1f, 2f, 4f };
    private int speedIndex;
    public float GameSpeed => speeds[speedIndex];

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateSpeedText();

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);

        if (quitButton != null && Application.platform == RuntimePlatform.WebGLPlayer)
            quitButton.SetActive(false);

        BaseHealth.Instance.OnBaseDestroyed += GameOver;
        if (WaveManager.Instance != null)
            WaveManager.Instance.OnAllWavesCompleted += Victory;

        if (skipMenuNext)
        {
            skipMenuNext = false;
            StartGame();
        }
        else
        {
            ShowMenu();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && IsPlaying && !IsFinished)
        {
            if (IsPaused) ResumeGame();
            else PauseGame();
        }
    }

    void OnDestroy()
    {
        if (BaseHealth.Instance != null)
            BaseHealth.Instance.OnBaseDestroyed -= GameOver;
        if (WaveManager.Instance != null)
            WaveManager.Instance.OnAllWavesCompleted -= Victory;
    }

    void ShowMenu()
    {
        if (mainMenuPanel == null)
        {
            Debug.LogWarning("GameManager: Main Menu Panel is not assigned, starting the game directly.");
            StartGame();
            return;
        }

        IsPlaying = false;
        IsPaused = false;
        Time.timeScale = 0f;

        mainMenuPanel.SetActive(true);
        if (gameplayUI != null) gameplayUI.SetActive(false);

        if (MusicManager.Instance != null) MusicManager.Instance.PlayMenu();
    }

    public void StartGame()
    {
        IsPlaying = true;
        IsPaused = false;
        Time.timeScale = GameSpeed;

        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (gameplayUI != null) gameplayUI.SetActive(true);

        if (MusicManager.Instance != null) MusicManager.Instance.PlayGame();
    }

    public void PauseGame()
    {
        if (!IsPlaying || IsFinished) return;

        IsPaused = true;
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = GameSpeed;
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    public void CycleSpeed()
    {
        speedIndex = (speedIndex + 1) % speeds.Length;
        UpdateSpeedText();

        if (IsPlaying && !IsPaused && !IsFinished)
            Time.timeScale = GameSpeed;
    }

    void UpdateSpeedText()
    {
        if (speedText != null)
            speedText.text = $"Speed: {GameSpeed:0}x";
    }

    void GameOver()
    {
        if (IsFinished) return;
        IsGameOver = true;

        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (MusicManager.Instance != null) MusicManager.Instance.PlayGameOver();

        Time.timeScale = 0f;
    }

    void Victory()
    {
        if (IsFinished) return;
        IsVictory = true;

        if (pausePanel != null) pausePanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(true);
        if (MusicManager.Instance != null) MusicManager.Instance.PlayVictory();

        if (victoryTimeText != null && waveUI != null)
        {
            int m = (int)(waveUI.ElapsedTime / 60f);
            int s = (int)(waveUI.ElapsedTime % 60f);
            victoryTimeText.text = $"Time: {m:00}:{s:00}";
        }

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        skipMenuNext = true;
        IsPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMenu()
    {
        skipMenuNext = false;
        IsPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif !UNITY_WEBGL
        Application.Quit();
#endif
    }
}