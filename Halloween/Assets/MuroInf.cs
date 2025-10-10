using UnityEngine;
using System.Collections;

public class MuroInferior : MonoBehaviour
{
    public int vidasRestantes = 2; // solo las bolas que están "de reserva"
    public GameObject Pelota;
    private Vector3 spawnPosicionInicial;
    private bool reiniciando = false;

    private void Start()
    {
        spawnPosicionInicial = Pelota.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Pelota") || reiniciando) return;

        if (vidasRestantes > 0)
        {
            reiniciando = true;

            // Oculta la vida que queda
            OcultarIcono(vidasRestantes);

            // Resta una vida de reserva
            vidasRestantes--;

            StartCoroutine(ReiniciarPelota());
        }
        else
        {
            // Última bola perdida
            Debug.Log("Game Over");
            Pelota.SetActive(false);
        }
    }

    private void OcultarIcono(int vidaActual)
    {
        switch (vidaActual)
        {
            case 2:
                GameObject vida1 = GameObject.FindGameObjectWithTag("Vida1");
                if (vida1 != null) vida1.SetActive(false);
                break;
            case 1:
                GameObject vida2 = GameObject.FindGameObjectWithTag("Vida2");
                if (vida2 != null) vida2.SetActive(false);
                break;
        }
    }

    private IEnumerator ReiniciarPelota()
    {
        Pelota.SetActive(false);
        yield return new WaitForSeconds(3f);

        Pelota.transform.position = spawnPosicionInicial;
        Pelota.SetActive(true);

        Rigidbody2D rb = Pelota.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(1, 1).normalized * 10f, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(0.5f);
        reiniciando = false;
    }
}
