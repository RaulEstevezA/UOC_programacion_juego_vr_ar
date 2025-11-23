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
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private float gameDurationSeconds = 60f;
    [SerializeField] private Color normalTimerColor = Color.white;
    [SerializeField] private Color warningTimerColor = Color.red;
    [SerializeField] private float warningTime = 5f;

    [Header("Referencias opcionales")]
    [SerializeField] private MonoBehaviour spawnerBehaviour;
    [SerializeField] private MonoBehaviour playerControllerBehaviour;

    [Header("Música de fondo")]
    [SerializeField] private AudioSource baseMusicSource;
    [SerializeField] private AudioSource intenseMusicSource;
    [SerializeField] private float intenseStartTime = 15f;
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
        remainingTime = Mathf.Max(1f, gameDurationSeconds);
        isRunning = true;
        IsGameOver = false;

        UpdateTimerUI(remainingTime);
        if (timerText) timerText.color = normalTimerColor;

        // Arranque de sistemas
        SetSpawnerEnabled(true);
        SetPlayerInputEnabled(true);

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

        // Activar música intensa
        if (!hasSwitchedToIntense && remainingTime <= intenseStartTime)
        {
            hasSwitchedToIntense = true;
            StartCoroutine(FadeToIntenseMusic());
        }

        if (remainingTime <= 0f)
        {
            EndGame();
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

        // Parar sistemas
        SetSpawnerEnabled(false);
        SetPlayerInputEnabled(false);

        StopMusic();

        int finalScore = score;

        //  MODO HISTORIA 
        if (StoryModeController.Instance != null &&
            StoryModeController.Instance.storyModeActive)
        {
            if (gameOverPanel) gameOverPanel.SetActive(true);
            if (finalScoreText) finalScoreText.text = $"Castañas: {finalScore}";

            StartCoroutine(EndGameStoryMode(finalScore));
            return;
        }

        // MODO LIBRE 
        if (gameOverPanel) gameOverPanel.SetActive(true);
        if (finalScoreText) finalScoreText.text = $"Castañas: {finalScore}";
    }

    private IEnumerator EndGameStoryMode(int finalScore)
    {
        // Mostrar la puntuación 3s
        yield return new WaitForSecondsRealtime(3f);

        // Enviar al Story Mode
        StoryModeController.Instance.OnMiniGameFinished(finalScore);
    }

    private void UpdateScoreUI()
    {
        if (scoreText) scoreText.text = $"Puntos: {score}";
    }

    private void UpdateTimerUI(float timeSeconds)
    {
        if (!timerText) return;

        int total = Mathf.CeilToInt(timeSeconds);
        int minutes = total / 60;
        int seconds = total % 60;
        timerText.text = $"{minutes:0}:{seconds:00}";

        timerText.color = timeSeconds <= warningTime ? warningTimerColor : normalTimerColor;
    }

    private void SetSpawnerEnabled(bool enabled)
    {
        if (spawnerBehaviour == null) return;

        var method = spawnerBehaviour.GetType().GetMethod("SetSpawningEnabled");
        if (method != null) method.Invoke(spawnerBehaviour, new object[] { enabled });
    }

    private void SetPlayerInputEnabled(bool enabled)
    {
        if (playerControllerBehaviour == null) return;

        var method = playerControllerBehaviour.GetType().GetMethod("SetInputEnabled");
        if (method != null) method.Invoke(playerControllerBehaviour, new object[] { enabled });
    }

    private IEnumerator FadeToIntenseMusic()
    {
        if (baseMusicSource == null || intenseMusicSource == null) yield break;

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

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
