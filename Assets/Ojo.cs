using UnityEngine;

public class Ojo : MonoBehaviour
{
    public AudioClip impacto; // Arrastra impacto.mp3 desde Assets

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Comprobar si el objeto que colisiona tiene el tag "Pelota"
        if (collision.gameObject.CompareTag("Pelota"))
        {
            // Reproducir sonido en la posición del ojo
            if (impacto != null)
            {
                AudioSource.PlayClipAtPoint(impacto, transform.position);
            }

            // Destruir el ojo
            Destroy(gameObject);
        }
    }
}
