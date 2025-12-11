using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;
    public Image fadeImage;
    public float fadeDuration = 1f;

    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Make sure fadeImage starts black (in case game loads mid-fade)
        fadeImage.color = new Color(0, 0, 0, 1f);

        // When a scene loads, fade in automatically
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Start fade in every time a scene finishes loading
        StartCoroutine(Fade(1, 0));
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOutIn(sceneName));
    }

    private IEnumerator FadeOutIn(string sceneName)
    {
        // Fade to black
        yield return StartCoroutine(Fade(0, 1));

        // Load scene
        SceneManager.LoadScene(sceneName);
    }

    public IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float normalized = t / fadeDuration;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, normalized);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }
}
