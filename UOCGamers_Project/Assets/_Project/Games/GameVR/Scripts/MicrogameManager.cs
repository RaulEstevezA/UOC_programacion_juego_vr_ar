using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class MicrogameManager : MonoBehaviour
{
    public static MicrogameManager Instance { get; private set; }

    [Header("Config")]
    [Tooltip("Duración máxima en segundos (300 = 5 min)")]
    public float durationSeconds = 120f;
    [Tooltip("Vidas iniciales")]
    public int startingLives = 3;
    [Tooltip("Spawners controlados por este microjuego")]
    public ThreeLaneSpawner[] spawners;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI timerText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    [Header("Flujo (opcional)")]
    public string nextSceneName;           // escena del hub o del siguiente microjuego
    public bool autoLoadNextOnContinue;    // si quieres que cargue al pulsar Continuar

    public bool IsRunning { get; private set; }
    public bool IsGameOver { get; private set; }
    public int Score { get; private set; }
    public int Lives { get; private set; }
    float elapsed;

    // Si tu hub quiere leer el resultado:
    public event Action<int, string> OnMicrogameEnded; // (puntuación, motivo)

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (gameOverPanel) gameOverPanel.SetActive(false);
        StartGame();

        // → Vincular al modo historia si está activo
        if (StoryModeController.Instance != null && StoryModeController.Instance.storyModeActive)
        {
            OnMicrogameEnded += (score, reason) =>
            {
                StoryModeController.Instance.OnMiniGameFinished(score);
            };
        }
    }


    public void StartGame()
    {
        IsGameOver = false;
        IsRunning = true;
        Score = 0;
        Lives = startingLives;
        elapsed = 0f;
        UpdateUI();

        // Enciende spawners
        foreach (var sp in spawners) if (sp) sp.StartSpawning();
    }

    void Update()
    {
        if (!IsRunning) return;

        elapsed += Time.deltaTime;
        float remaining = Mathf.Max(0f, durationSeconds - elapsed);
        if (timerText) timerText.text = FormatTime(remaining);

        if (remaining <= 0f)
            EndGame("TimeUp");
    }

    string FormatTime(float s)
    {
        int m = Mathf.FloorToInt(s / 60f);
        int r = Mathf.FloorToInt(s % 60f);
        return $"{m:00}:{r:00}";
    }

    public void AddScore(int amount)
    {
        if (IsGameOver) return;
        Score += amount;
        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        if (IsGameOver) return;

        Lives -= amount;
        Debug.Log($"Jugador recibe {amount} de daño. Vidas restantes: {Lives}");

        if (Lives <= 0)
        {
            Lives = 0;
            Debug.Log(" Vidas agotadas. Fin del juego: OutOfLives");
            EndGame("OutOfLives");
        }

        UpdateUI();
    }


    void UpdateUI()
    {
        if (scoreText) scoreText.text = $"Puntos: {Score}";
        if (livesText) livesText.text = $"Vidas: {Lives}";
        if (timerText) timerText.text = FormatTime(Mathf.Max(0f, durationSeconds - elapsed));
    }

    public void EndGame(string reason)
    {
        if (IsGameOver) return;
        IsGameOver = true;
        IsRunning = false;

        // Apaga spawners y limpia restos
        foreach (var sp in spawners) if (sp) sp.StopSpawning();
        CleanupObstacles();

        if (gameOverPanel)
        {
            gameOverPanel.SetActive(true);
            if (finalScoreText) finalScoreText.text = $"Score: {Score}";
        }

        // Lanzamos la coroutine que avisará al StoryModeController después de 3 segundos
        if (StoryModeController.Instance != null && StoryModeController.Instance.storyModeActive)
        {
            StartCoroutine(NotifyStoryModeAfterDelay(3f));
        }

        // Disparamos el evento normal
        OnMicrogameEnded?.Invoke(Score, reason);
    }

    // Coroutine para esperar unos segundos antes de avisar al StoryModeController
    private System.Collections.IEnumerator NotifyStoryModeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StoryModeController.Instance.OnMiniGameFinished(Score);
    }


    void CleanupObstacles()
    {
        var movers = FindObjectsOfType<MoveTowardsPlayer>();
        foreach (var m in movers) Destroy(m.gameObject);
    }

    // Botón "Continuar" del panel
    public void OnPressContinue()
    {
        if (autoLoadNextOnContinue && !string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
        // Si no, tu hub puede escuchar OnMicrogameEnded o leer MicrogameManager.Instance.Score
    }
}
