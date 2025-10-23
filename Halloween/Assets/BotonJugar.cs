using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BotonJugar : MonoBehaviour
{
    void Start()
    {
        Button btn = GetComponent<Button>(); 
        btn.onClick.AddListener(() => SceneManager.LoadScene("Halloween"));
    }
}
