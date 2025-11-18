using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    [Header("Botón volver")]
    [SerializeField] private Button btnBack;

    [Header("Nombre de la escena de menú")]
    [SerializeField] private string menuSceneName = "Menu";

    [Header("Audio - Toggle")]
    [SerializeField] private Toggle audioToggle;           // Toggle de la escena

    [Header("Audio - Imágenes ON/OFF")]
    [SerializeField] private Image audioToggleBackground;  
    [SerializeField] private Sprite audioOnSprite;         // Sprite con el altavoz encendido
    [SerializeField] private Sprite audioOffSprite;        // Sprite con el altavoz muteado

    private void Start()
    {
        // Botón volver
        if (btnBack != null)
        {
            btnBack.onClick.AddListener(OnBackClicked);
        }
        else
        {
            Debug.LogWarning("SettingsMenuController: btnBack no asignado en el Inspector.");
        }

        // Toggle audio
        if (audioToggle != null)
        {
            // 1 = ON, 0 = OFF (por defecto 1)
            bool audioOn = PlayerPrefs.GetInt("audio_on", 1) == 1;

            audioToggle.isOn = audioOn;
            audioToggle.onValueChanged.AddListener(OnAudioToggleChanged);

            ApplyAudio(audioOn);
            UpdateAudioVisual(audioOn);   // Actualizamos imagen al empezar
        }
        else
        {
            Debug.LogWarning("SettingsMenuController: audioToggle no asignado en el Inspector.");
        }
    }

    private void OnBackClicked()
    {
        if (!string.IsNullOrEmpty(menuSceneName))
        {
            SceneManager.LoadScene(menuSceneName);
        }
        else
        {
            Debug.LogWarning("SettingsMenuController: menuSceneName está vacío.");
        }
    }

    // --- LÓGICA AUDIO ON/OFF ---

    private void OnAudioToggleChanged(bool isOn)
    {
        ApplyAudio(isOn);
        UpdateAudioVisual(isOn);
        PlayerPrefs.SetInt("audio_on", isOn ? 1 : 0);
    }

    private void ApplyAudio(bool isOn)
    {
        // Volumen global del juego
        AudioListener.volume = isOn ? 1f : 0f;
    }

    private void UpdateAudioVisual(bool isOn)
    {
        if (audioToggleBackground == null) return;

        // Cambia sprite según estado
        audioToggleBackground.sprite = isOn ? audioOnSprite : audioOffSprite;
    }

    private void OnDestroy()
    {
        if (audioToggle != null)
        {
            audioToggle.onValueChanged.RemoveListener(OnAudioToggleChanged);
        }
    }
}
