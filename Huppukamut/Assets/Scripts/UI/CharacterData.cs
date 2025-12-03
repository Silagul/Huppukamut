using UnityEngine;
using UnityEngine.Localization.Settings;

public class CharacterData : MonoBehaviour
{
    [Header("Data")]
    public string charName;
    public string description;
    public string charNameFi;
    public string descriptionFi;
    public string charNameEn;
    public string descriptionEn;

    [Header("Ability")]
    public string abilityName;
    public string abilityDescription;
    public string abilityNameFi;
    public string abilityDescriptionFi;
    public string abilityNameEn;
    public string abilityDescriptionEn;
    public Sprite abilityIcon;
    public Sprite graffiti;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        /*var locales = LocalizationSettings.AvailableLocales.Locales;
        for (int i = 0; i < locales.Count; ++i)
        {
            print(locales[i].LocaleName); // "English (en)", "Finnish (fi)"
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        if (LocalizationSettings.SelectedLocale.LocaleName == "English (en)")
        {
            charName = charNameEn;
            description = descriptionEn;
            abilityName = abilityNameEn;
            abilityDescription = abilityDescriptionEn;
        }

        if (LocalizationSettings.SelectedLocale.LocaleName == "Finnish (fi)")
        {
            charName = charNameFi;
            description = descriptionFi;
            abilityName = abilityNameFi;
            abilityDescription = abilityDescriptionFi;
        }
    }
}
