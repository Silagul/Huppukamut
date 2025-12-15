using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Gameplay SFX")]
    public AudioClip collect;
    public AudioClip stomp;
    public AudioClip dash;
    public AudioClip glideStart;
    public AudioClip glideLoop;
    public AudioClip boxBreak;
    public AudioClip jumpSound;
    public AudioClip landSound;

    [Header("Helping Friend - Random Cheers")]
    public AudioClip[] helpFriendVariations;

    [Header("UI SFX")]
    public AudioClip uiClick; // Assign your button click sound here in the Inspector

    [Header("Settings")]
    [Range(0f, 1f)]
    public float sfxVolume = 1f; // Live volume, controlled by AudioVolumeManager

    private AudioSource oneShotSource;
    private AudioSource loopSource;

    private void Awake()
    {
        // Robust singleton: if another instance exists, destroy this one
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        // Setup AudioSources
        oneShotSource = gameObject.AddComponent<AudioSource>();
        loopSource = gameObject.AddComponent<AudioSource>();
        loopSource.loop = true;
        loopSource.playOnAwake = false;

        // Subscribe to scene loading to re-sync volume
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Initial volume sync
        UpdateSFXVolumeFromManager();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ensure volume is correct every time a new scene loads
        UpdateSFXVolumeFromManager();
    }

    private void UpdateSFXVolumeFromManager()
    {
        if (AudioVolumeManager.Instance != null)
        {
            sfxVolume = AudioVolumeManager.Instance.GetSFXVolume();
        }
        else
        {
            // Fallback to saved value if manager not ready
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        }

        // Update any currently looping sound (e.g. glide)
        if (loopSource != null && loopSource.isPlaying)
        {
            loopSource.volume = sfxVolume;
        }
    }

    // Internal method — keeps core playback logic encapsulated
    private void PlaySFX(AudioClip clip, float volumeScale = 1f)
    {
        if (clip != null && oneShotSource != null)
        {
            oneShotSource.PlayOneShot(clip, volumeScale * sfxVolume);
        }
    }

    // Public method — safe for external scripts (e.g. UIButtonSound) to play custom clips
    public void PlayOneShot(AudioClip clip, float volumeScale = 1f)
    {
        PlaySFX(clip, volumeScale);
    }

    // === All your gameplay sound methods ===
    public void PlayCollect() => PlaySFX(collect);
    public void PlayStomp() => PlaySFX(stomp);
    public void Dash() => PlaySFX(dash);
    public void GlideStart() => PlaySFX(glideStart);
    public void BoxBreak() => PlaySFX(boxBreak);
    public void PlayJump() => PlaySFX(jumpSound);
    public void PlayLand() => PlaySFX(landSound);

    public void GlideLoop(bool play)
    {
        if (glideLoop == null) return;

        if (play && !loopSource.isPlaying)
        {
            loopSource.clip = glideLoop;
            loopSource.volume = sfxVolume;
            loopSource.Play();
        }
        else if (!play && loopSource.isPlaying)
        {
            loopSource.Stop();
        }
    }

    public void PlayHelpFriend()
    {
        if (helpFriendVariations == null || helpFriendVariations.Length == 0) return;
        int index = Random.Range(0, helpFriendVariations.Length);
        PlaySFX(helpFriendVariations[index]);
    }

    // Default UI button click sound
    public void PlayUIClick()
    {
        PlaySFX(uiClick, 0.8f); // Slightly quieter than full volume — adjust as needed
    }

    // Called by AudioVolumeManager when the SFX slider changes
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;

        // Update looping sounds immediately
        if (loopSource != null && loopSource.isPlaying)
        {
            loopSource.volume = sfxVolume;
        }
    }

    private void OnDestroy()
    {
        // Clean up the event subscription
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}