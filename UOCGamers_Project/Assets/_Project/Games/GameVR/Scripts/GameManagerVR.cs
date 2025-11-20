using UnityEngine;
using TMPro;

public class GameManagerVR : MonoBehaviour
{
    public static GameManagerVR Instance;

    [Header("Juego")]
    public int lives = 3;
    public int score = 0;
    public bool isGameOver = false;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public GameObject gameOverPanel;

    void Awake()
    {
        if (Instance == null) Instance = this; else Destroy(gameObject);
    }

    void Start()
    {
        UpdateUI();
        if (gameOverPanel) gameOverPanel.SetActive(false);
    }

    public void AddScore(int amount)
    {
        if (isGameOver) return;
        score += amount;
        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        if (isGameOver) return;

        lives -= amount;
        Debug.Log($"Jugador recibe {amount} de daño. Vidas restantes: {lives}");

        if (lives <= 0)
        {
            lives = 0;
            Debug.Log("💀 Vidas agotadas. Game Over!");
            GameOver();
        }

        UpdateUI();
    }


    void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0.9f; // leve pausa
        if (gameOverPanel) gameOverPanel.SetActive(true);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    void UpdateUI()
    {
        if (scoreText) scoreText.text = $"Puntos: {score}";
        if (livesText) livesText.text = $"Vidas: {lives}";
    }
}
