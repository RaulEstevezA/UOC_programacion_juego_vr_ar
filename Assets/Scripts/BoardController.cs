using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Button resetButton;

    [Header("Ajustes de mezcla")]
    [Tooltip("Número de 'toques aleatorios' que se aplican para generar un patrón siempre soluble.")]
    [SerializeField, Range(1, 50)] private int randomMoves = 10;

    [Tooltip("Mezclar automáticamente al iniciar la escena.")]
    [SerializeField] private bool mezclarAlIniciar = true;

    private readonly List<Candle> velas = new List<Candle>();
    private System.Random rng;

    // Evento cuando TODO está encendido
    public event Action OnBoardSolved;

    private const int COLS = 3;
    private const int ROWS = 3;

    void Awake()
    {
        // 1) Recoger las 9 velas en orden de hijos (grid)
        velas.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            var c = transform.GetChild(i).GetComponent<Candle>();
            if (c != null) velas.Add(c);
        }

        // 2) Suscribirnos a los toggles de cada vela
        foreach (var v in velas)
            v.OnToggled.AddListener(OnCandleToggled);

        // 3) Botón reiniciar
        if (resetButton != null)
            resetButton.onClick.AddListener(BarajarPatronSoluble);

        rng = new System.Random();
    }

    void Start()
    {
        if (mezclarAlIniciar) BarajarPatronSoluble();
    }

    private void OnDestroy()
    {
        foreach (var v in velas)
            v.OnToggled.RemoveListener(OnCandleToggled);

        if (resetButton != null)
            resetButton.onClick.RemoveListener(BarajarPatronSoluble);
    }

    /// <summary>
    /// Genera un patrón SIEMPRE SOLUBLE: empieza del tablero resuelto y aplica 'randomMoves' toques aleatorios.
    /// </summary>
    public void BarajarPatronSoluble()
    {
        // Poner TODO ENCENDIDO (estado objetivo)
        for (int i = 0; i < velas.Count; i++)
            velas[i].SetState(true, true);

        // Aplicar N movimientos aleatorios
        for (int m = 0; m < randomMoves; m++)
        {
            int idx = rng.Next(0, velas.Count);
            ApplyMove(idx); // esto simula un toque real: alterna esa vela y vecinas
        }

        // Evitar quedar resuelto por casualidad (muy raro, pero por si acaso)
        if (EstanTodasEncendidas())
        {
            int idx = rng.Next(0, velas.Count);
            ApplyMove(idx);
        }
    }

    /// <summary>
    /// Llamado cuando el jugador TOCA una vela (Toggle lanzó el evento).
    /// </summary>
    private void OnCandleToggled(Candle c, bool isOn)
    {
        int idx = velas.IndexOf(c);
        if (idx < 0) return;

        // Al tocar: alternar VECINAS (la propia ya la alternó Candle.Toggle())
        ToggleNeighbors(idx);

        // Comprobar victoria
        if (EstanTodasEncendidas())
            OnBoardSolved?.Invoke();
    }

    /// <summary>
    /// Aplica un movimiento como si el jugador tocara la casilla 'idx':
    /// alterna esa vela y sus vecinas (sin disparar eventos).
    /// </summary>
    private void ApplyMove(int idx)
    {
        // 1) Alternar la propia
        var self = velas[idx];
        self.SetState(!self.IsOn, true);

        // 2) Alternar vecinas
        foreach (int n in GetNeighbors(idx))
        {
            var v = velas[n];
            v.SetState(!v.IsOn, true);
        }
    }

    /// <summary>
    /// Alterna vecinas cuando el jugador ha tocado una casilla real.
    /// (Usa SetState para no disparar eventos de vuelta).
    /// </summary>
    private void ToggleNeighbors(int idx)
    {
        foreach (int n in GetNeighbors(idx))
        {
            var v = velas[n];
            v.SetState(!v.IsOn, false);
        }
    }

    /// <summary>
    /// Devuelve índices vecinos ortogonales (arriba/abajo/izq/dcha) para un índice 0..8.
    /// </summary>
    private IEnumerable<int> GetNeighbors(int idx)
    {
        int row = idx / COLS;
        int col = idx % COLS;

        // Arriba
        if (row > 0) yield return (row - 1) * COLS + col;
        // Abajo
        if (row < ROWS - 1) yield return (row + 1) * COLS + col;
        // Izquierda
        if (col > 0) yield return row * COLS + (col - 1);
        // Derecha
        if (col < COLS - 1) yield return row * COLS + (col + 1);
    }

    public bool EstanTodasEncendidas()
    {
        foreach (var v in velas)
            if (!v.IsOn) return false;
        return true;
    }
}



