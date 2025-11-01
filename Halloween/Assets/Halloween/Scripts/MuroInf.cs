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

        // Obtener la puntuación parcial de este minijuego
        PuntuacionManager pm = PuntuacionManager.FindFirstObjectByType<PuntuacionManager>();

        int puntosParciales = 0;
        if (pm != null)
        {
            puntosParciales = pm.ObtenerPuntuacion(); // guardamos la puntuación parcial
        }
        StartCoroutine(MostrarResultadoTrasPausa(puntosParciales));

        PuntosGlobales.puntosParciales = puntosParciales;


        // Puedes usar puntosParciales más adelante cuando crees la escena de resultados
        // ResultadoUI.puntosDelJuego = puntosParciales;

        Time.timeScale = 0f; // pausa el juego
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
