using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

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
    [SerializeField] private Color normalTimerColor = Color.white;
    [SerializeField] private Color warningTimerColor = Color.red;
    [SerializeField] private float warningTime = 5f;

    [Header("Referencias opcionales")]
    [SerializeField] private MonoBehaviour spawnerBehaviour;      // ← Arrastra tu Spawner (debe tener SetSpawningEnabled(bool))
    [SerializeField] private MonoBehaviour playerControllerBehaviour; // ← Arrastra tu CastanyeraController (debe tener SetInputEnabled(bool))

    [Header("Música de fondo")]
    [SerializeField] private AudioSource baseMusicSource;
    [SerializeField] private AudioSource intenseMusicSource;
    [SerializeField] private float intenseStartTime = 15f;  // segundos restantes para activar música intensa
    [SerializeField] private float musicVolume = 0.6f;
    [SerializeField] private float musicFadeDuration = 2f;

    private bool hasSwitchedToIntense = false;
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
        if (timerText) timerText.color = normalTimerColor;

        // Arranque de sistemas (si se han asignado)
        SetSpawnerEnabled(true);
        SetPlayerInputEnabled(true);

        // Música
        hasSwitchedToIntense = false;

        if (baseMusicSource != null)
        {
            baseMusicSource.loop = true;
            baseMusicSource.volume = musicVolume;
            baseMusicSource.Play();
        }

        if (intenseMusicSource != null)
        {
            intenseMusicSource.Stop();
            intenseMusicSource.volume = 0f;
        }
    }

    void Update()
    {
        if (!isRunning || IsGameOver) return;

        // Cuenta atrás
        remainingTime -= Time.deltaTime;
        if (remainingTime < 0f) remainingTime = 0f;

        UpdateTimerUI(remainingTime);

        // Cambio de música a intensa
        if (!hasSwitchedToIntense && remainingTime <= intenseStartTime)
        {
            hasSwitchedToIntense = true;
            StartCoroutine(FadeToIntenseMusic());
        }

        // Fin de partida al llegar a 0
        if (remainingTime <= 0f)
        {
            EndGame(); // reutilizamos flujo de Game Over
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

        // pARAR MÚSICA
        StopMusic();

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

        // Color de aviso al final
        if (timeSeconds <= warningTime)
            timerText.color = warningTimerColor;
        else
            timerText.color = normalTimerColor;

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

    private IEnumerator FadeToIntenseMusic()
    {
        if (baseMusicSource == null || intenseMusicSource == null)
            yield break;

        intenseMusicSource.loop = true;
        intenseMusicSource.volume = 0f;
        intenseMusicSource.Play();

        float elapsed = 0f;
        float startBaseVol = baseMusicSource.volume;
        float targetVol = musicVolume;

        while (elapsed < musicFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / musicFadeDuration);

            baseMusicSource.volume = Mathf.Lerp(startBaseVol, 0f, t);
            intenseMusicSource.volume = Mathf.Lerp(0f, targetVol, t);

            yield return null;
        }

        baseMusicSource.Stop();
        baseMusicSource.volume = startBaseVol;
    }

    private void StopMusic()
    {
        if (baseMusicSource != null) baseMusicSource.Stop();
        if (intenseMusicSource != null) intenseMusicSource.Stop();
    }
}