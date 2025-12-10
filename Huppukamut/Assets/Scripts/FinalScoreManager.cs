using UnityEngine;
using TMPro;

public class FinalScoreManager : MonoBehaviour
{
    [Header("Auto-found Text Components – no need to assign")]
    private TextMeshProUGUI timeText;
    private TextMeshProUGUI scoreText;

    void Start()
    {
        FindTextComponents();

        // Show final time
        if (timeText != null)
            timeText.text = "Time: " + timer.FinalTimeFormatted;

        // Calculate and show score (you can change the formula as you like)
        int calculatedScore = CalculateScore();

        if (scoreText != null)
            scoreText.text = "Score: " + calculatedScore.ToString("N0"); // N0 = with commas
    }

    private void FindTextComponents()
    {
        // CHANGE THESE PATHS to match your exact hierarchy in the results scene
        GameObject timeObj = GameObject.Find("Canvas/TotalTime");           // ← your time text
        GameObject scoreObj = GameObject.Find("Canvas/TotalScore");         // ← your score text

        if (timeObj != null)
            timeText = timeObj.GetComponent<TextMeshProUGUI>();

        if (scoreObj != null)
            scoreText = scoreObj.GetComponent<TextMeshProUGUI>();

        // Optional fallback if names change
        if (timeText == null) timeText = GameObject.Find("Time")?.GetComponent<TextMeshProUGUI>();
        if (scoreText == null) scoreText = GameObject.Find("Score")?.GetComponent<TextMeshProUGUI>();
    }

    private int CalculateScore()
    {
        // Example formulas – pick one or make your own!

        // 1. Faster = higher score (recommended)
        float time = timer.FinalTime;
        int score = Mathf.Max(100000 - (int)(time * 100), 0);        // 1000 points per second penalty

        // 2. Or bonus for very fast times
        // int score = time < 60 ? 50000 : time < 120 ? 30000 : 10000;

        // 3. Or fixed points based on time brackets
        // int score = time < 30 ? 100000 : time < 60 ? 75000 : time < 120 ? 50000 : 25000;

        return score;
    }
}