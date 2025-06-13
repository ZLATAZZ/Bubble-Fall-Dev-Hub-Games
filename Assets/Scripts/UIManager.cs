using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Score UI")]
    public TextMeshProUGUI bestScoreText;

    [Header("Settings UI")]
    public Slider volumeSlider;
    public TextMeshProUGUI volumeValueText;

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

    public void OnVolumeSliderChanged(float value)
    {
        volumeValueText.text = $"{Mathf.RoundToInt(value * 100)}%";
        AudioListener.volume = value;
        SaveManager.Instance.CurrentData.volume = value;
        SaveManager.Instance.Save();
    }

    public void LoadUIFromSave()
    {
        SaveManager.Instance.Load();
        float savedVolume = SaveManager.Instance.CurrentData.volume;
        volumeSlider.value = savedVolume;
        volumeValueText.text = $"{Mathf.RoundToInt(savedVolume * 100)}%";
        AudioListener.volume = savedVolume;

        UpdateBestScoreUI(SaveManager.Instance.CurrentData.bestScore);
    }
}
