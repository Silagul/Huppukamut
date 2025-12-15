using UnityEngine;

public class AudioVolumeManager : MonoBehaviour
{
    public static AudioVolumeManager Instance;

    [Header("Volume Levels")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";

    private void Awake()
    {
        // Singleton pattern + persist across scenes
        if (Instance != null && Instance != this)
        {
            // If another instance exists, copy its values to this one (in case order changes)
            musicVolume = Instance.musicVolume;
            sfxVolume = Instance.sfxVolume;
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Load saved volumes
        musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
        sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);

        // Apply SFX volume to SoundManager as soon as it's available
        if (SoundManager.instance != null)
            SoundManager.instance.SetSFXVolume(sfxVolume);
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolume);
        PlayerPrefs.Save();

        // Update menu music (add gameplay music controller here if you have one)
        MenuMusicController.Instance?.UpdateVolume();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolume);
        PlayerPrefs.Save();

        // Immediately update SoundManager
        SoundManager.instance?.SetSFXVolume(sfxVolume);
    }

    public float GetMusicVolume() => musicVolume;
    public float GetSFXVolume() => sfxVolume;
}