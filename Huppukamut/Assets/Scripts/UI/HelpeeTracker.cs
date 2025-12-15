using UnityEngine;
using UnityEngine.UI;

public class HelpeeTracker : MonoBehaviour
{
    public GameObject iconPrefab;
    public int numberRescued = 0;
    public PlayerChoices playerChoices;

    private GameObject[] helpees;
    private GameObject[] icons;

    private Image imageGray;
    private Image image;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        helpees = GameObject.FindGameObjectsWithTag("Helpee");

        for (int i = 0; i < helpees.Length; i++)
        {
            GameObject character = helpees[i].transform.GetComponentInChildren<Animator>().gameObject;
            if (character.name == playerChoices.characterName)
            {
                helpees[i].SetActive(false);
            }
        }

        helpees = GameObject.FindGameObjectsWithTag("Helpee");
        print(helpees.Length);
        icons = new GameObject[helpees.Length];

        for (int i = 0; i < helpees.Length; i++)
        {
            HelpeeUI hui = helpees[i].transform.GetComponentInChildren<HelpeeUI>();

            GameObject o = Instantiate(iconPrefab, gameObject.transform);

            GetIconComponents(o);
            imageGray.sprite = hui.iconGray;
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
            icons[i] = o;

            image.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //
    }

    public void GetIconComponents(GameObject icon)
    {
        imageGray = icon.GetComponent<Image>();
        image = icon.transform.Find("Image").GetComponent<Image>();
    }

    public void HelpeeRescued(GameObject helpee)
    {
        for (int i = 0; i < helpees.Length; i++)
        {
            if (helpees[i] == helpee)
            {
                GameObject c = icons[i];
                GetIconComponents(c);
                image.gameObject.SetActive(true);
                numberRescued++;
            }
        }
    }
}
