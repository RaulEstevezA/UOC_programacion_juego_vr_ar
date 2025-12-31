using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BotonCargarEscena : MonoBehaviour
{
    [SerializeField] private string nombreEscena;

    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(nombreEscena);
        });
    }
}
