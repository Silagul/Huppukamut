using UnityEngine;
using UnityEngine.UI;

public class HelpeeTracker : MonoBehaviour
{
    public GameObject iconPrefab;
    private GameObject[] helpees;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        helpees = GameObject.FindGameObjectsWithTag("Helpee");
        for (int i = 0; i < helpees.Length; i++)
        {
            HelpeeUI hui = helpees[i].transform.GetComponentInChildren<HelpeeUI>();

            GameObject o = Instantiate(iconPrefab, gameObject.transform);

            Image imageGray = o.GetComponent<Image>();
            imageGray.sprite = hui.iconGray;

            Image image = o.transform.GetComponentsInChildren<Image>()[1];
            image.sprite = hui.icon;

            RectTransform rec = o.GetComponent<RectTransform>();

            if (i < 5)
            {
                rec.anchoredPosition = new Vector2(0, 0 + (i * 100));
            }
            else
            {
                rec.anchoredPosition = new Vector2(100, 0 + ((i - 5) * 100));
            }

            image.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //
    }
}
