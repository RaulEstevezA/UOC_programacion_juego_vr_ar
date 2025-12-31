using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class HistoriaFinalUI : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private TMP_Text finalText;     // “¡Historia completada!”
    [SerializeField] private TMP_Text scoreText;     // “Puntuación total: X”
    [SerializeField] private Button btnVolverMenu;   // Botón volver

    private void Start()
    {
        if (btnVolverMenu != null)
            btnVolverMenu.onClick.AddListener(VolverAlMenu);

        // Obtener la puntuación del StoryModeController
        int total = 0;

        if (StoryModeController.Instance != null)
            total = StoryModeController.Instance.totalScore;

        // Mostrar datos en pantalla
        if (scoreText != null)
            scoreText.text = "Puntuación total: " + total;

        if (finalText != null)
            finalText.text = "¡Historia completada!";
    }

    private void VolverAlMenu()
    {
        // Reset del modo historia
        if (StoryModeController.Instance != null)
        {
            StoryModeController.Instance.storyModeActive = false;
            StoryModeController.Instance.currentStep = 0;
            StoryModeController.Instance.totalScore = 0;
        }

        // Cargar menú principal
        SceneManager.LoadScene("Menu");
    }
}
