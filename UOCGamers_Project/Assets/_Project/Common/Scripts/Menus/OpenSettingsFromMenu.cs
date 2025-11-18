using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpenSettingsFromMenu : MonoBehaviour
{
    [Header("Botones del menú")]
    [SerializeField] private Button btnSettings;   // engranaje
    [SerializeField] private Button btnModoLibre;  // botón Modo Libre

    [Header("Nombres de escenas")]
    [SerializeField] private string settingsSceneName = "SettingsMenu";
    [SerializeField] private string freeModeSceneName = "ModoLibre";

    private void Start()
    {
        // Botón ajustes
        if (btnSettings != null)
            btnSettings.onClick.AddListener(OpenSettings);
        else
            Debug.LogWarning("OpenSettingsFromMenu: btnSettings no asignado.");

        // Botón Modo Libre
        if (btnModoLibre != null)
            btnModoLibre.onClick.AddListener(OpenFreeMode);
        else
            Debug.LogWarning("OpenSettingsFromMenu: btnModoLibre no asignado.");
    }

    private void OpenSettings()
    {
        SceneManager.LoadScene(settingsSceneName);
    }

    private void OpenFreeMode()
    {
        SceneManager.LoadScene(freeModeSceneName);
    }
}
