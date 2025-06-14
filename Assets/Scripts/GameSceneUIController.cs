using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���������� UI ����� ����: ���������� ����� � ��������� ������ �����/�������������/������.
/// </summary>
public class GameSceneUIController : MonoBehaviour
{
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private TextMeshProUGUI currentScoreText;

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
            Debug.LogWarning("UIManager.Instance is null � ������ �� ����� ��������");
            return;
        }

        pauseButton.onClick.AddListener(UIManager.Instance.PauseGame);
        resumeButton.onClick.AddListener(UIManager.Instance.ResumeGame);
        returnButton.onClick.AddListener(() => UIManager.Instance.LoadScene(0));
    }
}
