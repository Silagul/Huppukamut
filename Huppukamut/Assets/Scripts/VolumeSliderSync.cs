using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderSync : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;  // Drag your slider here

    private void Start()
    {
        if (AudioVolumeManager.Instance != null)
        {
            // Set slider to saved value (without triggering OnValueChanged yet)
            musicSlider.SetValueWithoutNotify(AudioVolumeManager.Instance.GetMusicVolume());
        }
        else
        {
            musicSlider.value = 1f; // fallback
        }

        // Now set up the listener (only once)
        musicSlider.onValueChanged.RemoveAllListeners();
        musicSlider.onValueChanged.AddListener(val => AudioVolumeManager.Instance.SetMusicVolume(val));
    }
}