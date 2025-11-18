using UnityEngine;
using UnityEngine.SceneManagement;

/// Director del Modo Historia (vive entre escenas)
public class StoryModeController : MonoBehaviour
{
    public static StoryModeController Instance;

    [Header("Escenas de historia / UI")]
    [SerializeField] private string storySceneName = "ModoHistoria";   // escena donde muestras texto
    [SerializeField] private string finalSceneName = "HistoriaFinal";  // escena resumen de puntos (la crearás luego)

    [Header("Escenas de minijuegos (en orden de historia)")]
    [SerializeField] private string game1SceneName = "Halloween";
    [SerializeField] private string game2SceneName = "Castanyera_scene";
    [SerializeField] private string game3SceneName = "LuzDivina";   // pon el nombre real
    [SerializeField] private string game4SceneName = "GameAR";
    [SerializeField] private string game5SceneName = "GameVR";

    [Header("Puntuación total")]
    public int totalScore = 0;

    // 0 = historia antes del juego1
    // 1 = historia después juego1
    // 2 = después juego2
    // 3 = después juego3
    // 4 = después juego4
    // 5 = historia final (después juego5)
    public int currentStep = 0;

    private void Awake()
    {
        // patrón Singleton básico
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);  // NO se destruye al cambiar de escena
    }

    /// Llamado desde el menú cuando el jugador pulsa "Modo Historia"
    public void StartStoryMode()
    {
        currentStep = 0;
        totalScore = 0;
        SceneManager.LoadScene(storySceneName);  // primera pantalla de historia
    }

    /// Devuelve el texto de historia que toca según el paso actual
    public string GetCurrentStoryText()
    {
        switch (currentStep)
        {
            case 0:
                return "Aquí va la INTRO de la historia antes del primer juego.";
            case 1:
                return "Texto después del Juego 1, antes del Juego 2.";
            case 2:
                return "Texto después del Juego 2, antes del Juego 3.";
            case 3:
                return "Texto después del Juego 3, antes del Juego 4.";
            case 4:
                return "Texto después del Juego 4, antes del Juego 5.";
            case 5:
                return "Texto final de la historia antes del resumen de puntuación.";
            default:
                return "";
        }
    }

    /// Llamado desde la escena de historia cuando el jugador pulsa "Continuar"
    public void ContinueFromStory()
    {
        switch (currentStep)
        {
            case 0:
                // INTRO -> Juego 1
                SceneManager.LoadScene(game1SceneName);
                break;
            case 1:
                SceneManager.LoadScene(game2SceneName);
                break;
            case 2:
                SceneManager.LoadScene(game3SceneName);
                break;
            case 3:
                SceneManager.LoadScene(game4SceneName);
                break;
            case 4:
                SceneManager.LoadScene(game5SceneName);
                break;
            case 5:
                // Historia final -> pantalla resumen
                SceneManager.LoadScene(finalSceneName);
                break;
            default:
                Debug.LogWarning("StoryModeController: paso fuera de rango.");
                break;
        }
    }

    /// Llamar desde cada minijuego cuando termine (en modo historia)
    public void OnMiniGameFinished(int score)
    {
        totalScore += score;
        currentStep++;  // pasamos al siguiente bloque de historia
        SceneManager.LoadScene(storySceneName);
    }
}
