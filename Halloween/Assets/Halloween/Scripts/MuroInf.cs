using UnityEngine;
using TMPro;
using System.Collections;

public class MuroInferior : MonoBehaviour
{
    public GameObject Pelota;
    public GameObject txtGameOver;
    public TMP_Text txtTiempo;
    public float tiempoMaximo = 60f;

    private float tiempoRestante;
    private bool juegoTerminado = false;
    private bool parpadeando = false;

    private void Start()
    {
        tiempoRestante = tiempoMaximo;
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

        if (tiempoRestante <= 10 && !parpadeando)
        {
            parpadeando = true;
            StartCoroutine(ParpadearTexto());
        }

        if (tiempoRestante <= 0)
            FinDelJuego();
    }

    private IEnumerator ParpadearTexto()
    {
        while (!juegoTerminado)
        {
            txtTiempo.enabled = !txtTiempo.enabled;
            txtTiempo.color = Color.red;
            yield return new WaitForSeconds(0.5f);
        }
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
        PuntosGlobales.puntosParciales = puntosParciales;


        // Puedes usar puntosParciales más adelante cuando crees la escena de resultados
        // ResultadoUI.puntosDelJuego = puntosParciales;

        Time.timeScale = 0f; // pausa el juego
    }

}
