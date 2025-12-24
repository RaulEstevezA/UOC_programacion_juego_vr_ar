using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.XR.Management;
using System.Collections;
using UnityEngine.UI;

public class MicrogameManager : MonoBehaviour
{
    public static MicrogameManager Instance { get; private set; }

    [Header("Config")]
    public float durationSeconds = 120f;
    public int startingLives = 3;
    public ThreeLaneSpawner[] spawners;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    [Header("Vidas (UI Images)")]
    public Image[] lifeIcons; // San1 y San2 → representan las vidas extra

    [Header("Flujo (opcional)")]
    public string nextSceneName;
    public bool autoLoadNextOnContinue;

    public bool IsRunning { get; private set; }
    public bool IsGameOver { get; private set; }
    public int Score { get; private set; }
    public int Lives { get; private set; }
    float elapsed;

    public event Action<int, string> OnMicrogameEnded;
    [Header("Audio")]
    public AudioClip sanfermin;
    private AudioSource musicSource;


    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.clip = sanfermin;
        musicSource.loop = true;
        musicSource.volume = 0.4f;
        musicSource.spatialBlend = 0f;
        musicSource.Play();
        if (gameOverPanel) gameOverPanel.SetActive(false);

        // 🕶️ ACTIVAR VR AL ENTRAR AL MICROJUEGO
        StartCoroutine(StartVR());

        StartGame();

        if (StoryModeController.Instance != null && StoryModeController.Instance.storyModeActive)
        {
            OnMicrogameEnded += (score, reason) =>
            {
                StoryModeController.Instance.OnMiniGameFinished(score);
            };
        }
    }

    // 🕶️ ---------- VR ----------
    IEnumerator StartVR()
    {
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            Debug.LogError("❌ No se pudo iniciar XR");
            yield break;
        }

        XRGeneralSettings.Instance.Manager.StartSubsystems();
        Debug.Log("🕶️ VR ACTIVADO");
    }

    void StopVR()
    {
        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
            return;

        XRGeneralSettings.Instance.Manager.StopSubsystems();
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        Debug.Log("⛔ VR DESACTIVADO");
    }
    // 🕶️ ------------------------

    public void StartGame()
    {
        IsGameOver = false;
        IsRunning = true;
        Score = 0;
        Lives = startingLives; // 3
        elapsed = 0f;

        UpdateUI();
        UpdateLivesUI(); // actualiza iconos al inicio

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

        if (Lives <= 0)
        {
            Lives = 0;
            EndGame("OutOfLives");
        }

        UpdateUI();
        UpdateLivesUI(); // actualiza iconos al perder vida
    }

    void UpdateUI()
    {
        if (scoreText) scoreText.text = $"Puntos: {Score}";
        if (timerText) timerText.text = FormatTime(Mathf.Max(0f, durationSeconds - elapsed));
    }

    void UpdateLivesUI()
    {
        if (lifeIcons == null || lifeIcons.Length == 0) return;

        int livesLeft = Lives - 1; // excluimos la vida que se está jugando

        for (int i = 0; i < lifeIcons.Length; i++)
        {
            if (lifeIcons[i] != null)
            {
                // Activar solo si hay suficiente vidas extras
                lifeIcons[i].gameObject.SetActive(i < livesLeft);
            }
        }
    }


    public void EndGame(string reason)
    {
        if (IsGameOver) return;
        IsGameOver = true;
        IsRunning = false;

        foreach (var sp in spawners) if (sp) sp.StopSpawning();
        CleanupObstacles();

        if (gameOverPanel)
        {
            gameOverPanel.SetActive(true);
            if (finalScoreText) finalScoreText.text = $"Score: {Score}";
        }
        if (musicSource != null && musicSource.isPlaying)
            musicSource.Stop();
        // 🕶️ DESACTIVAR VR AL TERMINAR
        StopVR();

        if (StoryModeController.Instance != null && StoryModeController.Instance.storyModeActive)
        {
            StartCoroutine(NotifyStoryModeAfterDelay(3f));
        }

        OnMicrogameEnded?.Invoke(Score, reason);
    }

    private IEnumerator NotifyStoryModeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StoryModeController.Instance.OnMiniGameFinished(Score);
    }

    void CleanupObstacles()
    {
        MoveTowardsPlayer[] movers =
            GameObject.FindObjectsByType<MoveTowardsPlayer>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (var m in movers)
        {
            if (m != null)
                Destroy(m.gameObject);
        }
    }


    public void OnPressContinue()
    {
        if (autoLoadNextOnContinue && !string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }

    void OnDestroy()
    {
        StopVR();
    }
}
