using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LevelMusicPlayer : MonoBehaviour
{
    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.loop = true;
        source.playOnAwake = false;
    }

    private void Start()
    {
        // Automatically respects the player’s music volume setting
        source.volume = MusicVolumeManager.Instance?.GetVolume() ?? 1f;
        source.Play();
    }

    // Live update when player moves the slider during gameplay
    private void Update()
    {
        if (source.isPlaying)
            source.volume = MusicVolumeManager.Instance?.GetVolume() ?? 1f;
    }
}