using UnityEngine;

public class MusicVolumeManager : MonoBehaviour
{
    public static MusicVolumeManager Instance;

    [Range(0f, 1f)]
    public float musicVolume = 1f;

    private const string PlayerPrefsKey = "MusicVolume";

    private void Awake()
    {
        // Singleton + persist across scenes
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Load saved volume (default = 1)
        musicVolume = PlayerPrefs.GetFloat(PlayerPrefsKey, 1f);
    }

    // Called by your UI Slider
    public void SetVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(PlayerPrefsKey, musicVolume);
        PlayerPrefs.Save();

        // Instantly update any playing music
        MenuMusicController.Instance?.UpdateVolume();
        // If you later have GameplayMusicController, add: GameplayMusicController.Instance?.UpdateVolume();
    }

    public float GetVolume() => musicVolume;
}