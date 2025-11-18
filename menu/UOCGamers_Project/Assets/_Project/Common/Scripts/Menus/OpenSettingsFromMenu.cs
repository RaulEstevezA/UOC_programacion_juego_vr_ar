using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpenSettingsFromMenu : MonoBehaviour
{
    [Header("Botones del menú")]
    [SerializeField] private Button btnSettings;     // engranaje
    [SerializeField] private Button btnModoHistoria;
    [SerializeField] private Button btnModoLibre;

    [Header("Nombres de escenas")]
    [SerializeField] private string settingsSceneName = "SettingsMenu";
    [SerializeField] private string freeModeSceneName = "ModoLibre";
    

    private void Start()
    {
        if (btnSettings != null)
            btnSettings.onClick.AddListener(OpenSettings);

        if (btnModoHistoria != null)
            btnModoHistoria.onClick.AddListener(OpenHistoria);

        if (btnModoLibre != null)
            btnModoLibre.onClick.AddListener(OpenFreeMode);
    }

    private void OpenSettings()
    {
        SceneManager.LoadScene(settingsSceneName);
    }

    private void OpenHistoria()
    {
        // Usamos el director del modo historia
        if (StoryModeController.Instance != null)
        {
            StoryModeController.Instance.StartStoryMode();
        }
        else
        {
            Debug.LogError("No hay StoryModeController en la escena Menu.");
        }
    }

    private void OpenFreeMode()
    {
        SceneManager.LoadScene(freeModeSceneName);
    }
}
