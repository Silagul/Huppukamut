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
        yield return null;
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

        float targetVolume = AudioVolumeManager.Instance != null
            ? AudioVolumeManager.Instance.GetMusicVolume()
            : 1f;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(0f, targetVolume, timer / fadeDuration);
            yield return null;
        }
        audioSource.volume = targetVolume;
    }

    private IEnumerator FadeOutAndDestroy()
    {
        float startVolume = audioSource.volume;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeDuration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = AudioVolumeManager.Instance != null
            ? AudioVolumeManager.Instance.GetMusicVolume()
            : 1f;

        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (Instance == this) Instance = null;
        Destroy(gameObject);
    }

    public void UpdateVolume()
    {
        if (audioSource != null && AudioVolumeManager.Instance != null)
        {
            audioSource.volume = AudioVolumeManager.Instance.GetMusicVolume();
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}