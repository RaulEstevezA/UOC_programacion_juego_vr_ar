using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ARGameController : MonoBehaviour
{
    [Header("AR")]
    public XROrigin xrOrigin;
    public Camera arCamera;

    [Header("Gameplay")]
    public GameObject tioPrefab;     // Lo tenemos en la escena, pero el Tió real nos vendrá del ARPlaneSpawner
    private GameObject tioInstance;

    [Header("UI")]
    public GameObject panelDifuminado;
    public GameObject panelHUD;
    public Button botonEmpezar;

    [Header("Golpes y regalos")]
    public int golpesPorRegalo = 3;
    public Text textoGolpes;
    public Text textoRegalos;
    public GameObject regaloPrefab;

    private bool juegoIniciado = false;
    private int golpesActuales = 0;
    private int totalRegalos = 0;

    void Start()
    {
        // Estado inicial UI
        if (panelHUD != null)
            panelHUD.SetActive(false);

        if (panelDifuminado != null)
            panelDifuminado.SetActive(true);

        if (textoGolpes != null)
            textoGolpes.text = "Golpes: 0";

        if (textoRegalos != null)
            textoRegalos.text = "Regalos: 0";

        // Conectar botón
        if (botonEmpezar != null)
            botonEmpezar.onClick.AddListener(IniciarJuego);
    }

    void IniciarJuego()
    {
        juegoIniciado = true;

        if (panelDifuminado != null)
            panelDifuminado.SetActive(false);

        if (panelHUD != null)
            panelHUD.SetActive(true);
    }

    void Update()
    {
        if (!juegoIniciado)
            return;

#if UNITY_EDITOR
        // Editor: ratón
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 posPantalla = Mouse.current.position.ReadValue();
            HandleTouch(posPantalla);
        }
#else
    // Móvil: pantalla táctil (nuevo Input System)
    if (Touchscreen.current != null)
    {
        var touch = Touchscreen.current.primaryTouch;
        if (touch.press.wasPressedThisFrame)
        {
            Vector2 posPantalla = touch.position.ReadValue();
            HandleTouch(posPantalla);
        }
    }
#endif
    }


    void HandleTouch(Vector2 screenPosition)
    {
        // Asegurarnos de que ya tenemos referencia al Tió
        if (tioInstance == null)
        {
            tioInstance = ARPlaneSpawner.CurrentTio;
            if (tioInstance == null)
                return; // Todavía no se ha colocado el Tió
        }

        if (arCamera == null)
            return;

        Ray ray = arCamera.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider == null) return;

            // ¿Hemos golpeado al Tió (o a algún hijo suyo)?
            if (hit.collider.gameObject == tioInstance ||
                hit.collider.transform.IsChildOf(tioInstance.transform))
            {
                OnGolpeTio();
            }
        }
    }

    void OnGolpeTio()
    {
        golpesActuales++;

        if (textoGolpes != null)
            textoGolpes.text = "Golpes: " + golpesActuales;

        if (golpesActuales >= golpesPorRegalo)
        {
            golpesActuales = 0;
            totalRegalos++;

            if (textoRegalos != null)
                textoRegalos.text = "Regalos: " + totalRegalos;

            SpawnRegalo();
        }
    }

    void SpawnRegalo()
    {
        if (regaloPrefab == null || tioInstance == null)
            return;

        Vector3 basePos = tioInstance.transform.position;
        Vector2 random2D = Random.insideUnitCircle.normalized * 0.4f;

        Vector3 spawnPos = new Vector3(
            basePos.x + random2D.x,
            basePos.y,
            basePos.z + random2D.y
        );

        Instantiate(regaloPrefab, spawnPos, Quaternion.identity);
    }
}
