using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;


public class GUIController : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    [SerializeField] private Text highscoreText;
    [SerializeField] private Text realtimeScoreText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject holdStartText;


    private float _score;



    public void HandleRestartButton()
    {
        SceneManager.LoadScene("Game");
    }
   public void StartGame()
    {
        holdStartText.SetActive(false);
    }
    public void GameOver()
    {
        realtimeScoreText.gameObject.SetActive(false);
        if (PlayerPrefs.HasKey("HighScore"))
        {
            scoreText.text = _score.ToString("0.00");

            float highestScore = PlayerPrefs.GetFloat("HighScore");
            if (_score > highestScore)
            {
                PlayerPrefs.SetFloat("HighScore", _score);
                highscoreText.text = "HighestScore: " + _score.ToString("0.00");
            }
            else
            {
                highscoreText.text = "HighestScore: " + highestScore.ToString("0.00");
            }
        }
        else
        {
            PlayerPrefs.SetFloat("HighScore", _score);
            highscoreText.text = "HighestScore: " + _score.ToString("0.00");
        }
        gameOverPanel.SetActive(true);
    }
    public void SetScore(float addedPoint)
    {
        _score += addedPoint;
        realtimeScoreText.text = _score.ToString("0.00");
    }
}
