using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MenuMusicController : MonoBehaviour
{
    public static MenuMusicController Instance;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private AudioSource audioSource;

    private readonly string[] menuScenes = { "Main Menu", "Character Select" };

    private bool isMusicAlreadyPlaying = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        audioSource.loop = true;
        audioSource.playOnAwake = false;

        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(InitialCheck());
    }

    private IEnumerator InitialCheck()
    {
        yield return null; // Wait one frame so active scene is valid
        EvaluateCurrentScene();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EvaluateCurrentScene();
    }

    private void EvaluateCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        bool isMenuScene = System.Array.Exists(menuScenes, s => s == currentScene);

        if (isMenuScene)
        {
            if (!isMusicAlreadyPlaying)
            {
                StopAllCoroutines();
                StartCoroutine(FadeIn());
                isMusicAlreadyPlaying = true;
            }
            // Already playing → seamless transition (no restart!)
        }
        else
        {
            if (isMusicAlreadyPlaying)
            {
                StopAllCoroutines();
                StartCoroutine(FadeOutAndDestroy());
                isMusicAlreadyPlaying = false;
            }
        }
    }

    private IEnumerator FadeIn()
    {
        audioSource.volume = 0f;
        audioSource.Play();

        float targetVolume = MusicVolumeManager.Instance.GetVolume();

        while (audioSource.volume < targetVolume)
        {
            audioSource.volume += Time.unscaledDeltaTime / fadeDuration;
            if (audioSource.volume > targetVolume)
                audioSource.volume = targetVolume;
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    private IEnumerator FadeOutAndDestroy()
    {
        float currentVolume = audioSource.volume;

        while (audioSource.volume > 0f)
        {
            audioSource.volume -= currentVolume * Time.unscaledDeltaTime / fadeDuration;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = MusicVolumeManager.Instance.GetVolume(); // reset

        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (Instance == this) Instance = null;

        Destroy(gameObject);
    }

    // Called whenever the slider changes
    public void UpdateVolume()
    {
        if (audioSource != null)
            audioSource.volume = MusicVolumeManager.Instance.GetVolume();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}