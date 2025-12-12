using UnityEngine;
using TMPro;
using System;

public class timer : MonoBehaviour
{
    public static timer Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI timeText;

    private float elapsedTime;
    private bool isRunning = false;

    // These survive scene changes
    public static float FinalTime { get; private set; } = 0f;
    public static string FinalTimeFormatted { get; private set; } = "00:00";

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        elapsedTime = 0f;
        isRunning = true;
        FinalTime = 0f;
        FinalTimeFormatted = "00:00";
        FindText();
    }

    private void OnDisable()
    {
        FinalTime = elapsedTime;
        TimeSpan t = TimeSpan.FromSeconds(elapsedTime);
        FinalTimeFormatted = string.Format("{0:00}:{1:00}", t.Minutes, t.Seconds);

        Debug.Log($"[timer] Final time saved: {FinalTimeFormatted} ({FinalTime:F2}s)");
    }

    void Update()
    {
        FindText(); // your original "always safe" call

        if (isRunning)
            elapsedTime += Time.deltaTime;

        if (timeText != null)
        {
            TimeSpan t = TimeSpan.FromSeconds(elapsedTime);
            timeText.text = string.Format("{0:00}:{1:00}", t.Minutes, t.Seconds);
        }
    }

    private void FindText()
    {
        if (timeText == null)
        {
            GameObject obj = GameObject.Find("Canvas/In game/Time");
            if (obj != null)
                timeText = obj.GetComponent<TextMeshProUGUI>();
        }
    }

    // YOUR ORIGINAL PAUSE/RESUME METHODS – 100% unchanged
    public void Pause() => isRunning = false;
    public void Resume() => isRunning = true;
    public float GetElapsedTime() => elapsedTime;
    public string GetFormattedTime()
    {
        TimeSpan t = TimeSpan.FromSeconds(elapsedTime);
        return string.Format("{0:00}:{1:00}", t.Minutes, t.Seconds);
    }
}