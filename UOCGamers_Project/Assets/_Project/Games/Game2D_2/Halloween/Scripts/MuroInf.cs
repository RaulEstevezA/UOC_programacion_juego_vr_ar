using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;


public class MuroInferior : MonoBehaviour
{
    public GameObject Pelota;
    public GameObject txtGameOver;
    public TMP_Text txtTiempo;
    public TMP_Text txtResultadoFinal;
    public float tiempoMaximo = 60f;

    private float tiempoRestante;
    private bool juegoTerminado = false;
    private bool parpadeando = false;
    [Header("Botones modo libre")]
    [SerializeField] private GameObject freeModeButtonsRoot;
    [SerializeField] private string menuSceneName = "Menu";


    private void Start()
    {
        tiempoRestante = tiempoMaximo;
        if (txtResultadoFinal != null)
            txtResultadoFinal.gameObject.SetActive(false);
        if (txtGameOver != null)
            txtGameOver.SetActive(false);
        if (txtTiempo != null)
            txtTiempo.text = "Tiempo restante: " + Mathf.Ceil(tiempoRestante);
        
    }

    private void Update()
    {
        if (juegoTerminado) return;

        tiempoRestante -= Time.deltaTime;

        if (txtTiempo != null)
            txtTiempo.text = "Tiempo restante: " + Mathf.Ceil(tiempoRestante);

        if (tiempoRestante <= 30 && !parpadeando)
        {
            parpadeando = true;
            StartCoroutine(ParpadearTexto());
        }

        if (tiempoRestante <= 0)
            FinDelJuego();
    }

    private IEnumerator ParpadearTexto()
    {
        Color[] colores = { Color.red, Color.yellow, Color.magenta, Color.cyan, Color.white };
        int indice = 0;

        while (!juegoTerminado)
        {
            txtTiempo.enabled = !txtTiempo.enabled; // enciende/apaga el texto
            if (txtTiempo.enabled)
            {
                txtTiempo.color = colores[indice];   // cambia el color
                indice = (indice + 1) % colores.Length; // siguiente color
            }

            yield return new WaitForSeconds(0.5f);
        }

        // Asegurarse de que el texto quede visible al final
        txtTiempo.enabled = true;
        txtTiempo.color = Color.red;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Pelota") || juegoTerminado) return;
        FinDelJuego();
    }

    private void FinDelJuego()
    {
        juegoTerminado = true;
        Pelota.SetActive(false);

        // 1. Obtenemos puntos
        PuntuacionManager pm = FindFirstObjectByType<PuntuacionManager>();
        int puntosParciales = (pm != null) ? pm.ObtenerPuntuacion() : 0;
        PuntosGlobales.puntosParciales = puntosParciales;

        // 2. DETECCIÓN DE MODO (Con Log de control)
        if (StoryModeController.Instance != null && StoryModeController.Instance.storyModeActive)
        {
            Debug.Log("<color=cyan>MODO DETECTADO: HISTORIA</color>");

            if (txtGameOver != null) txtGameOver.SetActive(true);
            StartCoroutine(FinJuegoModoHistoria(puntosParciales));
        }
        else
        {
            Debug.Log("<color=yellow>MODO DETECTADO: LIBRE (Activando secuencia de botones)</color>");

            // Limpiamos cualquier rastro visual previo
            if (txtGameOver != null) txtGameOver.SetActive(true); // Se apaga luego en la corrutina

            // Iniciamos la corrutina que mostrará los botones
            StartCoroutine(MostrarResultadoTrasPausa(puntosParciales));
        }
    }

    private IEnumerator FinJuegoModoHistoria(int puntos)
    {
        // ocultar el "GAME OVER" y mostrar solo el resultado
        if (txtGameOver != null)
            txtGameOver.SetActive(false);

        if (txtResultadoFinal != null)
        {
            txtResultadoFinal.gameObject.SetActive(true);
            txtResultadoFinal.text = "RESULTADO\nPUNTOS OBTENIDOS: " + puntos;
        }

        // Esperamos X segundos para que el jugador vea los puntos
        yield return new WaitForSecondsRealtime(3f); // ajusta 3f a lo que te parezca mejor

        // StoryModeController suma puntuación, avanza de paso y carga ModoHistoria
        StoryModeController.Instance.OnMiniGameFinished(puntos);
    }

    private IEnumerator MostrarResultadoTrasPausa(int puntos)
    {
        yield return new WaitForSecondsRealtime(2f);

        if (txtGameOver != null) txtGameOver.SetActive(false);

        if (txtResultadoFinal != null)
        {
            txtResultadoFinal.gameObject.SetActive(true);
            txtResultadoFinal.text = "RESULTADO\nPUNTOS OBTENIDOS: " + puntos;
        }

        yield return new WaitForSecondsRealtime(3f);

        if (txtResultadoFinal != null) txtResultadoFinal.gameObject.SetActive(false);

        if (freeModeButtonsRoot)
        {
            freeModeButtonsRoot.SetActive(true);
            freeModeButtonsRoot.transform.SetAsLastSibling();
            Debug.Log("Botones activados y movidos al frente.");
        }


        // Esperamos un frame para que Unity los pinte antes de pausar
        yield return null;
        
        Time.timeScale = 0.0001f;
    }
    public void Reintentar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

}