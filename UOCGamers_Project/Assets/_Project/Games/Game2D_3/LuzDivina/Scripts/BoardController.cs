using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardController : MonoBehaviour
{
    public event System.Action OnBoardSolved;

    [Header("Referencias")]
    [SerializeField] private Button resetButton;

    [Header("Grid")]
    [SerializeField] private int rows = 3;
    [SerializeField] private int cols = 3;

    [Header("Velas ordenadas MANUALMENTE")]
    [Tooltip("Rellenar en orden: fila 1 izq-der, luego fila 2, luego fila 3")]
    [SerializeField] private Candle[] candleSlots = new Candle[9];

    [Header("Configuracion inicial")]
    [SerializeField, Tooltip("Numero de clicks aleatorios al empezar")]
    private int randomClicksAtStart = 5;

    private List<Candle> candles = new List<Candle>();
    private Candle[,] grid;
    private Dictionary<Candle, Vector2Int> candleToPos = new Dictionary<Candle, Vector2Int>();

    private bool solved = false;

    private void Awake()
    {
        if (resetButton)
            resetButton.onClick.AddListener(ResetBoard);

        Debug.Log("[Board] Awake en " + name);
    }

    private void Start()
    {
        CacheCandlesFromSlots();
        ResetBoard();
    }

    private void CacheCandlesFromSlots()
    {
        candles.Clear();
        grid = new Candle[rows, cols];
        candleToPos.Clear();

        int expected = rows * cols;
        if (candleSlots == null || candleSlots.Length != expected)
        {
            Debug.LogError("[Board] Numero incorrecto de referencias en candleSlots.");
            return;
        }

        for (int i = 0; i < expected; i++)
        {
            var candle = candleSlots[i];
            if (!candle)
            {
                Debug.LogError("[Board] candleSlots[" + i + "] esta vacio.");
                continue;
            }

            candles.Add(candle);

            int r = i / cols;
            int c = i % cols;

            grid[r, c] = candle;
            candleToPos[candle] = new Vector2Int(r, c);

            candle.OnToggled.RemoveAllListeners();
            candle.OnToggled.AddListener(HandleCandleToggled);
        }

        Debug.Log("[Board] CacheCandlesFromSlots: " + candles.Count + " velas mapeadas.");
    }

    private void HandleCandleToggled(Candle candle, bool isOn)
    {
        if (solved) return;
        if (!candleToPos.TryGetValue(candle, out var pos)) return;

        int r = pos.x;
        int c = pos.y;

        Debug.Log("[Board] CLICK logico en (" + (r + 1) + "," + (c + 1) + ") para " + candle.name);

        ToggleAt(r - 1, c, false);
        ToggleAt(r + 1, c, false);
        ToggleAt(r, c - 1, false);
        ToggleAt(r, c + 1, false);

        CheckSolved();
    }

    private void ToggleAt(int r, int c, bool instant)
    {
        if (r < 0 || r >= rows || c < 0 || c >= cols) return;
        var candle = grid[r, c];
        if (candle == null) return;

        Debug.Log("   -> toggle (" + (r + 1) + "," + (c + 1) + ")");

        candle.SetState(!candle.IsOn, instant);
    }

    private void ApplyMove(int r, int c, bool instant)
    {
        ToggleAt(r, c, instant);
        ToggleAt(r - 1, c, instant);
        ToggleAt(r + 1, c, instant);
        ToggleAt(r, c - 1, instant);
        ToggleAt(r, c + 1, instant);
    }

    public void ResetBoard()
    {
        solved = false;

        if (candles.Count == 0 || grid == null)
            CacheCandlesFromSlots();

        foreach (var c in candles)
            c.SetState(false, true);

        int moves = Mathf.Max(0, randomClicksAtStart);
        for (int i = 0; i < moves; i++)
        {
            int r = Random.Range(0, rows);
            int c = Random.Range(0, cols);
            ApplyMove(r, c, true);
        }

        Debug.Log("[Board] ResetBoard completado.");
    }

    private void Update()
    {
        if (!solved)
            CheckSolved();
    }

    private void CheckSolved()
    {
        if (candles.Count == 0) return;

        foreach (var c in candles)
        {
            if (c.IsOn)
                return;
        }

        solved = true;
        Debug.Log("[Board] Puzzle resuelto (todas apagadas)");
        OnBoardSolved?.Invoke();
    }

    public void SetInteractable(bool enabled)
    {
        var buttons = GetComponentsInChildren<Button>(true);
        foreach (var b in buttons)
            b.interactable = enabled;
    }

    public void SetInteractactable(bool enabled)
    {
        SetInteractable(enabled);
    }
}
