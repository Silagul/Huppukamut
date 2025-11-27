using UnityEngine;

public class Credits : MonoBehaviour
{
    public float timePerCredit = 4f;

    private int index = 0;

    void Start()
    {
        // Hide all
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);

        // Show first one IMMEDIATELY
        if (transform.childCount > 0)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            InvokeRepeating(nameof(NextCredit), timePerCredit, timePerCredit);  // starts exactly after timePerCredit
        }
    }

    void NextCredit()
    {
        // Hide current
        transform.GetChild(index).gameObject.SetActive(false);

        // Go to next
        index = (index + 1) % transform.childCount;

        // Show next instantly
        transform.GetChild(index).gameObject.SetActive(true);
    }
}