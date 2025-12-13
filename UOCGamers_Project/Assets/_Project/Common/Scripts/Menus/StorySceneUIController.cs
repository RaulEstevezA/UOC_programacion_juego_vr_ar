using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StorySceneUIController : MonoBehaviour
{
    [System.Serializable]
    public class StoryStepData
    {
        public Sprite backgroundSprite;
        [TextArea(3, 10)] public string[] paragraphs;
    }

    [Header("UI References (según tu jerarquía)")]
    [SerializeField] private Image background;       // Canvas/Background
    [SerializeField] private TMP_Text storyText;     // Canvas/ParchmentContainer/ParchmentImage/StoryText
    [SerializeField] private Button btnContinuar;    // Canvas/BtnContinuar

    [Header("Story Steps (1 por paso: 0..4)")]
    [SerializeField] private StoryStepData[] stepsData;

    private StoryStepData currentData;
    private int currentParagraphIndex;

    private void Awake()
    {
        // Enlazar el botón por código
        if (btnContinuar != null)
        {
            btnContinuar.onClick.RemoveAllListeners();
            btnContinuar.onClick.AddListener(OnClickContinuar);
        }
    }

    private void Start()
    {
        
        if (StoryModeController.Instance == null)
        {
            // Modo "preview" para testear UI sin entrar desde menú
            if (storyText != null)
                storyText.text = "PREVIEW: Inicia el modo historia desde el menú para ver el texto real.";
            Debug.LogWarning("[StorySceneUIController] No existe StoryModeController.Instance. Ejecuta desde el menú.");
            return;
        }

        int step = StoryModeController.Instance.currentStep;

        if (stepsData == null || stepsData.Length == 0)
        {
            Debug.LogError("[StorySceneUIController] stepsData está vacío. Configura los pasos en el Inspector.");
            return;
        }

        if (step < 0 || step >= stepsData.Length)
        {
            Debug.LogWarning($"[StorySceneUIController] step {step} fuera de rango. Usando step 0.");
            step = 0;
        }

        currentData = stepsData[step];
        currentParagraphIndex = 0;

        // Fondo
        if (background != null && currentData.backgroundSprite != null)
            background.sprite = currentData.backgroundSprite;

        // Primer párrafo
        if (storyText != null)
        {
            if (currentData.paragraphs != null && currentData.paragraphs.Length > 0)
                storyText.text = currentData.paragraphs[0];
            else
                storyText.text = "";
        }
    }

    private void OnClickContinuar()
    {
        // Si no hay datos aún, intenta continuar por seguridad
        if (StoryModeController.Instance == null || currentData == null)
        {
            Debug.LogWarning("[StorySceneUIController] Sin StoryModeController o data. No se puede continuar flujo real.");
            return;
        }

        // Avanzar párrafo
        currentParagraphIndex++;

        if (currentData.paragraphs != null && currentParagraphIndex < currentData.paragraphs.Length)
        {
            storyText.text = currentData.paragraphs[currentParagraphIndex];
        }
        else
        {
            // Se acabaron los párrafos → entrar al minijuego
            StoryModeController.Instance.ContinueFromStory();
        }
    }
}
