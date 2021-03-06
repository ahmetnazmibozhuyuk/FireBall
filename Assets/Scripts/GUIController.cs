using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

//Rather than exposing Text and GameObject variables to other classes and making the code less safe, I used [SerializeField] attribute
//to add the necessary elements on from inspector.

//Methods that are related to GUI control are moved to this class to make both classes more self contained.
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
        gameOverPanel.SetActive(true);

        //nested if rewritten to be more readable

        if (!PlayerPrefs.HasKey("HighScore") || _score > PlayerPrefs.GetFloat("HighScore"))
        {
            NewHighScore();
        }
        else
        {
            DisplayHighScore();
        }
        DisplayScore();
    }
    //score display moved to their respective methods.
    private void DisplayScore()
    {
        scoreText.text = _score.ToString("0.00");
    }
    private void DisplayHighScore()
    {
        highscoreText.text = "HighestScore: " + PlayerPrefs.GetFloat("HighScore").ToString("0.00");
    }
    private void NewHighScore()
    {
        PlayerPrefs.SetFloat("HighScore", _score);
        highscoreText.text = "HighestScore: " + _score.ToString("0.00");
    }
    public void SetScore(float addedPoint)
    {
        _score += addedPoint;
        realtimeScoreText.text = _score.ToString("0.00");
    }
}
