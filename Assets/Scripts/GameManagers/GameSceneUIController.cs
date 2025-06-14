using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Контроллер UI сцены игры: обновление счёта и обработка кнопок паузы/возобновления/выхода.
/// </summary>
public class GameSceneUIController : MonoBehaviour
{
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private Button returnButtonAfterGameOver;
    [SerializeField] private Button replayButton;
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private Image gameOverPanel;

    public static GameSceneUIController Instance { get; private set; }

    private void Awake()
    {
       
        Instance = this;
    }

    private void Start()
    {
        SetupButtonListeners();
    }

    private void Update()
    {
        UpdateCurrentScore();
    }

    private void UpdateCurrentScore()
    {
        if (ScoreManager.Instance != null)
            currentScoreText.text = $"Score: {ScoreManager.Instance.CurrentScore}";
    }

    private void SetupButtonListeners()
    {
        if (UIManager.Instance == null)
        {
            Debug.LogWarning("UIManager.Instance is null — кнопки не будут работать");
            return;
        }

        pauseButton.onClick.AddListener(UIManager.Instance.PauseGame);
        resumeButton.onClick.AddListener(UIManager.Instance.ResumeGame);
        returnButton.onClick.AddListener(() => UIManager.Instance.LoadScene(0));
        returnButtonAfterGameOver.onClick.AddListener(() => UIManager.Instance.LoadScene(0));
        replayButton.onClick.AddListener(() => UIManager.Instance.LoadScene(1));
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        gameOverPanel.gameObject.SetActive(true);
        SoundManager.Instance.PlayOnGameOver();
    }
}
