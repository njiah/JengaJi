using UnityEngine;
using UnityEngine.UI;   
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public Transform towerBase;
    public float lossHeight = -5f;
    private bool _isGameOver = false;

    [Header("UI")]
    public GameObject gameOverPanel;
    public Text gameOverText;
    public Button restartButton;

    void Start()
    {
        gameOverPanel.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);
    }
    void Update()
    {
        if (towerBase.position.y < lossHeight)
        {
            //Debug.Log("Game Over! Tower collapsed.");
            // Add game over UI or restart logic
            GameOver();
        }
    }
    public void GameOver()
    {
        _isGameOver = true;
        gameOverPanel.SetActive(true);
        gameOverText.text = "Game Over! Tower collapsed.";
        Debug.Log("Game Over! Tower collapsed.");
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

