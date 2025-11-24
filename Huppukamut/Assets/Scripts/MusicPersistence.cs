using UnityEngine;

public class MusicPersistence : MonoBehaviour
{
    public static MusicPersistence Instance;

    void Awake()
    {
        // Check if there's already an instance of this object
        if (Instance == null)
        {
            // If not, make this the instance
            Instance = this;
            // Don't destroy this object when a new scene is loaded
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If an instance already exists, destroy the new one to prevent duplicates
            Destroy(gameObject);
        }
    }
}
