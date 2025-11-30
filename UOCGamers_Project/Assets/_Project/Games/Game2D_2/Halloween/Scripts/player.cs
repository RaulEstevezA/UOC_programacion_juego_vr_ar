using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerController : MonoBehaviour
{
    [Header("Configuración del movimiento")]
    public float velocidad = 10f;
    public float suavizado = 10f;

    private Vector3 posicionInicial;
    private float objetivoX;
    private float limiteXReal;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        posicionInicial = transform.position;
        objetivoX = posicionInicial.x;

        // Calcular límite horizontal visible según la cámara
        float mitadPantalla = cam.orthographicSize * cam.aspect;
        float mitadAnchoPaddle = GetComponent<SpriteRenderer>().bounds.size.x / 2;
        limiteXReal = mitadPantalla - mitadAnchoPaddle;
    }

    void Update()
    {
        float movimiento = 0f;
        bool pointerPressed = false;
        Vector3 pointerWorld = transform.position;

#if ENABLE_INPUT_SYSTEM
        // --- Teclado PC ---
        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.isPressed) movimiento = -1f;
            if (Keyboard.current.rightArrowKey.isPressed) movimiento = 1f;
        }

        // --- Touch en Android / Ratón ---
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
#else
        // Fallback clásico
        if (Input.GetKey(KeyCode.LeftArrow)) movimiento = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) movimiento = 1f;

        if (Input.touchCount > 0)
        {
            Touch toque = Input.GetTouch(0);
            pointerWorld = cam.ScreenToWorldPoint(new Vector3(toque.position.x, toque.position.y, 10f));
            pointerPressed = true;
        }
        else if (Input.GetMouseButton(0))
        {
            pointerWorld = cam.ScreenToWorldPoint(Input.mousePosition);
            pointerPressed = true;
        }
#endif

        // Movimiento
        if (pointerPressed)
        {
            objetivoX = Mathf.Clamp(pointerWorld.x, -limiteXReal, limiteXReal);
        }
        else
        {
            objetivoX += movimiento * velocidad * Time.deltaTime;
            objetivoX = Mathf.Clamp(objetivoX, -limiteXReal, limiteXReal);
        }

        // Suavizado
        float nuevaX = Mathf.Lerp(transform.position.x, objetivoX, Time.deltaTime * suavizado);
        transform.position = new Vector3(nuevaX, posicionInicial.y, posicionInicial.z);
    }
}
