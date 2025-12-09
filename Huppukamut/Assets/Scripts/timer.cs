using UnityEngine;
using TMPro;

public class timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText; // renamed for clarity
    
    private float elapsedTime;

    void Update()
    {
        elapsedTime += Time.deltaTime;

        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}