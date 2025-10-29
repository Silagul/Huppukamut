using System;
using UnityEngine;

public class CharacterSelectController : MonoBehaviour
{
    public GameObject platform;
    public GameObject[] characters;
    public TMPro.TextMeshProUGUI nameDisplay;
    public TMPro.TextMeshProUGUI description;
    public string[,] characterData;
    public int current = 0;
    public bool clockwise;
    public bool spinning = false;
    public float rotationTime;
    private float rotationTimer;
    private float step;
    private float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rotationTimer = 0f;

        nameDisplay = GameObject.Find("CharName").GetComponent<TMPro.TextMeshProUGUI>();
        description = GameObject.Find("CharDescription").GetComponent<TMPro.TextMeshProUGUI>();

        platform = GameObject.Find("Platform");

        characters = GameObject.FindGameObjectsWithTag("Player");
        step = 360 / characters.Length;
        speed = step / rotationTime;

        characterData = new string[characters.Length, 2];
        for (int i = 0; i < characters.Length; i++)
        {
            characterData[i, 0] = characters[i].name;
            characterData[i, 1] = characters[i].name + " description";

            characters[i].transform.position = platform.transform.position + new Vector3(0, 1.05f, -2.5f);
            characters[i].transform.RotateAround(platform.transform.position, Vector3.up, step * i);
        }

        nameDisplay.text = characterData[0, 0];
        description.text = characterData[0, 1];

        //characterData = { { "Character 1 name", "Character 1 description" }, { "Character 2 name", "Character 2 description" }, { "Character 3 name", "Character 3 description" }, { "Character 4 name", "Character 4 description" } };
    }

    // Update is called once per frame
    void Update()
    {
        rotationTimer -= Time.deltaTime;
        if (rotationTimer <= 0f)
        {
            spinning = false;
        }
        if (spinning)
        {
            foreach (GameObject character in characters)
            {
                if (clockwise)
                {
                    character.transform.RotateAround(platform.transform.position, Vector3.up, speed * Time.deltaTime);
                }
                else
                {
                    character.transform.RotateAround(platform.transform.position, Vector3.up, -1 * speed * Time.deltaTime);
                }
            }
        }
    }

    public void RotateSelection(bool cw)
    {
        if (!spinning)
        {
            spinning = true;
            clockwise = cw;
            rotationTimer = rotationTime;
            if (cw)
            {
                if (current == 0)
                {
                    current = characters.Length - 1;
                }
                else
                {
                    current--;
                }
            }
            else
            {
                if (current == characters.Length - 1)
                {
                    current = 0;
                }
                else
                {
                    current++;
                }
            }
            nameDisplay.text = characterData[current, 0];
            description.text = characterData[current, 1];
        }
    }
}
