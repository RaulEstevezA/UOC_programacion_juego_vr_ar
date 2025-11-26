using UnityEngine;
using TMPro;
using System.Collections;

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

        if (txtGameOver != null)
            txtGameOver.SetActive(true);

        // Obtener puntuación minijuego
        PuntuacionManager pm = FindFirstObjectByType<PuntuacionManager>();
        int puntosParciales = (pm != null) ? pm.ObtenerPuntuacion() : 0;

        // Guardar puntos parciales
        PuntosGlobales.puntosParciales = puntosParciales;

        // --- MODO HISTORIA ---
        if (StoryModeController.Instance != null &&
        StoryModeController.Instance.storyModeActive)
        {
            // Mostrar puntos y esperar antes de cambiar la escena
            StartCoroutine(FinJuegoModoHistoria(puntosParciales));
            return;
        }

        //  MODO LIBRE 
        StartCoroutine(MostrarResultadoTrasPausa(puntosParciales));
        Time.timeScale = 0f; // pausa el juego
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
        // Espera 3 segundos de reloj real aunque el juego esté pausado
        yield return new WaitForSecondsRealtime(3f);

        // Oculta Game Over
        if (txtGameOver != null)
            txtGameOver.SetActive(false);

        // Muestra el texto de resultado con los puntos dinámicos
        if (txtResultadoFinal != null)
        {
            txtResultadoFinal.gameObject.SetActive(true);
            txtResultadoFinal.text = "RESULTADO\nPUNTOS OBTENIDOS: " + puntos;
        }
    }
}