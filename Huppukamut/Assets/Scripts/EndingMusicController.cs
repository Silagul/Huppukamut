using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class EndingMusicController : MonoBehaviour
{
    public static EndingMusicController Instance;

    [Header("Settings")]
    [SerializeField] private AudioClip endingMusicClip;
    [SerializeField] private float fadeInDuration = 2.5f;
    [SerializeField] private float fadeOutDuration = 2f;

    private AudioSource audioSource;
    private bool hasStartedPlaying = false;
    private const string EndingSceneName = "EndingScene"; // Change if needed

    private void Awake()
    {
        // --- ROBUST SINGLETON: Prevent duplicates aggressively ---
        if (Instance != null && Instance != this)
        {
            // Duplicate detected → destroy THIS object immediately
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Get the AudioSource ONCE and keep it
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = endingMusicClip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0f; // Start silent

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        CheckAndStartMusicIfInEndingScene();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == EndingSceneName)
        {
            CheckAndStartMusicIfInEndingScene();
        }
        else if (hasStartedPlaying)
        {
            // Left the ending scene → fade out and die
            StopAllCoroutines();
            StartCoroutine(FadeOutAndDestroy());
        }
    }

    private void CheckAndStartMusicIfInEndingScene()
    {
        if (!hasStartedPlaying && SceneManager.GetActiveScene().name == EndingSceneName)
        {
            StartCoroutine(FadeInMusic());
            hasStartedPlaying = true;
        }
    }

    private IEnumerator FadeInMusic()
    {
        if (endingMusicClip == null)
        {
            Debug.LogWarning("EndingMusicController: No ending music clip assigned!");
            yield break;
        }

        // --- CRITICAL: Always get the LATEST volume from manager ---
        float targetVolume = AudioVolumeManager.Instance != null
            ? AudioVolumeManager.Instance.GetMusicVolume()
            : 1f;

        audioSource.volume = 0f;
        audioSource.Play();

        float timer = 0f;
        while (timer < fadeInDuration)
        {
            timer += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(0f, targetVolume, timer / fadeInDuration);

            // Live update if player changes volume during fade-in
            targetVolume = AudioVolumeManager.Instance != null
                ? AudioVolumeManager.Instance.GetMusicVolume()
                : 1f;

            yield return null;
        }

        // Final snap to correct volume
        audioSource.volume = targetVolume;
    }

    private IEnumerator FadeOutAndDestroy()
    {
        float startVolume = audioSource.volume;
        float timer = 0f;

        while (timer < fadeOutDuration)
        {
            timer += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeOutDuration);
            yield return null;
        }

        audioSource.Stop();

        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (Instance == this) Instance = null;
        Destroy(gameObject);
    }

    // Called by AudioVolumeManager when slider changes
    public void UpdateVolume()
    {
        if (audioSource == null || AudioVolumeManager.Instance == null) return;

        float newVolume = AudioVolumeManager.Instance.GetMusicVolume();

        // Instantly apply new volume (even during fade)
        // This overrides whatever the fade coroutine is doing
        audioSource.volume = newVolume;
    }

    // Backup: Ensure volume stays correct every frame
    private void Update()
    {
        if (hasStartedPlaying && audioSource.isPlaying && AudioVolumeManager.Instance != null)
        {
            float currentSavedVolume = AudioVolumeManager.Instance.GetMusicVolume();
            if (!Mathf.Approximately(audioSource.volume, currentSavedVolume))
            {
                // Only override if not in middle of fade-out (fade-out goes to 0)
                if (audioSource.volume > 0.01f || currentSavedVolume > 0f)
                {
                    audioSource.volume = currentSavedVolume;
                }
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}