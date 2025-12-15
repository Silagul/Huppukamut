using UnityEngine;
using UnityEngine.UI;

public class SoundSliderSync : MonoBehaviour
{
    [SerializeField] private Slider slider;

    private void Start()
    {
        Debug.Log("SoundSliderSync Start");

        if (SoundVolumeManager.Instance == null)
        {
            Debug.LogError("❌ SoundVolumeManager.Instance = NULL");
            return;
        }

        float saved = SoundVolumeManager.Instance.GetVolume();
        Debug.Log("Loaded SFX volume: " + saved);

        slider.SetValueWithoutNotify(saved);

        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    private void OnSliderChanged(float value)
    {
        Debug.Log("🎚 Slider changed to: " + value);
        SoundVolumeManager.Instance.SetVolume(value);
    }
}
