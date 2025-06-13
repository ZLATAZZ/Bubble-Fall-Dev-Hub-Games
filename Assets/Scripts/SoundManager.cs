using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource _sfxSource;
    public AudioSource _mainSource;

    [Header("Audio Clips")]
    public AudioClip _reflectSound;
    public AudioClip _laserFireSound;
    [SerializeField] private AudioClip _backgroundMusic;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        PlayBackgroundMusic();
    }

    /// <summary>
    /// Воспроизводит произвольный звук
    /// </summary>
    public void PlaySound(AudioClip clip)
    {
        if (clip != null && _sfxSource != null)
        {
            _sfxSource.PlayOneShot(clip);
        }
    }

    /// <summary>
    /// Воспроизводит звук рикошета
    /// </summary>
    public void PlayReflectSound()
    {
        PlaySound(_reflectSound);
    }

    /// <summary>
    /// Воспроизводит звук выстрела лазером
    /// </summary>
    public void PlayLaserFire()
    {
        PlaySound(_laserFireSound);
    }

    private void PlayBackgroundMusic()
    {
        if (_mainSource != null && _backgroundMusic != null)
        {
            _mainSource.clip = _backgroundMusic;
            _mainSource.loop = true; 
            _mainSource.Play();
        }
    }

}
