using UnityEngine;

public class player : MonoBehaviour
{
    [Header("Configuración del movimiento")]
    public float limiteX = 8f;        // Límite horizontal (ajustar según escena)
    public float velocidad = 10f;     // Velocidad de movimiento
    public float suavizado = 10f;     // Factor de interpolación (mayor = más suave)

    private Vector3 posicionInicial;
    private float objetivoX;           // Posición objetivo horizontal

    void Start()
    {
        posicionInicial = transform.position;
        objetivoX = posicionInicial.x;
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        // 💻 En PC: mover con teclado o ratón
        MoverEnPC();
#else
        // 📱 En Android: mover con el dedo
        MoverEnAndroid();
#endif

        // 🔹 Movimiento suavizado hacia la posición objetivo
        float nuevaX = Mathf.Lerp(transform.position.x, objetivoX, Time.deltaTime * suavizado);
        transform.position = new Vector3(nuevaX, posicionInicial.y, posicionInicial.z);
    }

    // --- Movimiento con teclado o ratón ---
    void MoverEnPC()
    {
        float movimiento = 0f;

        // Flechas de dirección
        if (Input.GetKey(KeyCode.LeftArrow))
            movimiento = -1f;
        else if (Input.GetKey(KeyCode.RightArrow))
            movimiento = 1f;

        // Movimiento directo con el ratón (opcional)
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            objetivoX = Mathf.Clamp(mousePos.x, -limiteX, limiteX);
        }
        else
        {
            // Movimiento con teclado (incremental)
            objetivoX += movimiento * velocidad * Time.deltaTime;
            objetivoX = Mathf.Clamp(objetivoX, -limiteX, limiteX);
        }
    }

    // --- Movimiento con el dedo (Android) ---
    void MoverEnAndroid()
    {
        if (Input.touchCount > 0)
        {
            Touch toque = Input.GetTouch(0);
            Vector3 toquePos = Camera.main.ScreenToWorldPoint(toque.position);
            objetivoX = Mathf.Clamp(toquePos.x, -limiteX, limiteX);
        }
    }
}
