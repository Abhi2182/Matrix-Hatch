using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField]private AudioSource _sfxAudioSource;

    [Header("SFX Clips")]
    public AudioClip flipClip;
    public AudioClip matchClip;
    public AudioClip mismatchClip;
    public AudioClip gameOverClip;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this.gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        if (_sfxAudioSource != null)
        {
            _sfxAudioSource.playOnAwake = false;
            _sfxAudioSource.loop = false;
        }
    }

    #region SFX

    public void PlayFlip() => PlayOneShot(flipClip);
    public void PlayMatch() => PlayOneShot(matchClip);
    public void PlayMismatch() => PlayOneShot(mismatchClip);
    public void PlayGameOver() => PlayOneShot(gameOverClip);

    private void PlayOneShot(AudioClip clip)
    {
        if (clip == null) return;
        _sfxAudioSource.PlayOneShot(clip);
    }

    #endregion

}
