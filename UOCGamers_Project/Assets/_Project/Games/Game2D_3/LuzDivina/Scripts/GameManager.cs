using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Referencias de escena")]
    [SerializeField] private BoardController board;      // arrastra el Board
    [SerializeField] private Button resetButton;         // arrastra "Reiniciar"
    [SerializeField] private TextMeshProUGUI timeText;   // "Tiempo: 120"
    [SerializeField] private TextMeshProUGUI puzzleText; // "Puzzle 0/3"
    [SerializeField] private GameObject winPanel;        // opcional
    [SerializeField] private GameObject gameOverPanel;   // <-- NUEVO: arrastra el panel

    [Header("Lógica de juego")]
    [SerializeField] private int startTimeSeconds = 120;
    [SerializeField] private int totalPuzzles = 3;

    private int puzzlesSolved = 0;
    private float timeLeft;
    private Coroutine timerCo;
    private bool levelWon;
    private bool gameOver;

    private void Awake()
    {
        if (resetButton) { resetButton.onClick.RemoveAllListeners(); resetButton.onClick.AddListener(OnPressReset); }
        if (board) board.OnBoardSolved += HandleBoardSolved;
    }
    private void OnDestroy()
    {
        if (board) board.OnBoardSolved -= HandleBoardSolved;
    }

    private void Start() => StartLevel();

    private void StartLevel()
    {
        levelWon = false;
        gameOver = false;

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
    }

    private void HandleBoardSolved()
    {
        if (levelWon || gameOver) return;
        levelWon = true;

        timeLeft += 15f;
        if (timeLeft > startTimeSeconds)
            startTimeSeconds = Mathf.CeilToInt(timeLeft); // actualiza valor base si reinicias

        if (timerCo != null) StopCoroutine(timerCo);
        timerCo = StartCoroutine(TimerTick());

        if (board) board.SetInteractable(false);
        if (winPanel) winPanel.SetActive(true);

        puzzlesSolved = Mathf.Min(totalPuzzles, puzzlesSolved + 1);
        UpdateHud();

        Debug.Log("+15 segundos por completar el puzzle!");
    }

    private void OnPressReset() => StartLevel();

    private void UpdateHud()
    {
        if (timeText) timeText.text = $"Tiempo: {Mathf.CeilToInt(timeLeft)}";
        if (puzzleText) puzzleText.text = $"Puzzle {puzzlesSolved}/{Mathf.Max(1, totalPuzzles)}";
    }
}

