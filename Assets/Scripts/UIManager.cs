using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Score UI")]
    public Text currentScoreText;
    public Text bestScoreText;

    [Header("Settings UI")]
    public Slider volumeSlider;
    public Text volumeValueText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        LoadUIFromSave();
    }

    public void UpdateScoreUI(int score)
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
