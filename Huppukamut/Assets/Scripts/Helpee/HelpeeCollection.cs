using UnityEngine;

[CreateAssetMenu(fileName = "HelpeeCollection", menuName = "Scriptable Objects/HelpeeCollection")]
public class HelpeeCollection : ScriptableObject
{
    public string[] characters = new string[6];
    public bool[] rescued = new bool[6];
    //public int number = 0;

    public void ListCharacters(GameObject[] ch)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i] = ch[i].name;
        }

        for (int i = 0; i < rescued.Length; i++)
        {
            rescued[i] = false;
        }
    }
    
    public void CharcterRescued(string charName)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i] == charName)
            {
                rescued[i] = true;
                //number++;
            }
        }
    }
}
