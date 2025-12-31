using System.Collections;
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

    private Animator animator;
    private SpriteRenderer sr;

    private bool inputEnabled = true;
    private Coroutine stunCoroutine;

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
        if (!inputEnabled) return;
        if (CastanyeraGameManager.Instance != null && CastanyeraGameManager.Instance.IsGameOver) return;

        float input = 0f;
        Vector3 pos = transform.position;

#if ENABLE_INPUT_SYSTEM
        // --- Teclado PC ---
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) input -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) input += 1f;
        }

        // --- Touch en Android / Ratón ---
        bool pointerPressed = false;
        Vector3 pointerWorld = pos;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();
            pointerWorld = cam.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y, cam.nearClipPlane));
            pointerPressed = true;
        }
        else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            pointerWorld = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));
            pointerPressed = true;
        }

        if (allowMouseFollow && pointerPressed)
        {
            float dir = Mathf.Sign(pointerWorld.x - pos.x);
            input = Mathf.Abs(pointerWorld.x - pos.x) > 0.01f ? dir : 0f;
            pos.x = Mathf.MoveTowards(pos.x, pointerWorld.x, moveSpeed * Time.deltaTime);
        }

#else
        // Fallback clásico
        if (Mathf.Approximately(input, 0f))
            input = Input.GetAxisRaw("Horizontal");

        if (allowMouseFollow && Input.GetMouseButton(0))
        {
            Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
            float dir = Mathf.Sign(mouseWorld.x - pos.x);
            input = Mathf.Abs(mouseWorld.x - pos.x) > 0.01f ? dir : 0f;
            pos.x = Mathf.MoveTowards(pos.x, mouseWorld.x, moveSpeed * Time.deltaTime);
        }
#endif

        // Movimiento normal si no es seguimiento táctil
        if (!allowMouseFollow || (allowMouseFollow && !pointerPressed))
        {
            pos.x += input * moveSpeed * Time.deltaTime;
        }

        pos.x = Mathf.Clamp(pos.x, xMin, xMax);
        transform.position = pos;

        float speed = Mathf.Abs(input);
        if (animator) animator.SetFloat("Speed", speed);
        if (sr) sr.flipX = speed > 0.01f ? input > 0f : false;
    }

    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
    }

    public void ApplyHitStun(float duration)
    {
        if (stunCoroutine != null) return;
        stunCoroutine = StartCoroutine(HitStunRoutine(duration));
    }

    private IEnumerator HitStunRoutine(float duration)
    {
        SetInputEnabled(false);

        float elapsed = 0f;
        float blinkInterval = 0.1f;
        bool visible = true;

        while (elapsed < duration)
        {
            elapsed += blinkInterval;
            visible = !visible;
            if (sr != null) sr.enabled = visible;
            yield return new WaitForSeconds(blinkInterval);
        }

        if (sr != null) sr.enabled = true;

        if (CastanyeraGameManager.Instance == null || !CastanyeraGameManager.Instance.IsGameOver)
            SetInputEnabled(true);

        stunCoroutine = null;
    }
}
