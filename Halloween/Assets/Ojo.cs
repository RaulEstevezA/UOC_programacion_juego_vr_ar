using UnityEngine;

public class Ojo : MonoBehaviour
{
    public AudioClip impacto;   // Asignar en Inspector
    private static AudioSource audioSourceCompartido;

    private void Start()
    {
        // Crear AudioSource compartido si no existe
        if (audioSourceCompartido == null)
        {
            GameObject go = new GameObject("AudioSourceOjos");
            audioSourceCompartido = go.AddComponent<AudioSource>();
            audioSourceCompartido.spatialBlend = 0f; // 2D
            audioSourceCompartido.volume = 1f;       // Volumen máximo
            DontDestroyOnLoad(go);                    // Opcional: persiste entre escenas
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Pelota") && impacto != null)
        {
            // Reproducir el sonido
            audioSourceCompartido.PlayOneShot(impacto, 1f);

            // Destruir el ojo
            Destroy(gameObject);
        }
    }
}
