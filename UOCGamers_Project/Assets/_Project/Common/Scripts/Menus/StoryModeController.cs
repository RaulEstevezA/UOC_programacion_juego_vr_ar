using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryModeController : MonoBehaviour
{
    public static StoryModeController Instance;

    [Header("Escenas de historia / UI")]
    [SerializeField] private string storySceneName = "ModoHistoria";   // escena de texto
    [SerializeField] private string finalSceneName = "HistoriaFinal";  // escena resumen (la crearás luego)

    [Header("Escenas de minijuegos (en orden)")]
    [SerializeField] private string game1SceneName = "Halloween";
    [SerializeField] private string game2SceneName = "Castanyera_scene";
    [SerializeField] private string game3SceneName = "LuzDivina";
    
    [SerializeField] private string game5SceneName = "GameVR"; //por ahora no se usara
    [SerializeField] private string game4SceneName = "TioAR"; //por ahora no se usara
    [Header("Progreso historia")]
    public int currentStep = 0;      // 0=intro, 1=post juego1, 2=post juego2, etc.
    public int totalScore = 0;

    [Tooltip("Indica si estamos en modo historia.")]
    public bool storyModeActive = false;   // <- bandera clave

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// Se llama desde el menú al pulsar "Modo Historia"
    public void StartStoryMode()
    {
        currentStep = 0;
        totalScore = 0;
        storyModeActive = true;

        Debug.Log("[StoryMode] StartStoryMode() → Paso 0, puntuación 0");
        SceneManager.LoadScene(storySceneName); // Primera pantalla de historia
    }

    /// Puedes llamarlo cuando vuelvas al menú principal
    public void ExitStoryMode()
    {
        storyModeActive = false;
        Debug.Log("[StoryMode] ExitStoryMode() → storyModeActive = false");
    }

    /// Devuelve el texto de historia según el paso actual
    public string GetCurrentStoryText()
    {
        switch (currentStep)
        {
            case 0:
                return "Dicen que en España cada fiesta es una puerta.. Un niño curioso encuentra un calendario viejo en el desvan y de repente comienza un año de aventuras...";
            case 1:
                return "Aqui huele a brasero y castañas.. uhm esa señora necesita mi ayuda para recojer los frutos.";
            case 2:
                return "Se acerca Semana Santa... Debo apagar todas las velas, es la única forma de adquirir la Luz Divina";
            case 3:
                return "Al fin civilización... Running Of The Bulls. Hostia, ya entiendo, que no me pille el toro. Tengo que escapaaaar!";
            case 4:
                return "La navidad esta cerca... ¡Que poco me gusta! Necesito encontrar al Tió para salir de aqui"; 
            default:
                return "";
        }
    }


    /// Llamado desde la escena de historia cuando se pulsa "Continuar"
    public void ContinueFromStory()
    {
        Debug.Log($"[StoryMode] ContinueFromStory() paso={currentStep}");

        switch (currentStep)
        {
            case 0: SceneManager.LoadScene(game1SceneName); break;
            case 1: SceneManager.LoadScene(game2SceneName); break;
            case 2: SceneManager.LoadScene(game3SceneName); break;
            case 3: SceneManager.LoadScene(game4SceneName); break;
            case 4: SceneManager.LoadScene(game5SceneName); break;

            case 5:
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
        currentStep++;

        Debug.Log($"[StoryMode] MiniJuego terminado. score={score}, total={totalScore}, nextStep={currentStep}");

        if (currentStep <= 4)   // después de los juegos 1..4 vuelve a historia
        {
            SceneManager.LoadScene(storySceneName);
        }
        else                    // step 5 → FINAL
        {
            SceneManager.LoadScene(finalSceneName);
        }
    }



}
