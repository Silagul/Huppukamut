using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MenuSoundController : MonoBehaviour
{
    public static MenuSoundController Instance;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private AudioSource audioSource;

    private readonly string[] menuScenes = { "Main Menu", "Character Select" };

    private bool isMusicAlreadyPlaying = false;
    private float fadeMultiplier = 1f;

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

        audioSource.loop = false;
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
        fadeMultiplier = 0f;
        audioSource.Play();

        while (fadeMultiplier < 1f)
        {
            fadeMultiplier += Time.unscaledDeltaTime / fadeDuration;
            fadeMultiplier = Mathf.Clamp01(fadeMultiplier);
            ApplyVolume();
            yield return null;
        }
    }

    private IEnumerator FadeOutAndDestroy()
    {
        while (fadeMultiplier > 0f)
        {
            fadeMultiplier -= Time.unscaledDeltaTime / fadeDuration;
            fadeMultiplier = Mathf.Clamp01(fadeMultiplier);
            ApplyVolume();
            yield return null;
        }

        audioSource.Stop();

        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (Instance == this) Instance = null;

        Destroy(gameObject);
    }

    private void ApplyVolume()
    {
        audioSource.volume =
            fadeMultiplier * AudioVolumeManager.Instance.GetMusicVolume();
    }

    // KUTSUTAAN SLIDERISTA
    public void UpdateVolume()
    {
        ApplyVolume();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
