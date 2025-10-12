using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text finalScoreText;

    public bool IsGameOver { get; private set; }
    private int score;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        UpdateScoreUI();
        if (gameOverPanel) gameOverPanel.SetActive(false);
        IsGameOver = false;
    }

    public void AddScore(int amount)
    {
        if (IsGameOver) return;
        score += amount;
        UpdateScoreUI();
    }

    public void EndGame()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        if (gameOverPanel) gameOverPanel.SetActive(true);
        if (finalScoreText) finalScoreText.text = $"Castañas: {score}";
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UpdateScoreUI()
    {
        if (scoreText) scoreText.text = $"Puntos: {score}";
    }
}
