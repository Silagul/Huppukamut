using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public TextMeshProUGUI ScoreText;

    int score = 0;


    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ScoreText.text = score.ToString() + "";
    }

    public void AddPoint()
    {
        score += 1;
        ScoreText.text = score.ToString() + "";
    }
}
