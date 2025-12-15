using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    [SerializeField] private AudioClip customClickSound; // Optional: override the default click

    [Header("Settings")]
    [SerializeField] private float volumeScale = 0.8f; // How loud compared to full SFX volume

    private void Awake()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(PlayClickSound);
    }

    private void PlayClickSound()
    {
        if (SoundManager.instance == null) return;

        // Use custom sound if assigned, otherwise use the default UI click from SoundManager
        if (customClickSound != null)
        {
            SoundManager.instance.PlayOneShot(customClickSound, volumeScale);
        }
        else
        {
            SoundManager.instance.PlayUIClick();
        }
    }
}