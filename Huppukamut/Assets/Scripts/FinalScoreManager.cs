using UnityEngine;
using TMPro;

public class FinalScoreManager : MonoBehaviour
{
    [Header("Drag your TextMeshPro objects here")]
    [SerializeField] private TextMeshProUGUI timeText;         // TimeNumber
    [SerializeField] private TextMeshProUGUI scoreText;        // ScoreNumber (optional – shows time bonus only)
    [SerializeField] private TextMeshProUGUI finalScoreText;   // FinalScoreNumber ← main one

    void Start()
    {
        // Auto-find fallback
        if (timeText == null || scoreText == null || finalScoreText == null)
            FindTextComponents();

        // 1. Show time
        string displayTime = timer.FinalTimeFormatted;
        float rawTime = timer.FinalTime;

        if (timeText != null)
            timeText.text = "" + displayTime;

        // 2. Time bonus → round to nearest 50
        int rawTimeScore = Mathf.Max(50000 - (int)(rawTime * 120), 0);
        int timeScoreRounded = Mathf.RoundToInt(rawTimeScore / 50f) * 50;

        if (scoreText != null)
            scoreText.text = " = " + timeScoreRounded.ToString("N0"); // e.g. 33,650 or 33,700

        // 3. Collectible score from intern's system (untouched)
        int collectibleScore = ScoreManager.instance?.CurrentScore ?? 0;

        // 4. Total → round to nearest 50
        int totalBeforeRound = timeScoreRounded + collectibleScore;
        int finalScoreRounded = Mathf.RoundToInt(totalBeforeRound / 50f) * 50;

        // 5. Show final total on FinalScoreNumber
        if (finalScoreText != null)
            finalScoreText.text = finalScoreRounded.ToString("N0");

        // Debug
        Debug.Log($"[FinalScore] Time Bonus: {timeScoreRounded:N0} | " +
                  $"Collectibles: {collectibleScore:N0} | " +
                  $"FINAL TOTAL (rounded to 50): {finalScoreRounded:N0}");
    }

    private void FindTextComponents()
    {
        if (timeText == null)
            timeText = GameObject.Find("Canvas/Score Numbers/TimeNumber")?.GetComponent<TextMeshProUGUI>();

        if (scoreText == null)
            scoreText = GameObject.Find("Canvas/Score Numbers/ScoreNumber")?.GetComponent<TextMeshProUGUI>();

        if (finalScoreText == null)
            finalScoreText = GameObject.Find("Canvas/Score Numbers/FinalScoreNumber")?.GetComponent<TextMeshProUGUI>();

        // Extra safety
        var all = FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var t in all)
        {
            if (timeText == null && t.name.Contains("Time")) timeText = t;
            if (scoreText == null && t.name.Contains("Score") && !t.name.Contains("Final")) scoreText = t;
            if (finalScoreText == null && t.name.Contains("FinalScore")) finalScoreText = t;
        }
    }
}