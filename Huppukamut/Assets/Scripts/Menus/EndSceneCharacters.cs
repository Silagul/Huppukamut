using UnityEngine;

public class EndSceneCharacters : MonoBehaviour
{
    public HelpeeCollection helpeeCollection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string[] names = helpeeCollection.characters;
        for (int i = 0; i < names.Length; i++)
        {
            if (helpeeCollection.rescued[i] == false)
            {
                GameObject.Find(names[i]).SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //
    }
}
