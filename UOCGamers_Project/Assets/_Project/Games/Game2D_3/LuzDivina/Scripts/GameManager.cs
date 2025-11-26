using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Referencias de escena")]
    [SerializeField] private BoardController board;      // arrastra el Board
    [SerializeField] private Button resetButton;         // arrastra "Reiniciar"
    [SerializeField] private TextMeshProUGUI timeText;   // "Tiempo: 120"
    [SerializeField] private TextMeshProUGUI puzzleText; // "Puzzle 0/3"
    [SerializeField] private GameObject winPanel;        // panel de victoria FINAL
    [SerializeField] private GameObject gameOverPanel;   // panel de game over

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
        if (resetButton)
        {
            resetButton.onClick.RemoveAllListeners();
            resetButton.onClick.AddListener(OnPressReset);
        }

        if (board)
            board.OnBoardSolved += HandleBoardSolved;
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
        puzzlesSolved = 0; // empezamos desde el puzzle 0

        if (winPanel) winPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);

        if (board)
        {
            board.ResetBoard();
            board.SetInteractable(true); // habilita clicks
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

        // Si salimos del bucle por tiempo agotado y no hay victoria:
        if (!levelWon && !gameOver && timeLeft <= 0f)
            ShowGameOver();
    }

    private void ShowGameOver()
    {
        gameOver = true;
        if (timerCo != null) { StopCoroutine(timerCo); timerCo = null; }
        if (board) board.SetInteractable(false); // bloquea clicks
        if (gameOverPanel) gameOverPanel.SetActive(true);
        Debug.Log("GAME OVER");

        HandleEndOfMinigame(false);
    }

    private void HandleBoardSolved()
    {
        if (gameOver) return;

        // Sumamos +15 segundos por puzzle completado
        timeLeft += 15f;
        if (timeLeft > startTimeSeconds)
            startTimeSeconds = Mathf.CeilToInt(timeLeft); 

        // Marcamos que hemos resuelto un puzzle
        puzzlesSolved = Mathf.Min(totalPuzzles, puzzlesSolved + 1);
        UpdateHud();

        Debug.Log("+15 segundos por completar el puzzle! Puzzle " + puzzlesSolved + "/" + totalPuzzles);

        // Si hemos completado TODOS los puzzles, victoria final
        if (puzzlesSolved >= totalPuzzles)
        {
            levelWon = true;

            if (timerCo != null) { StopCoroutine(timerCo); timerCo = null; }
            if (board) board.SetInteractable(false);
            if (winPanel) winPanel.SetActive(true);

            HandleEndOfMinigame(true); // victoria final
        }
        else
        {
            // Solo completado un puzzle intermedio:
            // preparamos el siguiente puzzle y seguimos con el mismo contador de tiempo.
            if (board)
            {
                board.ResetBoard();
                board.SetInteractable(true);
            }
        }
    }

    private void HandleEndOfMinigame(bool victory)
    {
        // Mantener tu puntuacion original: 10 puntos por puzzle
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

    private void UpdateHud()
    {
        if (timeText) timeText.text = "Tiempo: " + Mathf.CeilToInt(timeLeft);
        if (puzzleText) puzzleText.text = "Puzzle " + puzzlesSolved + "/" + Mathf.Max(1, totalPuzzles);
    }
}


