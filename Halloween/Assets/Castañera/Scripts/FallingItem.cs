using UnityEngine;

public class FallingItem : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 5f;
    [SerializeField] private string chestnutTag = "Chestnut";
    [SerializeField] private string rockTag = "Rock";
    [SerializeField] private string headTag = "PlayerHead";
    [SerializeField] private string groundTag = "Ground";     

    private float yKill;
    private string myTag;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        yKill = cam.ViewportToWorldPoint(new Vector3(0.5f, -0.1f, 0f)).y; 
        myTag = gameObject.tag;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        transform.position += Vector3.down * (fallSpeed * Time.deltaTime);

        if (transform.position.y < yKill)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.Instance == null) return;

        // 1) Solo cuenta si toca la CABEZA del jugador
        if (other.CompareTag(headTag))
        {
            if (myTag == chestnutTag)
            {
                GameManager.Instance.AddScore(1);
            }
            else if (myTag == rockTag)
            {
                GameManager.Instance.EndGame(); 
            }

            Destroy(gameObject);
            return;
        }

        // 2) Tocar el SUELO limpia el ítem
        if (other.CompareTag(groundTag))
        {
            // Futura animación de explosión de castaña
            Destroy(gameObject);
        }
    }
}
