using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class LevelMusicPlayer : MonoBehaviour
{
    public static LevelMusicPlayer Instance;

    [Header("Music Settings")]
    [SerializeField] private AudioClip[] songs;
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("Current Status (Read-Only)")]
    [SerializeField] private int currentSongIndex;
    [SerializeField] private float fadeAlpha;
    [SerializeField] private float masterVolume;

    private AudioSource audioSource;
    private string prefsKey;

    private void Awake()
    {
        // Singleton (prevents multiples in same scene)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        fadeAlpha = 0f;
    }

    private void Start()
    {
        // Per-level save key (e.g., "LevelMusicSongIndex_Level01")
        prefsKey = $"LevelMusicSongIndex_{SceneManager.GetActiveScene().name}";

        if (songs == null || songs.Length == 0)
        {
            return; // No music
        }

        // Load saved index (or default 0)
        currentSongIndex = PlayerPrefs.GetInt(prefsKey, 0);
        if (currentSongIndex >= songs.Length)
            currentSongIndex = 0;

        // Fade in the loaded song
        StartCoroutine(SwitchSong(currentSongIndex));
    }

    private void Update()
    {
        if (songs == null || songs.Length == 0 || audioSource == null)
            return;

        // Live volume sync (instant slider response)
        masterVolume = MusicVolumeManager.Instance != null ? MusicVolumeManager.Instance.GetVolume() : 1f;
        audioSource.volume = masterVolume * fadeAlpha;
    }

    // UI Button: Next Song (cycles forward)
    public void NextSong()
    {
        if (songs == null || songs.Length == 0)
            return;

        int nextIndex = (currentSongIndex + 1) % songs.Length;
        StartCoroutine(SwitchSong(nextIndex));
    }

    // UI Button: Previous Song (cycles backward)
    public void PreviousSong()
    {
        if (songs == null || songs.Length == 0)
            return;

        int prevIndex = currentSongIndex - 1;
        if (prevIndex < 0)
            prevIndex = songs.Length - 1;
        StartCoroutine(SwitchSong(prevIndex));
    }

    // UI Button: Random Song
    public void RandomSong()
    {
        if (songs == null || songs.Length == 0)
            return;

        int randomIndex = Random.Range(0, songs.Length);
        StartCoroutine(SwitchSong(randomIndex));
    }

    // Internal: Seamless switch with fade out → change → fade in
    private IEnumerator SwitchSong(int newIndex)
    {
        if (newIndex >= songs.Length || newIndex < 0)
            yield break;

        // Fade out current
        yield return StartCoroutine(FadeOut());

        // Switch clip
        audioSource.clip = songs[newIndex];
        audioSource.Play();

        // Update & save
        currentSongIndex = newIndex;
        PlayerPrefs.SetInt(prefsKey, currentSongIndex);
        PlayerPrefs.Save();

        // Fade in new
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeOut()
    {
        while (fadeAlpha > 0f)
        {
            fadeAlpha -= Time.unscaledDeltaTime / fadeDuration;
            if (fadeAlpha < 0f)
                fadeAlpha = 0f;
            yield return null;
        }
    }

    private IEnumerator FadeIn()
    {
        while (fadeAlpha < 1f)
        {
            fadeAlpha += Time.unscaledDeltaTime / fadeDuration;
            if (fadeAlpha > 1f)
                fadeAlpha = 1f;
            yield return null;
        }
    }

    // For UI display
    public string CurrentSongName => (songs != null && currentSongIndex < songs.Length) ? songs[currentSongIndex].name : "No Music";

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}