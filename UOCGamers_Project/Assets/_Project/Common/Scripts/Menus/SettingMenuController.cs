using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    [Header("Botón volver")]
    [SerializeField] private Button btnBack;

    [Header("Nombre de la escena de menú")]
    [SerializeField] private string menuSceneName = "Menu";

    private void Start()
    {
        if (btnBack != null)
        {
            btnBack.onClick.AddListener(OnBackClicked);
        }
        else
        {
            Debug.LogWarning("SettingsMenuController: btnBack no asignado en el Inspector.");
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
}
