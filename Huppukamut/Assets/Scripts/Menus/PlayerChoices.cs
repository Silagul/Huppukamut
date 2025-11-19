using UnityEngine;

[CreateAssetMenu(fileName = "PlayerChoices", menuName = "Scriptable Objects/PlayerChoices")]
public class PlayerChoices : ScriptableObject
{
    public float volume;
    public string characterName = null;

    public void SetCharacterName(string charName)
    {
        characterName = charName;
    }

    public void SetVolume(float vol)
    {
        volume = vol;
    }
}
