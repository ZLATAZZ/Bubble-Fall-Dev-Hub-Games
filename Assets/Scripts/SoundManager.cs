using UnityEngine;

/// <summary>
/// �������� ������, ����������� ������� Singleton.
/// �������� �� ��������������� ������ � ������� ������.
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioSource _mainSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip _reflectSound;
    [SerializeField] private AudioClip _laserFireSound;
    [SerializeField] private AudioClip _hitSound;
    [SerializeField] private AudioClip _gameOver;
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
    /// ������������� �������� ���������.
    /// </summary>
    public void PlaySound(AudioClip clip)
    {
        if (clip != null && _sfxSource != null)
        {
            _sfxSource.PlayOneShot(clip);
        }
    }

    /// <summary>
    /// ������������� ���� ��������.
    /// </summary>
    public void PlayReflectSound() => PlaySound(_reflectSound);

    /// <summary>
    /// ������������� ���� �������� �������.
    /// </summary>
    public void PlayLaserFire() => PlaySound(_laserFireSound);

    /// <summary>
    /// ������������� ���� ���������.
    /// </summary>
    public void PlayBallHit() => PlaySound(_hitSound);

    /// <summary>
    /// ������������� ���� ��������� ����.
    /// </summary>
    public void PlayOnGameOver() => PlaySound(_gameOver);

    /// <summary>
    /// ������������� ������� ������ � �����.
    /// </summary>
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
