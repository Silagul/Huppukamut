using UnityEngine;

public class SoundVolumeManager : MonoBehaviour
{
    public static SoundVolumeManager Instance;

    private float volume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            volume = PlayerPrefs.GetFloat("SoundVolume", 1f);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetVolume(float value)
    {
        volume = value;
        Debug.Log("🔊 SoundVolumeManager volume = " + volume);

        PlayerPrefs.SetFloat("SoundVolume", volume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        Debug.Log("📤 GetVolume = " + volume);
        return volume;
    }
}
