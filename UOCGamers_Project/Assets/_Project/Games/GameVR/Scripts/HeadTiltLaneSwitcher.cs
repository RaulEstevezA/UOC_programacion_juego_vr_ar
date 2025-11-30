using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.InputSystem;

public class HeadTiltLaneSwitcher : MonoBehaviour
{
    [Header("Refs")]
    public XROrigin xrOrigin;
    public Transform hmd; // Cámara VR o cámara principal

    [Header("Carriles")]
    public float leftX = -0.6f;
    public float centerX = 0f;
    public float rightX = 0.6f;

    [Header("Control por inclinación cabeza (VR)")]
    public float tiltLeftDeg = 10f;
    public float tiltRightDeg = 10f;
    public float centerDeadZoneDeg = 6f;

    [Header("Control por inclinación móvil (Android)")]
    public bool useMobileTilt = true;
    public float phoneTiltLeft = -0.2f;   // acelerómetro x <= esto → izquierda
    public float phoneTiltRight = 0.2f;   // acelerómetro x >= esto → derecha
    public float phoneCenterDeadZone = 0.1f;

    [Header("Movimiento")]
    public float moveSpeed = 3f;
    public float laneCooldown = 0.3f;

    private enum Lane { Left, Center, Right }
    private Lane targetLane = Lane.Center;
    private float lastSwitchTime;

    bool vrActivo = false;
    bool mobileTiltActivo = false;

    void Start()
    {
        // Buscar refs si no están puestas
        if (!xrOrigin)
            xrOrigin = FindAnyObjectByType<XROrigin>();

        if (!hmd && xrOrigin && xrOrigin.Camera)
            hmd = xrOrigin.Camera.transform;

        if (!hmd && Camera.main)
            hmd = Camera.main.transform;

        // Detectar VR
        vrActivo = DetectarVRActivo();

        // Detectar si vamos a usar tilt de móvil (solo si no hay VR)
        mobileTiltActivo = !vrActivo && Application.isMobilePlatform && useMobileTilt;

        if (mobileTiltActivo)
        {
            // No es obligatorio, pero ayuda a que el sensor vaya fino
            if (SystemInfo.supportsGyroscope)
                Input.gyro.enabled = true;
        }

        Debug.Log($"HeadTiltLaneSwitcher iniciado | VR: {vrActivo} | MobileTilt: {mobileTiltActivo}");
    }

    bool DetectarVRActivo()
    {
        // Usamos la propia XROrigin si existe
        if (xrOrigin && xrOrigin.Camera && xrOrigin.Camera.stereoEnabled)
            return true;

        // Fallback: cámara principal en modo estéreo
        Camera cam = Camera.main;
        if (cam && cam.stereoEnabled)
            return true;

        return false;
    }

    void Update()
    {
        // Control de entrada con cooldown entre cambios de carril
        if (Time.time - lastSwitchTime >= laneCooldown)
        {
            if (vrActivo)
                ControlConHMD();
            else if (mobileTiltActivo)
                ControlConTiltMovil();
            else
                ControlConTeclado();
        }

        // Movimiento suave hacia el carril objetivo
        float targetX = targetLane switch
        {
            Lane.Left => leftX,
            Lane.Right => rightX,
            _ => centerX
        };

        var pos = transform.position;
        pos.x = Mathf.MoveTowards(pos.x, targetX, moveSpeed * Time.deltaTime);
        transform.position = pos;
    }

    // ------------ VR: cabeceo (roll de la HMD) ------------
    void ControlConHMD()
    {
        if (!hmd) return;

        float roll = hmd.localEulerAngles.z;
        if (roll > 180f) roll -= 360f; // pasar a rango [-180, 180]

        if (roll <= -tiltLeftDeg && targetLane != Lane.Left)
        {
            targetLane = Lane.Left;
            lastSwitchTime = Time.time;
            Debug.Log("VR: Cambio a carril LEFT");
        }
        else if (roll >= tiltRightDeg && targetLane != Lane.Right)
        {
            targetLane = Lane.Right;
            lastSwitchTime = Time.time;
            Debug.Log("VR: Cambio a carril RIGHT");
        }
        else if (Mathf.Abs(roll) <= centerDeadZoneDeg && targetLane != Lane.Center)
        {
            targetLane = Lane.Center;
            lastSwitchTime = Time.time;
            Debug.Log("VR: Cambio a carril CENTER");
        }
    }

    // ------------ Android: inclinación del móvil ------------
    void ControlConTiltMovil()
    {
        // Asumiendo orientación horizontal (Landscape)
        Vector3 acc = Input.acceleration;
        float x = acc.x;

        // Izquierda
        if (x <= phoneTiltLeft && targetLane != Lane.Left)
        {
            targetLane = Lane.Left;
            lastSwitchTime = Time.time;
            Debug.Log($"Móvil: Cambio a carril LEFT | acc.x={x:F2}");
        }
        // Derecha
        else if (x >= phoneTiltRight && targetLane != Lane.Right)
        {
            targetLane = Lane.Right;
            lastSwitchTime = Time.time;
            Debug.Log($"Móvil: Cambio a carril RIGHT | acc.x={x:F2}");
        }
        // Centro
        else if (Mathf.Abs(x) <= phoneCenterDeadZone && targetLane != Lane.Center)
        {
            targetLane = Lane.Center;
            lastSwitchTime = Time.time;
            Debug.Log($"Móvil: Cambio a carril CENTER | acc.x={x:F2}");
        }
    }

    // ------------ PC: teclado (flechas izq/der) ------------
    void ControlConTeclado()
    {
        var k = Keyboard.current;
        if (k == null) return;

        bool cambio = false;

        if (k.leftArrowKey.wasPressedThisFrame)
        {
            if (targetLane == Lane.Right) { targetLane = Lane.Center; cambio = true; }
            else if (targetLane == Lane.Center) { targetLane = Lane.Left; cambio = true; }
        }
        else if (k.rightArrowKey.wasPressedThisFrame)
        {
            if (targetLane == Lane.Left) { targetLane = Lane.Center; cambio = true; }
            else if (targetLane == Lane.Center) { targetLane = Lane.Right; cambio = true; }
        }

        if (cambio)
        {
            lastSwitchTime = Time.time;
            Debug.Log($"PC: Cambio a carril {targetLane}");
        }
    }
}
