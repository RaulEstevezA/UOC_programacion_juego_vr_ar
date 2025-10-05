using UnityEngine;

public class Pelota : MonoBehaviour
{
    public float velocidad = 5f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;

        // La pelota empieza bajando con un pequeño ángulo aleatorio
        float anguloX = Random.Range(-2f, 2f);
        rb.velocity = new Vector2(anguloX, -velocidad);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 🔹 Rebote con el paddle (player)
        if (collision.gameObject.CompareTag("player"))
        {
            // Calcula dónde ha golpeado (izquierda o derecha del paddle)
            float puntoImpacto = transform.position.x - collision.transform.position.x;
            float anchoPaddle = collision.collider.bounds.size.x / 2;

            // Factor entre -1 (izquierda) y 1 (derecha)
            float direccionX = puntoImpacto / anchoPaddle;

            // Ajusta la dirección de rebote
            rb.velocity = new Vector2(direccionX * velocidad, velocidad);
        }
        // 🔹 Rebote con muros laterales
        else if (collision.gameObject.CompareTag("MuroIzq") || collision.gameObject.CompareTag("MuroDer"))
        {
            rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
        }
        // 🔹 Rebote con muro superior
        else if (collision.gameObject.CompareTag("Murosup"))
        {
            rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
        }
    }
}
