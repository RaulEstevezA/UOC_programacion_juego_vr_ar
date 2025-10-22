using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BotonJugar : MonoBehaviour
{
    void Start()
    {
        Button btn = GetComponent<Button>(); // Toma el Button del mismo GameObject
        btn.onClick.AddListener(() => SceneManager.LoadScene("SampleScene"));
    }
}
