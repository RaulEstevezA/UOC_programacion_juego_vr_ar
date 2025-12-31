using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FreeModeManager : MonoBehaviour
{
    [Header("Botones de juegos")]
    [SerializeField] private Button btnPumpkin;     // 2D Halloween
    [SerializeField] private Button btnCastanyera;  // 2D Castañera
    [SerializeField] private Button btnLuz;         // 2D Luz Divina
    [SerializeField] private Button btnAR;          // Juego AR
    [SerializeField] private Button btnVR;          // Juego VR

    [Header("Botones navegación")]
    [SerializeField] private Button btnBack;        // Flecha para volver al Menu
    [SerializeField] private Button btnSettings;    // Icono engranaje 

    [Header("Nombres de escenas")]
    [SerializeField] private string menuSceneName = "Menu";
    [SerializeField] private string settingsSceneName = "SettingsMenu";

    [SerializeField] private string pumpkinSceneName;     // Ej: "Halloween"
    [SerializeField] private string castanyeraSceneName;  // Ej: "Castanyera_scene"
    [SerializeField] private string luzSceneName;         // Ej: "LuzDivina"
    [SerializeField] private string arSceneName;          // Ej: "GameAR"
    [SerializeField] private string vrSceneName;          // Ej: "GameVR"

    private void Start()
    {
        // --- Botones de juegos ---
        if (btnPumpkin != null)
            btnPumpkin.onClick.AddListener(() => LoadSceneSafe(pumpkinSceneName, "Pumpkin"));

        if (btnCastanyera != null)
            btnCastanyera.onClick.AddListener(() => LoadSceneSafe(castanyeraSceneName, "Castañera"));

        if (btnLuz != null)
            btnLuz.onClick.AddListener(() => LoadSceneSafe(luzSceneName, "Luz Divina"));

        if (btnAR != null)
            btnAR.onClick.AddListener(() => LoadSceneSafe(arSceneName, "AR"));

        if (btnVR != null)
            btnVR.onClick.AddListener(() => LoadSceneSafe(vrSceneName, "VR"));

        // --- Navegación ---
        if (btnBack != null)
            btnBack.onClick.AddListener(GoBackToMenu);

        if (btnSettings != null)
            btnSettings.onClick.AddListener(GoToSettings);
    }

    private void LoadSceneSafe(string sceneName, string gameLabel)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning($"FreeModeManager: nombre de escena vacío para el juego {gameLabel}.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    private void GoBackToMenu()
    {
        if (!string.IsNullOrEmpty(menuSceneName))
            SceneManager.LoadScene(menuSceneName);
        else
            Debug.LogWarning("FreeModeManager: menuSceneName está vacío.");
    }

    private void GoToSettings()
    {
        if (!string.IsNullOrEmpty(settingsSceneName))
            SceneManager.LoadScene(settingsSceneName);
        else
            Debug.LogWarning("FreeModeManager: settingsSceneName está vacío.");
    }
}