using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EndingMusicPlayer : MonoBehaviour
{
    [Header("Music Clip")]
    [SerializeField] private AudioClip endingMusicClip;

    [Header("Optional Fade In")]
    [SerializeField] private bool fadeInOnStart = true;
    [SerializeField] private float fadeDuration = 2.5f;

    private AudioSource audioSource;
    private float fadeAlpha = 0f; // 0 = silent, 1 = full volume

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = endingMusicClip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0f;
    }

    private void Start()
    {
        if (endingMusicClip == null)
        {
            Debug.LogWarning("EndingMusicPlayer: No clip assigned!");
            return;
        }

        audioSource.Play();

        if (fadeInOnStart)
        {
            fadeAlpha = 0f;
            StartCoroutine(FadeIn());
        }
        else
        {
            fadeAlpha = 1f;
        }
    }

    private void Update()
    {
        // Instantly follows the music volume slider
        float masterVolume = AudioVolumeManager.Instance != null 
            ? AudioVolumeManager.Instance.GetMusicVolume() 
            : 1f;

        audioSource.volume = masterVolume * fadeAlpha;
    }

    private System.Collections.IEnumerator FadeIn()
    {
        while (fadeAlpha < 1f)
        {
            fadeAlpha += Time.unscaledDeltaTime / fadeDuration;
            if (fadeAlpha > 1f) fadeAlpha = 1f;
            yield return null;
        }
    }
}