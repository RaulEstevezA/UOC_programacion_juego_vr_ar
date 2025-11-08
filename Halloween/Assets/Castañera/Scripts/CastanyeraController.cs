using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class CastanyeraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private bool allowMouseFollow = true;

    private float xMin, xMax;
    private Camera cam;

    // refs de animación
    private Animator animator;
    private SpriteRenderer sr;

    // Control desde el GameManager
    private bool inputEnabled = true;

    void Start()
    {
        cam = Camera.main;
        Vector3 left = cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, 0f));
        Vector3 right = cam.ViewportToWorldPoint(new Vector3(1f, 0.5f, 0f));
        xMin = left.x + 0.5f;
        xMax = right.x - 0.5f;

        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Si el GameManager ha terminado la partida o ha desactivado el input -> no hacemos nada
        if (!inputEnabled) return;
        if (CastanyeraGameManager.Instance != null && CastanyeraGameManager.Instance.IsGameOver) return;

        float input = 0f;

        // --- Teclado con nuevo Input System ---
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) input -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) input += 1f;
        }
#endif

        // --- Fallback al Input clásico ---
        if (Mathf.Approximately(input, 0f))
            input = Input.GetAxisRaw("Horizontal");

        Vector3 pos = transform.position;

        // --- Seguimiento con ratón/pantalla táctil ---
        bool mousePressed =
#if ENABLE_INPUT_SYSTEM
            Mouse.current != null && Mouse.current.leftButton.isPressed;
#else
            Input.GetMouseButton(0);
#endif

        if (allowMouseFollow && mousePressed)
        {
#if ENABLE_INPUT_SYSTEM
            Vector3 mouseWorld = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
#else
            Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
#endif
            float dir = Mathf.Sign(mouseWorld.x - pos.x);
            input = Mathf.Abs(mouseWorld.x - pos.x) > 0.01f ? dir : 0f;
            pos.x = Mathf.MoveTowards(pos.x, mouseWorld.x, moveSpeed * Time.deltaTime);
        }
        else
        {
            pos.x += input * moveSpeed * Time.deltaTime;
        }

        // Limitar movimiento a los bordes de pantalla
        pos.x = Mathf.Clamp(pos.x, xMin, xMax);
        transform.position = pos;

        // --- Animación + flip de sprite ---
        float speed = Mathf.Abs(input);
        if (animator) animator.SetFloat("Speed", speed);

        if (sr)
        {
            // Gira a la derecha si se mueve a la derecha
            if (speed > 0.01f)
                sr.flipX = input > 0f;
            else
                sr.flipX = false;
        }
    }

    // Lo usa el GameManager para habilitar/deshabilitar input al inicio/fin de partida
    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
    }
}
