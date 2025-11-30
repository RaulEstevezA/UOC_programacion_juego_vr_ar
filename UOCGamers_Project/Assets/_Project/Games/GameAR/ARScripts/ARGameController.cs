using UnityEngine;
using UnityEngine.UI;
using Unity.XR.CoreUtils;

public class ARGameController : MonoBehaviour
{
    [Header("AR")]
    public XROrigin xrOrigin;
    public Camera arCamera;

    [Header("Gameplay")]
    public GameObject tioPrefab;
    private GameObject tioInstance;

    [Header("UI")]
    public GameObject panelDifuminado;
    public GameObject panelHUD;
    public Button botonEmpezar;

    private bool juegoIniciado = false;

    void Start()
    {
        // UI inicial
        panelHUD.SetActive(false);         
        panelDifuminado.SetActive(true);   

        // Conectar botón
        botonEmpezar.onClick.AddListener(IniciarJuego);
    }

    void IniciarJuego()
    {
        juegoIniciado = true;

        // UI
        panelDifuminado.SetActive(false);
        panelHUD.SetActive(true);

    }
}
