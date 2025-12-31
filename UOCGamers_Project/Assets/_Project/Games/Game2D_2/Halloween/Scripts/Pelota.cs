using UnityEngine;

public class Pelota : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidad = 5f;        // Velocidad constante de la pelota
    public float velocidadMaxima = 7f;  // Límite superior por seguridad

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // evita rebotes raros

        // La pelota empieza bajando con un ángulo aleatorio
        float anguloX = Random.Range(-2f, 2f);
        rb.velocity = new Vector2(anguloX, -velocidad);
    }

    void FixedUpdate()
    {
        // 🔹 Mantiene la velocidad constante
        if (rb.velocity.magnitude > velocidadMaxima)
        {
            rb.velocity = rb.velocity.normalized * velocidadMaxima;
        }
        else if (rb.velocity.magnitude < velocidad)
        {
            rb.velocity = rb.velocity.normalized * velocidad;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // --- Rebote con el paddle ---
        if (collision.gameObject.CompareTag("player"))
        {
            float puntoImpacto = transform.position.x - collision.transform.position.x;
            float anchoPaddle = collision.collider.bounds.size.x / 2;
            float direccionX = puntoImpacto / anchoPaddle;

            // Nuevo vector de dirección (normalizado)
            Vector2 nuevaDireccion = new Vector2(direccionX, 1).normalized;
            rb.velocity = nuevaDireccion * velocidad;
        }

        // --- Rebote con muros laterales ---
        else if (collision.gameObject.CompareTag("MuroIzq") || collision.gameObject.CompareTag("MuroDer"))
        {
            rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y).normalized * velocidad;
        }

        // --- Rebote con muro superior ---
        else if (collision.gameObject.CompareTag("Murosup"))
        {
            rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y).normalized * velocidad;
        }
    }
}
