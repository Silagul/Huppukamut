using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectController : MonoBehaviour
{
    public GameObject platform;
    public GameObject[] characters;
    public PlayerChoices playerChoices;
    public TMPro.TextMeshProUGUI nameDisplay;
    public TMPro.TextMeshProUGUI nameDisplay2;
    public TMPro.TextMeshProUGUI description;
    public TMPro.TextMeshProUGUI abilityName;
    public TMPro.TextMeshProUGUI abilityDescription;
    public Image abilityIcon;
    public string[,] characterData;
    public int current = 0;
    public bool clockwise;
    public bool spinning = false;
    public float rotationTime;
    public float selectDelay;
    private float selectTimer;
    private bool ticking = false;
    private float rotationTimer;
    private float step;
    private float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rotationTimer = 0f;
        selectTimer = selectDelay;

        nameDisplay = GameObject.Find("CharName1").GetComponent<TMPro.TextMeshProUGUI>();
        nameDisplay2 = GameObject.Find("CharName2").GetComponent<TMPro.TextMeshProUGUI>();
        description = GameObject.Find("CharDescription").GetComponent<TMPro.TextMeshProUGUI>();

        abilityName = GameObject.Find("AbilityName").GetComponent<TMPro.TextMeshProUGUI>();
        abilityDescription = GameObject.Find("AbilityDescription").GetComponent<TMPro.TextMeshProUGUI>();
        abilityIcon = GameObject.Find("AbilityIcon").GetComponent<Image>();

        platform = GameObject.Find("Platform");

        characters = GameObject.FindGameObjectsWithTag("Player");
        step = 360 / characters.Length;
        speed = step / rotationTime;

        characterData = new string[characters.Length, 2];
        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i].TryGetComponent<Animator>(out Animator animator))
            {
                animator.SetFloat("Stamina", 1);
            }

            characterData[i, 0] = characters[i].name;
            characterData[i, 1] = characters[i].name + " description";

            characters[i].transform.position = platform.transform.position + new Vector3(0, 0.1f, -2.5f);
            characters[i].transform.RotateAround(platform.transform.position, Vector3.up, step * i);
            characters[i].transform.RotateAround(characters[i].transform.position, Vector3.up, 180);
        }

        if (characters[0].TryGetComponent<CharacterData>(out CharacterData cd))
        {
            nameDisplay.text = cd.charName;
            nameDisplay2.text = cd.charName;
            description.text = cd.description;

            abilityName.text = cd.abilityName;
            abilityDescription.text = cd.abilityDescription;
            abilityIcon.sprite = cd.abilityIcon;
        }
        else
        {
            nameDisplay.text = characterData[0, 0];
            nameDisplay2.text = characterData[0, 0];
            description.text = characterData[0, 1];
        }
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

        if (ticking)
        {
            selectTimer -= Time.deltaTime;
        }

        if (selectTimer <= 0f)
        {
            LoadLevel("Level1");
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

            if (characters[current].TryGetComponent<CharacterData>(out CharacterData cd))
            {
                nameDisplay.text = cd.charName;
                nameDisplay2.text = cd.charName;
                description.text = cd.description;

                abilityName.text = cd.abilityName;
                abilityDescription.text = cd.abilityDescription;
                abilityIcon.sprite = cd.abilityIcon;
            }
            else
            {
                nameDisplay.text = characterData[current, 0];
                nameDisplay2.text = characterData[current, 0];
                description.text = characterData[current, 1];
            }
        }
    }

    public void SelectCharacter()
    {
        if (characters[current].TryGetComponent<Animator>(out Animator animator))
        {
            animator.SetTrigger("Pose");
        }
        else
        {
            //
        }
        playerChoices.SetCharacterName(characters[current].gameObject.name);
        ticking = true;
    }

    public void LoadLevel(string name)
    {
        SceneManager.LoadScene(name);
    }
}
