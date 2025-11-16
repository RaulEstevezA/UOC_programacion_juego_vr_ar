using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpenSettingsFromMenu : MonoBehaviour
{
    [Header("Botón de ajustes (engranaje)")]
    [SerializeField] private Button btnSettings;

    [Header("Nombre de la escena de ajustes")]
    [SerializeField] private string settingsSceneName = "SettingsMenu";

    private void Start()
    {
        if (btnSettings != null)
        {
            btnSettings.onClick.AddListener(OpenSettings);
        }
        else
        {
            Debug.LogWarning("OpenSettingsFromMenu: btnSettings no asignado en el Inspector.");
        }
    }

    private void OpenSettings()
    {
        if (!string.IsNullOrEmpty(settingsSceneName))
        {
            SceneManager.LoadScene(settingsSceneName);
        }
        else
        {
            Debug.LogWarning("OpenSettingsFromMenu: settingsSceneName está vacío.");
        }
    }
}
