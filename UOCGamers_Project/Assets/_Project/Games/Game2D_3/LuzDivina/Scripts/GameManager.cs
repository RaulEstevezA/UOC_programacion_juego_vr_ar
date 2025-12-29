using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Referencias de escena")]
    [SerializeField] private BoardController board;      // arrastra el Board
    [SerializeField] private Button resetButton;         // botón "Reiniciar" (el de HUD si lo tienes)
    [SerializeField] private TextMeshProUGUI timeText;   // "Tiempo: 120"
    [SerializeField] private TextMeshProUGUI puzzleText; // "Puzzle 0/3"
    [SerializeField] private GameObject winPanel;        // panel de victoria FINAL
    [SerializeField] private GameObject gameOverPanel;   // panel de game over

    [Header("Botones GameOver (listeners)")]
    // Estos botones van DENTRO del gameOverPanel
    [SerializeField] private Button gameOverRestartButton;
    [SerializeField] private Button gameOverMenuButton;

    [Header("UI GameOver (visibilidad por modo)")]
    // Arrastra AQUÍ el GameObject del botón (Btn_Reiniciar y Btn_VolverMenu)
    [SerializeField] private GameObject btnReiniciarGO;
    [SerializeField] private GameObject btnVolverMenuGO;

    [Header("Escenas")]
    [SerializeField] private int menuLibreBuildIndex = 4;

    [Header("Logica de juego")]
    [SerializeField] private int startTimeSeconds = 120;
    [SerializeField] private int totalPuzzles = 3;

    private int puzzlesSolved = 0;
    private float timeLeft;
    private Coroutine timerCo;
    private bool levelWon;
    private bool gameOver;

    private void Awake()
    {
        // Botón reset HUD (si existe)
        if (resetButton)
        {
            resetButton.onClick.RemoveAllListeners();
            resetButton.onClick.AddListener(OnPressReset);
        }

        // Suscripción al board
        if (board)
            board.OnBoardSolved += HandleBoardSolved;

        // Botones del panel de GameOver (si los asignas por Inspector)
        if (gameOverRestartButton)
        {
            gameOverRestartButton.onClick.RemoveAllListeners();
            gameOverRestartButton.onClick.AddListener(ReiniciarDesdeGameOver);
        }

        if (gameOverMenuButton)
        {
            gameOverMenuButton.onClick.RemoveAllListeners();
            gameOverMenuButton.onClick.AddListener(VolverAMenuLibre);
        }

        // Estado inicial de visibilidad (por si entras en historia y se ve desde el principio)
        RefreshEndButtonsVisibility();
    }

    private void OnDestroy()
    {
        if (board)
            board.OnBoardSolved -= HandleBoardSolved;
    }

    private void Start() => StartLevel();

    private void StartLevel()
    {
        levelWon = false;
        gameOver = false;
        puzzlesSolved = 0;

        if (winPanel) winPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);

        if (board)
        {
            board.ResetBoard();
            board.SetInteractable(true);
        }

        if (timerCo != null) StopCoroutine(timerCo);
        timeLeft = Mathf.Max(0, startTimeSeconds);
        UpdateHud();
        timerCo = StartCoroutine(TimerTick());
    }

    private IEnumerator TimerTick()
    {
        var wait = new WaitForSeconds(0.1f);
        while (!levelWon && !gameOver && timeLeft > 0f)
        {
            timeLeft -= 0.1f;
            if (timeLeft < 0f) timeLeft = 0f;
            UpdateHud();
            yield return wait;
        }

        if (!levelWon && !gameOver && timeLeft <= 0f)
            ShowGameOver();
    }

    private void ShowGameOver()
    {
        gameOver = true;

        if (timerCo != null)
        {
            StopCoroutine(timerCo);
            timerCo = null;
        }

        if (board) board.SetInteractable(false);

        if (gameOverPanel) gameOverPanel.SetActive(true);

        // ✅ Oculta/enseña botones según modo
        RefreshEndButtonsVisibility();

        Debug.Log("GAME OVER");

        HandleEndOfMinigame(false);
    }

    private void HandleBoardSolved()
    {
        if (gameOver) return;

        // +15s por puzzle completado
        timeLeft += 15f;

        if (timeLeft > startTimeSeconds)
            startTimeSeconds = Mathf.CeilToInt(timeLeft);

        puzzlesSolved = Mathf.Min(totalPuzzles, puzzlesSolved + 1);
        UpdateHud();

        Debug.Log("+15 segundos por completar el puzzle! Puzzle " + puzzlesSolved + "/" + totalPuzzles);

        if (puzzlesSolved >= totalPuzzles)
        {
            levelWon = true;

            if (timerCo != null)
            {
                StopCoroutine(timerCo);
                timerCo = null;
            }

            if (board) board.SetInteractable(false);
            if (winPanel) winPanel.SetActive(true);

            // (Opcional) Si en el winPanel también hay botones y quieres ocultarlos en historia:
            // RefreshEndButtonsVisibility();

            HandleEndOfMinigame(true);
        }
        else
        {
            if (board)
            {
                board.ResetBoard();
                board.SetInteractable(true);
            }
        }
    }

    private void HandleEndOfMinigame(bool victory)
    {
        int finalScore = puzzlesSolved * 10;

        // --- MODO HISTORIA ---
        if (StoryModeController.Instance != null &&
            StoryModeController.Instance.storyModeActive)
        {
            StartCoroutine(EndGameStoryMode(finalScore));
            return;
        }

        // --- MODO LIBRE ---
        Debug.Log("[Luz Divina - Modo Libre] Puntuacion final: " + finalScore);
    }

    private IEnumerator EndGameStoryMode(int score)
    {
        yield return new WaitForSecondsRealtime(3f);

        if (StoryModeController.Instance != null)
            StoryModeController.Instance.OnMiniGameFinished(score);
    }

    private void OnPressReset() => StartLevel();

    // =========================
    // BOTONES DEL GAME OVER
    // =========================

    public void ReiniciarDesdeGameOver()
    {
        StartLevel();
    }

    public void VolverAMenuLibre()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuLibreBuildIndex);
    }

    // =========================
    // VISIBILIDAD POR MODO
    // =========================

    private bool IsStoryMode()
    {
        return StoryModeController.Instance != null &&
               StoryModeController.Instance.storyModeActive;
    }

    private void RefreshEndButtonsVisibility()
    {
        bool story = IsStoryMode();

        // En historia: NO mostramos volver a menú libre
        if (btnVolverMenuGO) btnVolverMenuGO.SetActive(!story);

        // En historia: por defecto oculto también Reiniciar (cámbialo si lo quieres)
        if (btnReiniciarGO) btnReiniciarGO.SetActive(!story);
        // Si QUIERES que Reiniciar sí se vea en historia, usa esto:
        // if (btnReiniciarGO) btnReiniciarGO.SetActive(true);
    }

    private void UpdateHud()
    {
        if (timeText) timeText.text = "Tiempo: " + Mathf.CeilToInt(timeLeft);
        if (puzzleText) puzzleText.text = "Puzzle " + puzzlesSolved + "/" + Mathf.Max(1, totalPuzzles);
    }
}
