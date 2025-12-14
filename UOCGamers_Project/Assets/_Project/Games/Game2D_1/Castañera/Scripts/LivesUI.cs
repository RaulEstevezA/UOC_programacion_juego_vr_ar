using UnityEngine;
using UnityEngine.UI;

public class LivesUI : MonoBehaviour
{
    [Header("Heart Images (2 lives)")]
    [SerializeField] private Image life1;
    [SerializeField] private Image life2;

    [Header("Sprites")]
    [SerializeField] private Sprite fullVida;
    [SerializeField] private Sprite emptyVida;

    public void SetLives(int lives)
    {
        lives = Mathf.Clamp(lives, 0, 2);

        if (life1 != null) life1.sprite = (lives >= 1) ? fullVida : emptyVida;
        if (life2 != null) life2.sprite = (lives >= 2) ? fullVida : emptyVida;
    }
}
