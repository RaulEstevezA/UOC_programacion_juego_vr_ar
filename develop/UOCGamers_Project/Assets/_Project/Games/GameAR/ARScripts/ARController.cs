using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class ARController : MonoBehaviour
{
    [Header("AR Components")]
    public ARSession arSession;
    public GameObject xrOrigin;

    private void OnEnable()
    {
        EnableAR(true);
    }

    private void OnDisable()
    {
        EnableAR(false);
    }

    public void EnableAR(bool active)
    {
        if (arSession != null)
            arSession.enabled = active;

        if (xrOrigin != null)
            xrOrigin.SetActive(active);
    }

    // Llamado desde botones o desde otro script
    public void ExitTo2DScene(string sceneName)
    {
        EnableAR(false);
        SceneManager.LoadScene(sceneName);
    }
}