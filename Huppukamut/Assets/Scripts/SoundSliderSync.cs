using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public enum VolumeType { Music, SFX }

    [SerializeField] private VolumeType volumeType;
    [SerializeField] private Slider slider;

    private void Reset()
    {
        slider = GetComponent<Slider>();
    }

    private void Start()
    {
        if (slider == null)
        {
            Debug.LogError("Slider component missing on VolumeSlider!");
            return;
        }

        if (AudioVolumeManager.Instance == null)
        {
            Debug.LogWarning("AudioVolumeManager not found yet — will try again next frame.");
            Invoke(nameof(InitializeSlider), 0.1f);
            return;
        }

        InitializeSlider();
    }

    private void InitializeSlider()
    {
        float current = volumeType == VolumeType.Music
            ? AudioVolumeManager.Instance.GetMusicVolume()
            : AudioVolumeManager.Instance.GetSFXVolume();

        slider.SetValueWithoutNotify(current);

        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(float value)
    {
        if (volumeType == VolumeType.Music)
            AudioVolumeManager.Instance.SetMusicVolume(value);
        else
            AudioVolumeManager.Instance.SetSFXVolume(value);
    }
}