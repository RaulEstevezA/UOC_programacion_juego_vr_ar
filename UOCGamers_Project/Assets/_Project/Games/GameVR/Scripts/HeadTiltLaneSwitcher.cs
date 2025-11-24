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

    [Header("Control por inclinación (VR)")]
    public float tiltLeftDeg = 10f;
    public float tiltRightDeg = 10f;
    public float centerDeadZoneDeg = 6f;

    [Header("Movimiento")]
    public float moveSpeed = 3f;
    public float laneCooldown = 0.3f;

    private enum Lane { Left, Center, Right }
    private Lane targetLane = Lane.Center;
    private float lastSwitchTime;

    bool vrActivo = false;

    void Start()
    {
        // Detectar si hay VR activo
        vrActivo = false;

        if (!xrOrigin) xrOrigin = FindAnyObjectByType<XROrigin>();
        if (!hmd && xrOrigin) hmd = xrOrigin.Camera.transform;

        Debug.Log("HeadTiltLaneSwitcher iniciado | VR activo: " + vrActivo);
    }

    bool DetectarVRActivo()
    {
        var origen = FindAnyObjectByType<XROrigin>();
        if (origen && origen.Camera) return true;

        Camera cam = Camera.main;
        if (cam && cam.stereoEnabled) return true;

        return false;
    }

    void Update()
    {
        if (Time.time - lastSwitchTime >= laneCooldown)
        {
            if (vrActivo)
                ControlConHMD();
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

    void ControlConHMD()
    {
        if (!hmd) return;

        float roll = hmd.localEulerAngles.z;
        if (roll > 180f) roll -= 360f;

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
