using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private bool allowMouseFollow = true;

    private float xMin, xMax;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        // Calcula límites en mundo usando el viewport
        Vector3 left = cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, 0f));
        Vector3 right = cam.ViewportToWorldPoint(new Vector3(1f, 0.5f, 0f));
        xMin = left.x + 0.5f;
        xMax = right.x - 0.5f;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        float input = Input.GetAxisRaw("Horizontal"); // A/D o ?/?
        Vector3 pos = transform.position;

        if (allowMouseFollow && Input.GetMouseButton(0))
        {
            Vector3 mouse = cam.ScreenToWorldPoint(Input.mousePosition);
            pos.x = Mathf.MoveTowards(pos.x, mouse.x, moveSpeed * Time.deltaTime);
        }
        else
        {
            pos.x += input * moveSpeed * Time.deltaTime;
        }

        pos.x = Mathf.Clamp(pos.x, xMin, xMax);
        transform.position = pos;
    }
}
