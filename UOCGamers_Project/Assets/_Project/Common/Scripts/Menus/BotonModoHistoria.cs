using UnityEngine;

public class BotonModoHistoria : MonoBehaviour
{
    public void ClickModoHistoria()
    {
        if (StoryModeController.Instance != null)
            StoryModeController.Instance.StartStoryMode();
    }
}
