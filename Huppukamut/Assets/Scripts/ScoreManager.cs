using TMPro;
using UnityEngine;
//using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;


    public TextMeshProUGUI ScoreText;

    public int score = 0;


    public int CurrentScore => score;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ScoreText.text = score.ToString() + "";
    }

    public void AddPoint(int amount)
    {
        score += amount;
        ScoreText.text = score.ToString() + "";
    }
    public void SaveScore()
    {
        PlayerPrefs.SetInt("FinalScore", score);
        PlayerPrefs.Save();
        Debug.Log("Tallennettu score: " + score);
    }

}
