using UnityEngine;
using TMPro;
using System;

public class timer : MonoBehaviour
{
    public static timer Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI timeText;

    private float elapsedTime;
    private bool isRunning = false;

    // These static values carry the final time to the next scene
    public static float FinalTime { get; private set; }
    public static string FinalTimeFormatted { get; private set; }

    private void Awake()
    {
        // New instance every time you enter the level → fresh timer
        Instance = this;
    }

    private void OnEnable()
    {
        // Auto-start fresh every time level loads
        elapsedTime = 0f;
        isRunning = true;
        FindText();
    }

    private void OnDisable()
    {
        // When leaving level → save final time for results scene
        FinalTime = elapsedTime;
        TimeSpan t = TimeSpan.FromSeconds(elapsedTime);
        FinalTimeFormatted = string.Format("{0:00}:{1:00}", t.Minutes, t.Seconds);
    }

    void Update()
    {
        FindText(); // Always safe

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

    // YOUR EXISTING PAUSE MENU CODE STILL WORKS 100% WITH THESE:
    public void Pause()  => isRunning = false;
    public void Resume() => isRunning = true;

    public float GetElapsedTime() => elapsedTime;

    public string GetFormattedTime()
    {
        TimeSpan t = TimeSpan.FromSeconds(elapsedTime);
        return string.Format("{0:00}:{1:00}", t.Minutes, t.Seconds);
    }
}