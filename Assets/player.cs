using UnityEngine;

public class player : MonoBehaviour
{
    [Header("Configuración del movimiento")]
    public float velocidad = 10f;
    public float suavizado = 10f;

    private Vector3 posicionInicial;
    private float objetivoX;
    private float limiteXReal;

    void Start()
    {
        posicionInicial = transform.position;
        objetivoX = posicionInicial.x;

        // 🔹 Calcular el límite horizontal visible según la cámara
        float mitadPantalla = Camera.main.orthographicSize * Camera.main.aspect;
        float mitadAnchoPaddle = GetComponent<SpriteRenderer>().bounds.size.x / 2;
        limiteXReal = mitadPantalla - mitadAnchoPaddle;
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        MoverEnPC();
#else
        MoverEnAndroid();
#endif

        // Movimiento suavizado
        float nuevaX = Mathf.Lerp(transform.position.x, objetivoX, Time.deltaTime * suavizado);
        transform.position = new Vector3(nuevaX, posicionInicial.y, posicionInicial.z);
    }

    void MoverEnPC()
    {
        float movimiento = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))
            movimiento = -1f;
        else if (Input.GetKey(KeyCode.RightArrow))
            movimiento = 1f;

        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            objetivoX = Mathf.Clamp(mousePos.x, -limiteXReal, limiteXReal);
        }
        else
        {
            objetivoX += movimiento * velocidad * Time.deltaTime;
            objetivoX = Mathf.Clamp(objetivoX, -limiteXReal, limiteXReal);
        }
    }

    void MoverEnAndroid()
    {
        if (Input.touchCount > 0)
        {
            Touch toque = Input.GetTouch(0);
            Vector3 toquePos = Camera.main.ScreenToWorldPoint(new Vector3(toque.position.x, toque.position.y, 10f));
            transform.position = new Vector3(
                Mathf.Clamp(toquePos.x, -limiteXReal, limiteXReal),
                posicionInicial.y,
                posicionInicial.z
            );
        }
    }
}
