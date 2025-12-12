using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Gameplay SFX")]
    public AudioClip collect;
    public AudioClip stomp;
    public AudioClip dash;
    public AudioClip glideStart;
    public AudioClip glideLoop;
    public AudioClip boxBreak;

    [Header("Player Movement")]
    public AudioClip jumpSound;
    public AudioClip landSound;

    [Header("Helping Friend - Random Cheers")]
    public AudioClip[] helpFriendVariations;

    [Header("Settings")]
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    private AudioSource oneShotSource;
    private AudioSource loopSource;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        oneShotSource = gameObject.AddComponent<AudioSource>();

        loopSource = gameObject.AddComponent<AudioSource>();
        loopSource.loop = true;
        loopSource.playOnAwake = false;
    }

    private void PlaySFX(AudioClip clip, float volumeScale = 1f)
    {
        if (clip != null)
            oneShotSource.PlayOneShot(clip, volumeScale * sfxVolume);
    }

    // Gameplay
    public void PlayCollect()      => PlaySFX(collect);
    public void PlayStomp()        => PlaySFX(stomp);
    public void Dash()             => PlaySFX(dash);
    public void GlideStart()       => PlaySFX(glideStart);
    public void BoxBreak()         => PlaySFX(boxBreak);

    // Movement – THESE FIX YOUR ERROR
    public void PlayJump()         => PlaySFX(jumpSound);
    public void PlayLand()         => PlaySFX(landSound);

    // Glide Loop
    public void GlideLoop(bool play)
    {
        if (glideLoop == null) return;

        if (play && !loopSource.isPlaying)
        {
            loopSource.clip = glideLoop;
            loopSource.volume = sfxVolume;
            loopSource.Play();
        }
        else if (!play && loopSource.isPlaying)
        {
            loopSource.Stop();
        }
    }

    // Friend Helped
    public void PlayHelpFriend()
    {
        if (helpFriendVariations == null || helpFriendVariations.Length == 0) return;

        int index = Random.Range(0, helpFriendVariations.Length);
        PlaySFX(helpFriendVariations[index]);
    }
}