using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashTeamControlador : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private CanvasGroup splashCanvasGroup;

    [Header("Tiempos")]
    [SerializeField] private float fadeDuration = 1f; // Tiempo que tarda en aparecer/desaparecer (en segundos)

    [SerializeField] private float showDuration = 2f; // Tiempo que permanece totalmente visible

    [Header("Escena siguiente")]
    [SerializeField] private string nextSceneName = "Menu";

    private void Start()
    {
        // Empieza transparente
        if (splashCanvasGroup != null)
        {
            splashCanvasGroup.alpha = 0f;
        }

        // Secuencia de fade + cambio de escena
        StartCoroutine(SplashSequence());
    }

    private IEnumerator SplashSequence()
    {
        // 1) FADE IN (0 → 1)
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / fadeDuration);

            if (splashCanvasGroup != null)
                splashCanvasGroup.alpha = t;  // Interpola de 0 a 1

            yield return null; // Esperar al siguiente frame
        }

        // 2) Mantener visible 
        yield return new WaitForSeconds(showDuration);

        // 3) FADE OUT (1 → 0)
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / fadeDuration);

            if (splashCanvasGroup != null)
                splashCanvasGroup.alpha = 1f - t; // Interpola de 1 a 0

            yield return null;
        }

        // 4) Cargar la siguiente escena
        SceneManager.LoadScene(nextSceneName);
    }
}
