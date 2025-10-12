using UnityEngine;

public class FallingItem : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 5f;
    [SerializeField] private string chestnutTag = "Chestnut";
    [SerializeField] private string rockTag = "Rock";

    private float yKill;
    private string myTag;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        yKill = cam.ViewportToWorldPoint(new Vector3(0.5f, -0.1f, 0f)).y; // por debajo de la pantalla
        myTag = gameObject.tag;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        transform.position += Vector3.down * (fallSpeed * Time.deltaTime);

        if (transform.position.y < yKill)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.Instance == null) return;
        if (!other.CompareTag("Player")) return;

        if (myTag == chestnutTag)
        {
            GameManager.Instance.AddScore(1);
            Destroy(gameObject);
        }
        else if (myTag == rockTag)
        {
            GameManager.Instance.EndGame();
            Destroy(gameObject);
        }
    }
}
