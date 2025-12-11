using UnityEngine;
using TMPro;

public class EndScoreDisplay : MonoBehaviour
{
   

    public int score;
    public TMP_Text scoreText;

    void Start()
    {
        PlayerPrefs.SetInt("FinalScore", score);

        PlayerPrefs.SetInt("FinalScore", ScoreManager.instance.CurrentScore);

        int finalScore = PlayerPrefs.GetInt("FinalScore", 0);
        scoreText.text = "Score: " + finalScore;
        scoreText.text = "" + finalScore;

    }
}
