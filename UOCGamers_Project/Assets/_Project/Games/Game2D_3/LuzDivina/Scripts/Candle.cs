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

    [Header("Sonidos")]
    [SerializeField] private AudioClip soundOn;
    [SerializeField] private AudioClip soundOff;
    [SerializeField] private float volume = 0.8f;

    public bool IsOn { get; private set; }
    public UnityEvent<Candle, bool> OnToggled;

    private Coroutine anim;
    private AudioSource audioSource;

    void Awake()
    {
        // ----- Referencias básicas -----
        if (!hitArea) hitArea = GetComponent<Image>();

        // Busca un Image hijo distinto al root para art
        if (!art)
        {
            var imgs = GetComponentsInChildren<Image>(true);
            foreach (var img in imgs)
            {
                if (img.gameObject != this.gameObject)
                {
                    art = img;
                    break;
                }
            }
        }

        if (!art)
            Debug.LogError("[Candle] Falta asignar 'art' (Image hijo)", this);

        // HitArea: invisible pero clicable
        if (hitArea)
        {
            if (hitArea.sprite != null) hitArea.sprite = null;
            var c = hitArea.color; c.a = 0f; hitArea.color = c;
            hitArea.raycastTarget = true;
        }

        // El arte no intercepta toques
        if (art) art.raycastTarget = false;

        // ----- Botón -----
        var btn = GetComponent<Button>();
        if (!btn) btn = gameObject.AddComponent<Button>();
        if (btn && btn.targetGraphic == null) btn.targetGraphic = hitArea;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(Toggle);

        // ----- Audio -----
        audioSource = GetComponent<AudioSource>();
        if (!audioSource) audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = volume;

        // ----- Eventos -----
        if (OnToggled == null)
            OnToggled = new UnityEvent<Candle, bool>();

        // ----- Estado inicial -----
        SetState(false, true); // apagada sin sonido
    }

    public void Toggle()
    {
        Debug.Log($"[Candle] CLICK en {name}, antes={IsOn}");
        SetState(!IsOn, false);
        OnToggled?.Invoke(this, IsOn);   // ahora mismo BoardController NUEVO no lo usa
    }

    public void SetState(bool on, bool instant)
    {
        IsOn = on;

        // Sonido
        if (!instant && audioSource)
        {
            var clip = on ? soundOn : soundOff;
            if (clip)
            {
                audioSource.pitch = Random.Range(0.95f, 1.05f);
                audioSource.PlayOneShot(clip);
            }
        }

        // Visual / animación
        if (anim != null)
        {
            StopCoroutine(anim);
            anim = null;
        }

        if (!art) return;

        if (IsOn && onFrames != null && onFrames.Length > 0)
        {
            anim = StartCoroutine(PlayOnLoop());
        }
        else
        {
            art.sprite = offSprite;
        }
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
