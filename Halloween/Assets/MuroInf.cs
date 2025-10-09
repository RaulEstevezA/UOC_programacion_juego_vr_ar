using UnityEngine;

public class MuroInferior : MonoBehaviour
{
    public int vidas = 2; // número de vidas
    public GameObject Pelota; // la pelota que ya existe en la escena
    public Transform spawnPoint; // posición donde reaparecerá la pelota

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pelota"))
        {
            // Resta una vida
            vidas--;

            // Oculta la calabaza correspondiente
            if (vidas == 1)
            {
                GameObject vida1 = GameObject.FindGameObjectWithTag("Vida1");
                if (vida1 != null) vida1.SetActive(false);
            }
            else if (vidas == 0)
            {
                GameObject vida2 = GameObject.FindGameObjectWithTag("Vida2");
                if (vida2 != null) vida2.SetActive(false);
                Debug.Log("Game Over");
            }

            // Reinicia la pelota en la posición de spawn
            Pelota.transform.position = spawnPoint.position;
            // Opcional: reinicia la velocidad de la pelota si usas Rigidbody2D
            Rigidbody2D rb = Pelota.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero; // detiene la pelota
                rb.AddForce(new Vector2(1, 1).normalized * 10f, ForceMode2D.Impulse); // lanza la pelota otra vez
            }
        }
    }
}
