using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Score UI")]
    public TextMeshProUGUI bestScoreText;

    [Header("Settings UI")]
    public Slider musicSlider;
    public Slider sfxSlider;

    public TextMeshProUGUI musicVolumeText;
    public TextMeshProUGUI sfxVolumeText;

    [Header("Audio")]
    public AudioMixer audioMixer;

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

    public void LoadScene(int sceneIndex)
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(sceneIndex);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    public void QuitApp()
    {
        Application.Quit();
    }

    public void UpdateScoreUI(int score, TextMeshProUGUI currentScoreText)
    {
        currentScoreText.text = $"Score: {score}";
    }

    public void UpdateBestScoreUI(int best)
    {
        bestScoreText.text = $"Best: {best}";
    }

    public void OnMusicSliderChanged(float value)
    {
        musicVolumeText.text = $"{Mathf.RoundToInt(value * 100)}%";
        float db = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        SetMusicVolume(db);

        SaveManager.Instance.CurrentData.musicVolume = value;
        SaveManager.Instance.Save();
    }

    public void OnSFXSliderChanged(float value)
    {
        sfxVolumeText.text = $"{Mathf.RoundToInt(value * 100)}%";
        float db = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        SetSFXVolume(db);

        SaveManager.Instance.CurrentData.sfxVolume = value;
        SaveManager.Instance.Save();
    }

    private void SetMusicVolume(float db)
    {
        audioMixer.SetFloat("MusicVolume", db);
    }

    private void SetSFXVolume(float db)
    {
        audioMixer.SetFloat("SFXVolume", db);
    }

    public void LoadUIFromSave()
    {
        SaveManager.Instance.Load();

        float music = SaveManager.Instance.CurrentData.musicVolume;
        float sfx = SaveManager.Instance.CurrentData.sfxVolume;

        musicSlider.value = music;
        sfxSlider.value = sfx;

        musicVolumeText.text = $"{Mathf.RoundToInt(music * 100)}%";
        sfxVolumeText.text = $"{Mathf.RoundToInt(sfx * 100)}%";

        SetMusicVolume(Mathf.Log10(Mathf.Clamp(music, 0.0001f, 1f)) * 20f);
        SetSFXVolume(Mathf.Log10(Mathf.Clamp(sfx, 0.0001f, 1f)) * 20f);

        UpdateBestScoreUI(SaveManager.Instance.CurrentData.bestScore);
    }
}
