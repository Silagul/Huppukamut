using UnityEngine;
using TMPro;

public class FinalScoreManager : MonoBehaviour
{
    [Header("Drag your TextMeshPro objects here")]
    [SerializeField] private TextMeshProUGUI timeText;           // TimeNumber
    [SerializeField] private TextMeshProUGUI friendsText;        // FriendsNumber → "3 × 2000 = 6,000"
    [SerializeField] private TextMeshProUGUI timeBonusText;      // TimeBonusNumber
    [SerializeField] private TextMeshProUGUI collectiblesText;   // CollectiblesNumber → "30 × 200 = 6,000"
    [SerializeField] private TextMeshProUGUI finalScoreText;     // FinalScoreNumber

    [Header("Drag your HelpeeCollection asset here")]
    [SerializeField] private HelpeeCollection helpeeCollection;

    void Start()
    {
        // Fallback if not assigned
        if (timeText == null || friendsText == null || timeBonusText == null ||
            collectiblesText == null || finalScoreText == null)
            FindTextComponents();

        // 1. Time
        string displayTime = timer.FinalTimeFormatted;
        float rawTime = timer.FinalTime;

        if (timeText != null)
            timeText.text = displayTime;

        // 2. Time bonus (rounded to 50)
        int rawTimeScore = Mathf.Max(30000 - (int)(rawTime * 100), 0);
        int timeBonus = Mathf.RoundToInt(rawTimeScore / 50f) * 50;

        if (timeBonusText != null)
            timeBonusText.text = timeBonus.ToString("N0");

        // 3. Friends count
        int actualFriendsHelped = 0;
        for (int i = 0; i < helpeeCollection.rescued.Length; i++)
        {
            if (helpeeCollection.rescued[i]) actualFriendsHelped++;
        }

        int displayedFriends = Mathf.Max(0, actualFriendsHelped - 1);
        int displayedFriendsBonus = displayedFriends * 2000;

        if (friendsText != null)
            friendsText.text = $"{displayedFriends} × 2000 = {displayedFriendsBonus.ToString("N0")}";

        // 4. Collectibles: subtract only the DISPLAYED friends bonus
        int rawScoreFromManager = ScoreManager.instance?.CurrentScore ?? 0;
        int cleanCollectiblePoints = Mathf.Max(0, rawScoreFromManager - displayedFriendsBonus);

        int collectedItemsCount = cleanCollectiblePoints / 200;

        if (collectiblesText != null)
            collectiblesText.text = $"{collectedItemsCount} × 200 = {cleanCollectiblePoints.ToString("N0")}";

        // 5. Final score = time + displayed friends + clean collectibles → round to 50
        int total = timeBonus + displayedFriendsBonus + cleanCollectiblePoints;
        int finalRounded = Mathf.RoundToInt(total / 50f) * 50;

        if (finalScoreText != null)
            finalScoreText.text = finalRounded.ToString("N0");

        // Debug - matches your example exactly
        Debug.Log($"[FinalScore Breakdown]\n" +
                  $"Actual Friends Helped: {actualFriendsHelped}\n" +
                  $"Displayed Friends: {displayedFriends} → {displayedFriendsBonus:N0}\n" +
                  $"Raw ScoreManager: {rawScoreFromManager:N0}\n" +
                  $"Clean Collectibles: {collectedItemsCount} items → {cleanCollectiblePoints:N0}\n" +
                  $"Time Bonus: {timeBonus:N0}\n" +
                  $"FINAL SCORE: {finalRounded:N0}");
    }

    private void FindTextComponents()
    {
        var all = FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        if (timeText == null)         timeText = System.Array.Find(all, t => t.name.Contains("Time") && !t.name.Contains("Bonus"));
        if (friendsText == null)      friendsText = System.Array.Find(all, t => t.name.Contains("Friends"));
        if (timeBonusText == null)    timeBonusText = System.Array.Find(all, t => t.name.Contains("TimeBonus") || t.name.Contains("Time Score"));
        if (collectiblesText == null) collectiblesText = System.Array.Find(all, t => t.name.Contains("Collectibles"));
        if (finalScoreText == null)   finalScoreText = System.Array.Find(all, t => t.name.Contains("FinalScore"));
    }
}