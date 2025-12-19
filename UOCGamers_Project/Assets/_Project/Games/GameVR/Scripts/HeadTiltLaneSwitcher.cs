using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.InputSystem;

public class HeadTiltLaneSwitcher : MonoBehaviour
{
    [Header("Refs")]
    public XROrigin xrOrigin;
    public Transform hmd;

    [Header("Carriles")]
    public float leftX = -0.6f;
    public float centerX = 0f;
    public float rightX = 0.6f;

    [Header("Control VR (cabeza)")]
    public float tiltLeftDeg = 10f;
    public float tiltRightDeg = 10f;
    public float centerDeadZoneDeg = 6f;

    [Header("Control móvil (Input System NEW)")]
    public bool useMobileTilt = true;
    public float phoneTiltLeft = -0.05f;
    public float phoneTiltRight = 0.05f;
    public float phoneCenterDeadZone = 0.03f;

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
        // Buscar cámara
        if (!xrOrigin) xrOrigin = FindAnyObjectByType<XROrigin>();
        if (!hmd && xrOrigin && xrOrigin.Camera) hmd = xrOrigin.Camera.transform;
        if (!hmd && Camera.main) hmd = Camera.main.transform;

        vrActivo = DetectarVRActivo();

        // Activar tilt móvil si estamos en Android y no hay VR
        mobileTiltActivo = Application.isMobilePlatform && useMobileTilt && !vrActivo;

        if (mobileTiltActivo)
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;

            if (Accelerometer.current != null)
            {
                InputSystem.EnableDevice(Accelerometer.current);
                Debug.Log("✅ Acelerómetro activado (Input System NEW)");
            }
            else
            {
                Debug.LogError("❌ No se detecta acelerómetro");
            }
        }

        Debug.Log($"HeadTiltLaneSwitcher iniciado | VR={vrActivo} | MobileTilt={mobileTiltActivo}");
    }

    void Update()
    {
        if (Time.time - lastSwitchTime >= laneCooldown)
        {
            if (mobileTiltActivo)
                ControlConTiltMovil_NEW();
            else if (vrActivo)
                ControlConHMD();
            else
                ControlConTeclado();
        }

        float targetX = targetLane switch
        {
            Lane.Left => leftX,
            Lane.Right => rightX,
            _ => centerX
        };

        Vector3 pos = transform.position;
        pos.x = Mathf.MoveTowards(pos.x, targetX, moveSpeed * Time.deltaTime);
        transform.position = pos;
    }

    // ---------------- VR ----------------
    void ControlConHMD()
    {
        if (!hmd) return;

        float roll = hmd.localEulerAngles.z;
        if (roll > 180f) roll -= 360f;

        if (roll <= -tiltLeftDeg && targetLane != Lane.Left)
            CambiarCarril(Lane.Left, "VR LEFT");
        else if (roll >= tiltRightDeg && targetLane != Lane.Right)
            CambiarCarril(Lane.Right, "VR RIGHT");
        else if (Mathf.Abs(roll) <= centerDeadZoneDeg && targetLane != Lane.Center)
            CambiarCarril(Lane.Center, "VR CENTER");
    }

    // ---------------- MÓVIL (NEW INPUT SYSTEM) ----------------
    void ControlConTiltMovil_NEW()
    {
        if (Accelerometer.current == null) return;

        Vector3 acc = Accelerometer.current.acceleration.ReadValue();
        float x = acc.x;

        Debug.Log($"📱 ACC => x:{x:F2} y:{acc.y:F2} z:{acc.z:F2}");

        if (x <= phoneTiltLeft && targetLane != Lane.Left)
            CambiarCarril(Lane.Left, "MÓVIL LEFT");
        else if (x >= phoneTiltRight && targetLane != Lane.Right)
            CambiarCarril(Lane.Right, "MÓVIL RIGHT");
        else if (Mathf.Abs(x) <= phoneCenterDeadZone && targetLane != Lane.Center)
            CambiarCarril(Lane.Center, "MÓVIL CENTER");
    }

    // ---------------- TECLADO ----------------
    void ControlConTeclado()
    {
        var k = Keyboard.current;
        if (k == null) return;

        if (k.leftArrowKey.wasPressedThisFrame)
        {
            if (targetLane == Lane.Right) CambiarCarril(Lane.Center, "PC");
            else if (targetLane == Lane.Center) CambiarCarril(Lane.Left, "PC");
        }
        else if (k.rightArrowKey.wasPressedThisFrame)
        {
            if (targetLane == Lane.Left) CambiarCarril(Lane.Center, "PC");
            else if (targetLane == Lane.Center) CambiarCarril(Lane.Right, "PC");
        }
    }

    // ---------------- UTIL ----------------
    void CambiarCarril(Lane nuevo, string origen)
    {
        targetLane = nuevo;
        lastSwitchTime = Time.time;
        Debug.Log($"➡ {origen}: {nuevo}");
    }

    bool DetectarVRActivo()
    {
        if (xrOrigin && xrOrigin.Camera && xrOrigin.Camera.stereoEnabled)
            return true;

        if (Camera.main && Camera.main.stereoEnabled)
            return true;

        return false;
    }
}
