using UnityEngine;

public class MoveTowardsPlayer : MonoBehaviour
{
    public float speed = 6f;
    public float destroyZ = -3f; // destruye cuando pasa al jugador
    public int damage = 1;
    public int scoreOnPass = 1; // puntos si no te da y te pasa

    private bool scored = false;

    void Update()
    {
        transform.Translate(0f, 0f, -speed * Time.deltaTime, Space.World);

        if (!scored && transform.position.z <= 0f)
        {
            // Ya pasó la línea del jugador sin chocar
            scored = true;
            MicrogameManager.Instance?.AddScore(scoreOnPass);
        }

        if (transform.position.z < destroyZ)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHitbox"))
        {
            MicrogameManager.Instance?.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
