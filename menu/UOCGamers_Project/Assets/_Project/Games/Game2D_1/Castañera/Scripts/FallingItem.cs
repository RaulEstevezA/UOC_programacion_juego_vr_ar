using UnityEngine;

public class FallingItem : MonoBehaviour
{
    [SerializeField] private float baseFallSpeed = 5f;   // antes: fallSpeed
    [SerializeField] private string chestnutTag = "Chestnut";
    [SerializeField] private string rockTag = "Rock";
    [SerializeField] private string headTag = "PlayerHead";
    [SerializeField] private string groundTag = "Ground";
    [SerializeField] private AudioSource sfxSource;      // Fuente de sonido (Managers)
    [SerializeField] private AudioClip chestnutSfxClip;  // Sonido al recoger castaña
    [SerializeField] private AudioClip rockHitSfxClip;   // Sonido al golpear con una roca

    private float currentFallSpeed;
    private float xKill;
    private string myTag;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        xKill = cam.ViewportToWorldPoint(new Vector3(0.5f, -0.1f, 0f)).y;

        myTag = gameObject.tag;
        currentFallSpeed = baseFallSpeed; // por defecto, 1x

        if (sfxSource == null)
        {
            var manager = UnityEngine.Object.FindFirstObjectByType<CastanyeraGameManager>();
            if (manager != null)
            {
                sfxSource = manager.GetComponent<AudioSource>();
            }

            if (sfxSource == null)
            {
                Debug.LogWarning("[FallingItem] No se encontro AudioForce Managers; El sonido de castaña no se reproducirá.");
            }
        }
    }

    void Update()
    {
        if (CastanyeraGameManager.Instance != null && CastanyeraGameManager.Instance.IsGameOver) return;

        transform.position += Vector3.down * currentFallSpeed * Time.deltaTime;

        if (transform.position.y < xKill)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (CastanyeraGameManager.Instance == null) return;

        // Solo cuenta si toca la CABEZA del jugador
        if (other.CompareTag(headTag))
        {
            if (myTag == chestnutTag)
            {
                // Añade Puntuación
                CastanyeraGameManager.Instance.AddScore(1);

                // Reproduce Sonido
                if (sfxSource != null && chestnutSfxClip != null)
                {
                    //Pequeña variación del Pitch ara no sonar siempre igual
                    float originalPitch = sfxSource.pitch;
                    sfxSource.pitch = Random.Range(0.95f, 1.05f);
                    sfxSource.PlayOneShot(chestnutSfxClip);
                    sfxSource.pitch = originalPitch;
                }
            }
            else if (myTag == rockTag)
            {
                // Roca → stun al jugador
                CastanyeraController controller = other.GetComponentInParent<CastanyeraController>();
                if (controller != null)
                {
                    controller.ApplyHitStun(0.5f); // 0.5s de penalización

                    // Reproduce Sonido de Golpe
                    if (sfxSource != null && rockHitSfxClip != null)
                    {
                        float originalPitch = sfxSource.pitch;
                        sfxSource.pitch = Random.Range(0.95f, 1.05f);
                        sfxSource.PlayOneShot(rockHitSfxClip);
                        sfxSource.pitch = originalPitch;

                    }
                }
            }

            // En ambos casos, el objeto se destruye al impactar
            Destroy(gameObject);
        }

        // Si toca el suelo, también se destruye
        if (other.CompareTag(groundTag))
        {
            Destroy(gameObject);
        }
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        currentFallSpeed = baseFallSpeed * multiplier;
    }
}
