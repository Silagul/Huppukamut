using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    private static MusicPlayer instance;
    private AudioSource audioSource;

    [SerializeField] private string[] allowedScenes = { "MainMenu", "SelectCharacter" };
    [SerializeField] private string stopScene = "Level1_copy";

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();

        if (IsSceneAllowed(SceneManager.GetActiveScene().name))
            audioSource.Play();

        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        if (newScene.name == stopScene)
        {
            audioSource.Stop();
            Destroy(gameObject);
        }
        else if (IsSceneAllowed(newScene.name) && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    bool IsSceneAllowed(string sceneName)
    {
        foreach (string s in allowedScenes)
        {
            if (s == sceneName)
                return true;
        }
        return false;
    }

    // 🔹 Uusi metodi, joka hiljentää musiikin
    public void MuteMusic()
    {
        if (audioSource != null)
            audioSource.Stop();
    }
}
