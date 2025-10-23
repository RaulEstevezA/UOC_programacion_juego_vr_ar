using UnityEngine;
using TMPro;

public class MuroInferior : MonoBehaviour
{
    public GameObject Pelota;
    public GameObject txtGameOver;
    public TMP_Text txtTiempo;
    public float tiempoMaximo = 60f;

    private float tiempoRestante;
    private bool juegoTerminado = false;
    private Vector3 spawnPosicionInicial;

    private void Start()
    {
        tiempoRestante = tiempoMaximo;
        spawnPosicionInicial = Pelota.transform.position;

        if (txtGameOver != null)
            txtGameOver.SetActive(false);

        if (txtTiempo != null)
            txtTiempo.text = Mathf.Ceil(tiempoRestante).ToString();
    }

    private void Update()
    {
        if (juegoTerminado) return;

        tiempoRestante -= Time.unscaledDeltaTime;

        if (txtTiempo != null)
            txtTiempo.text = Mathf.Ceil(tiempoRestante).ToString();

        if (tiempoRestante <= 0)
            JuegoTerminado("Se acabó el tiempo");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (juegoTerminado) return;

        if (other.CompareTag("Pelota"))
        {
            JuegoTerminado("Pelota perdida");
        }
    }

    private void JuegoTerminado(string motivo)
    {
        juegoTerminado = true;
        Pelota.SetActive(false);

        if (txtGameOver != null)
            txtGameOver.SetActive(true);

        Time.timeScale = 0f;
        Debug.Log("Game Over: " + motivo);
    }
}
