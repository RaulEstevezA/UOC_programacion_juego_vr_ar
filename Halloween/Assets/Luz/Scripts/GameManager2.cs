using UnityEngine;
using TMPro; // si usas TextMeshPro

public class GameManager2 : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private BoardController board;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private GameObject winPanel;

    [Header("Reglas")]
    [SerializeField] private int puzzlesParaGanar = 3;
    [SerializeField] private float tiempoInicial = 60f;
    [SerializeField] private float bonusTiempoPorPuzzle = 0f; // pon 5f si quieres recompensa

    private float tiempoRestante;
    private int puzzlesCompletados;
    private bool juegoTerminado;

    void Awake()
    {
        if (board != null)
            board.OnBoardSolved += OnBoardSolved;
    }

    void Start()
    {
        tiempoRestante = tiempoInicial;
        puzzlesCompletados = 0;
        juegoTerminado = false;

        ActualizarHUD();

        if (winPanel) winPanel.SetActive(false);
    }

    void OnDestroy()
    {
        if (board != null)
            board.OnBoardSolved -= OnBoardSolved;
    }

    void Update()
    {
        if (juegoTerminado) return;

        // Cuenta atrás
        tiempoRestante -= Time.deltaTime;
        if (tiempoRestante < 0f)
        {
            tiempoRestante = 0f;
            FinDeJuego(false);
        }

        ActualizarTimerVisual();
    }

    private void OnBoardSolved()
    {
        if (juegoTerminado) return;

        puzzlesCompletados++;
        if (bonusTiempoPorPuzzle > 0f)
            tiempoRestante += bonusTiempoPorPuzzle;

        if (puzzlesCompletados >= puzzlesParaGanar)
        {
            FinDeJuego(true);
        }
        else
        {
            // Siguiente puzzle
            board.BarajarPatronSoluble();
            ActualizarHUD();
        }
    }

    private void FinDeJuego(bool victoria)
    {
        juegoTerminado = true;

        if (winPanel)
            winPanel.SetActive(victoria);

        // (Opcional) desactivar interacción del tablero
        // foreach (var b in board.GetComponentsInChildren<Button>(true)) b.interactable = false;

        ActualizarHUD();
    }

    private void ActualizarHUD()
    {
        if (progressText)
            progressText.text = $"Puzzle {puzzlesCompletados}/{puzzlesParaGanar}";

        ActualizarTimerVisual();
    }

    private void ActualizarTimerVisual()
    {
        if (!timerText) return;

        int t = Mathf.CeilToInt(tiempoRestante);
        timerText.text = $"Tiempo: {t}";

        // Cambiar color cuando quede poco tiempo (<=10s)
        if (t <= 10) timerText.color = new Color(1f, 0.3f, 0.3f, 1f);
        else timerText.color = Color.white;
    }
}

