using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardController : MonoBehaviour
{
    public event System.Action OnBoardSolved;

    [Header("Referencias")]
    [SerializeField] private Button resetButton;        // Botón "Reiniciar"
    [SerializeField] private Transform gridParent;      // Contenedor donde se instancian las velas
    [SerializeField] private GameObject candlePrefab;   // Prefab de la vela

    [Header("Tamaño del tablero")]
    [SerializeField] private int rows = 3;
    [SerializeField] private int cols = 3;

    [Header("Ajustes de mezcla")]
    [SerializeField, Tooltip("Número de movimientos aleatorios al iniciar")]
    private int randomMoves = 0;
    [SerializeField] private bool mezclarAlIniciar = true;

    private readonly List<Candle> candles = new List<Candle>();

    // ----------------------------------------------------------------------

    private void Awake()
    {
        if (!gridParent) gridParent = transform; // Si no se asigna, usa este mismo objeto (Board)

        if (resetButton != null)
            resetButton.onClick.AddListener(ResetBoard);
    }

    private void Start()
    {
        BuildGrid();

        if (mezclarAlIniciar && randomMoves > 0)
            Shuffle(randomMoves);
    }

    // ----------------------------------------------------------------------

    private void BuildGrid()
    {
        // Limpia los hijos antiguos
        candles.Clear();

        for (int i = gridParent.childCount - 1; i >= 0; i--)
            Destroy(gridParent.GetChild(i).gameObject);

        if (!candlePrefab)
        {
            Debug.LogError("Falta asignar Candle Prefab en BoardController.");
            return;
        }

        int total = Mathf.Max(1, rows) * Mathf.Max(1, cols);

        for (int i = 0; i < total; i++)
        {
            GameObject go = Instantiate(candlePrefab, gridParent);
            Candle c = go.GetComponent<Candle>();

            if (c == null)
            {
                Debug.LogError("El prefab no tiene componente Candle.");
                continue;
            }

            c.SetState(false, true); // Empieza apagada

            // Cuando el usuario pulsa esta vela:
            c.OnToggled?.AddListener((who, _) => OnUserToggled(who));

            candles.Add(c);
        }
    }

    // ----------------------------------------------------------------------

    private void Shuffle(int moves)
    {
        if (candles.Count == 0) return;
        moves = Mathf.Max(0, moves);

        for (int i = 0; i < moves; i++)
        {
            int idx = Random.Range(0, candles.Count);

            // Simula un toque real: cambia la propia y vecinas
            candles[idx].Toggle();     // propia 
            ToggleNeighborsOf(idx);    // vecinas
        }
        CheckSolved();
    }


    public void ResetBoard()
    {
        foreach (var c in candles)
            c.SetState(false, true);

        if (mezclarAlIniciar && randomMoves > 0)
            Shuffle(randomMoves);
    }
    // Se llama cuando el usuario pulsa una vela 'who'
    private void OnUserToggled(Candle who)
    {
        int idx = candles.IndexOf(who);
        if (idx < 0) return;

        ToggleNeighborsOf(idx); // cambia vecinas SIN disparar eventos recursivos
        CheckSolved();
    }

    // Cambia arriba/abajo/izquierda/derecha del índice dado
    private void ToggleNeighborsOf(int idx)
    {
        int r = idx / cols;
        int c = idx % cols;

        ToggleNeighbor(r - 1, c); // up
        ToggleNeighbor(r + 1, c); // down
        ToggleNeighbor(r, c - 1); // left
        ToggleNeighbor(r, c + 1); // right
    }

    // Cambia una celda concreta si existe (sin emitir OnToggled para evitar cascadas)
    private void ToggleNeighbor(int row, int col)
    {
        if (row < 0 || col < 0 || row >= rows || col >= cols) return;

        int i = row * cols + col;
        var n = candles[i];
        n.SetState(!n.IsOn, false);  // cambia estado sin invocar OnToggled
    }

    // Comprueba si TODAS están encendidas
    private void CheckSolved()
    {
        if (candles.Count == 0) return;
        for (int i = 0; i < candles.Count; i++)
            if (!candles[i].IsOn) return;

        // ¡victoria!
        OnBoardSolved?.Invoke();
    }

    public void SetInteractable(bool enabled)
    {
        // Desactiva/activa todos los botones de las velas
        var buttons = gridParent ? gridParent.GetComponentsInChildren<Button>(true)
                                 : GetComponentsInChildren<Button>(true);
        foreach (var b in buttons) b.interactable = enabled;
    }



}
