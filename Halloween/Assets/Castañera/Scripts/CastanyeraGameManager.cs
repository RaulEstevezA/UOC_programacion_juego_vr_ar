using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CastanyeraGameManager : MonoBehaviour
{
    public static CastanyeraGameManager Instance;

    [Header("UI - Score")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text finalScoreText;

    [Header("UI - Timer")]
    [SerializeField] private TMP_Text timerText;          // ← Asigna el TextMeshPro del contador (Top-Right)
    [SerializeField] private float gameDurationSeconds = 60f; // ← Duración total (por defecto 60s)

    [Header("Referencias opcionales")]
    [SerializeField] private MonoBehaviour spawnerBehaviour;      // ← Arrastra tu Spawner (debe tener SetSpawningEnabled(bool))
    [SerializeField] private MonoBehaviour playerControllerBehaviour; // ← Arrastra tu CastanyeraController (debe tener SetInputEnabled(bool))

    public bool IsGameOver { get; private set; }

    private int score;
    private float remainingTime;
    private bool isRunning;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        // Estado inicial UI
        UpdateScoreUI();
        if (gameOverPanel) gameOverPanel.SetActive(false);

        // Iniciar partida
        remainingTime = Mathf.Max(1f, gameDurationSeconds); // evita valores 0/negativos
        isRunning = true;
        IsGameOver = false;

        UpdateTimerUI(remainingTime);

        // Arranque de sistemas (si se han asignado)
        SetSpawnerEnabled(true);
        SetPlayerInputEnabled(true);
    }

    void Update()
    {
        if (!isRunning || IsGameOver) return;

        // Cuenta atrás
        remainingTime -= Time.deltaTime;
        if (remainingTime < 0f) remainingTime = 0f;

        UpdateTimerUI(remainingTime);

        // Fin de partida al llegar a 0
        if (remainingTime <= 0f)
        {
            EndGame(); // reutilizamos tu flujo de Game Over
        }
    }

    public void AddScore(int amount)
    {
        if (IsGameOver) return;
        score += amount;
        UpdateScoreUI();
    }

    public void EndGame()
    {
        if (IsGameOver) return;

        IsGameOver = true;
        isRunning = false;

        // Parar sistemas de juego
        SetSpawnerEnabled(false);
        SetPlayerInputEnabled(false);

        // Mostrar resultados
        if (gameOverPanel) gameOverPanel.SetActive(true);
        if (finalScoreText) finalScoreText.text = $"Castañas: {score}";
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UpdateScoreUI()
    {
        if (scoreText) scoreText.text = $"Puntos: {score}";
    }

    private void UpdateTimerUI(float timeSeconds)
    {
        if (!timerText) return;

        // Ceil para mostrar 1:00 → 0:00 sin “saltar” a 0 antes de tiempo
        int total = Mathf.CeilToInt(timeSeconds);
        int minutes = total / 60;
        int seconds = total % 60;
        timerText.text = $"{minutes:0}:{seconds:00}";
    }

    // --- Helpers para no acoplar tipos concretos (evita dependencias fuertes) ---
    private void SetSpawnerEnabled(bool enabled)
    {
        if (spawnerBehaviour == null) return;

        // Requiere que el componente tenga un método público:
        // void SetSpawningEnabled(bool enabled)
        var method = spawnerBehaviour.GetType().GetMethod("SetSpawningEnabled");
        if (method != null) method.Invoke(spawnerBehaviour, new object[] { enabled });
    }

    private void SetPlayerInputEnabled(bool enabled)
    {
        if (playerControllerBehaviour == null) return;

        // Requiere que el componente tenga un método público:
        // void SetInputEnabled(bool enabled)
        var method = playerControllerBehaviour.GetType().GetMethod("SetInputEnabled");
        if (method != null) method.Invoke(playerControllerBehaviour, new object[] { enabled });
    }
}
