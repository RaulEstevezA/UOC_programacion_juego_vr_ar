using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StorySceneUI : MonoBehaviour
{
    [SerializeField] private TMP_Text storyText;   
    [SerializeField] private Button btnContinue;   // Botón CONTINUAR

    private void Start()
    {
        // Cargar el texto desde el StoryModeController
        if (StoryModeController.Instance != null)
        {
            storyText.text = StoryModeController.Instance.GetCurrentStoryText();
        }
        else
        {
            storyText.text = "ERROR: no hay StoryModeController en escena.";
            Debug.LogError("StorySceneUI: StoryModeController.Instance es null.");
        }

        // Asignar el click del botón
        if (btnContinue != null)
        {
            btnContinue.onClick.AddListener(OnContinueClicked);
        }
        else
        {
            Debug.LogWarning("StorySceneUI: btnContinue no asignado en el Inspector.");
        }
    }

    private void OnContinueClicked()
    {
        if (StoryModeController.Instance != null)
        {
            StoryModeController.Instance.ContinueFromStory();
        }
        else
        {
            Debug.LogError("StorySceneUI: no se pudo continuar, StoryModeController.Instance es null.");
        }
    }
}
