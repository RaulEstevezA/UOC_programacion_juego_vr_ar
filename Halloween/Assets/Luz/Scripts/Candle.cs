using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Candle : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Image hitArea;  // Image del objeto raíz (zona de toque)
    [SerializeField] private Image art;      // Image del hijo "Art" que muestra la vela

    [Header("Sprites")]
    [SerializeField] private Sprite offSprite;      // candle_off.png
    [SerializeField] private Sprite[] onFrames;     // candle_on_0 .. candle_on_5
    [SerializeField] private float fps = 10f;       // velocidad de parpadeo

    public bool IsOn { get; private set; }
    public UnityEvent<Candle, bool> OnToggled;

    private Coroutine anim;

    // ----- ÚNICO Awake -----
    void Awake()
    {
        if (!hitArea) hitArea = GetComponent<Image>();
        if (!art) art = GetComponentInChildren<Image>(true);

        // BLINDAJE: el Image del padre no debe renderizar nunca (solo recibir toques)
        if (hitArea)
        {
            hitArea.sprite = null;         // sin sprite
            hitArea.material = null;
            var c = hitArea.color; c.a = 0f;
            hitArea.color = c;
            hitArea.raycastTarget = true;  // sí recibe toques
        }

        if (art) art.raycastTarget = false; // el arte no intercepta toques

        var btn = GetComponent<Button>();
        if (!btn) btn = GetComponentInChildren<Button>(true);
        if (btn) btn.onClick.AddListener(Toggle);

        if (OnToggled == null) OnToggled = new UnityEvent<Candle, bool>();

        SetState(false, true); // empezar apagada
    }

    public void Toggle()
    {
        SetState(!IsOn, false);
        OnToggled?.Invoke(this, IsOn);
    }

    public void SetState(bool on, bool instant)
    {
        IsOn = on;

        if (anim != null) { StopCoroutine(anim); anim = null; }
        if (!art) return;

        if (on)
            anim = StartCoroutine(PlayOnLoop());
        else
            art.sprite = offSprite;
    }

    private IEnumerator PlayOnLoop()
    {
        if (onFrames == null || onFrames.Length == 0) yield break;

        float dt = 1f / Mathf.Max(1f, fps);
        int i = 0;
        while (true)
        {
            art.sprite = onFrames[i];
            i = (i + 1) % onFrames.Length;
            yield return new WaitForSeconds(dt);
        }
    }
}


