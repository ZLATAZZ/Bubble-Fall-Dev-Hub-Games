using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

/// <summary>
/// Управляет пользовательским интерфейсом и настройками звука.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Score UI")]
    [SerializeField] private TextMeshProUGUI bestScoreText;

    [Header("Settings UI")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI sfxVolumeText;

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadUIFromSave();
    }

    /// <summary>
    /// Загружает указанную сцену.
    /// </summary>
    public void LoadScene(int sceneIndex)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneIndex);
    }

    /// <summary>
    /// Приостанавливает игру.
    /// </summary>
    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Возобновляет игру.
    /// </summary>
    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Завершает приложение.
    /// </summary>
    public void QuitApp()
    {
        Application.Quit();
    }

    /// <summary>
    /// Обновляет текст текущего счёта.
    /// </summary>
    public void UpdateScoreUI(int score, TextMeshProUGUI scoreText)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }

    /// <summary>
    /// Обновляет текст лучшего счёта.
    /// </summary>
    public void UpdateBestScoreUI(int bestScore)
    {
        if (bestScoreText != null)
            bestScoreText.text = $"Best: {bestScore}";
    }

    /// <summary>
    /// Вызывается при изменении ползунка музыки.
    /// </summary>
    public void OnMusicSliderChanged(float value)
    {
        musicVolumeText.text = $"{Mathf.RoundToInt(value * 100)}%";
        SetMusicVolume(ToDecibel(value));

        SaveManager.Instance.CurrentData.musicVolume = value;
        SaveManager.Instance.Save();
    }

    /// <summary>
    /// Вызывается при изменении ползунка эффектов.
    /// </summary>
    public void OnSFXSliderChanged(float value)
    {
        sfxVolumeText.text = $"{Mathf.RoundToInt(value * 100)}%";
        SetSFXVolume(ToDecibel(value));

        SaveManager.Instance.CurrentData.sfxVolume = value;
        SaveManager.Instance.Save();
    }

    /// <summary>
    /// Загружает данные из сохранения и применяет к UI.
    /// </summary>
    public void LoadUIFromSave()
    {
        SaveManager.Instance.Load();

        float music = SaveManager.Instance.CurrentData.musicVolume;
        float sfx = SaveManager.Instance.CurrentData.sfxVolume;

        musicSlider.value = music;
        sfxSlider.value = sfx;

        musicVolumeText.text = $"{Mathf.RoundToInt(music * 100)}%";
        sfxVolumeText.text = $"{Mathf.RoundToInt(sfx * 100)}%";

        SetMusicVolume(ToDecibel(music));
        SetSFXVolume(ToDecibel(sfx));

        UpdateBestScoreUI(SaveManager.Instance.CurrentData.bestScore);
    }

    /// <summary>
    /// Устанавливает громкость музыки в микшере.
    /// </summary>
    private void SetMusicVolume(float db)
    {
        audioMixer.SetFloat("MusicVolume", db);
    }

    /// <summary>
    /// Устанавливает громкость звуковых эффектов в микшере.
    /// </summary>
    private void SetSFXVolume(float db)
    {
        audioMixer.SetFloat("SFXVolume", db);
    }

    /// <summary>
    /// Перевод значения (0-1) в децибелы.
    /// </summary>
    private float ToDecibel(float value)
    {
        return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
    }
}
