using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneUIController : MonoBehaviour
{
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _returnButton;
    [SerializeField] private TextMeshProUGUI _currentScore;

    private void Start()
    {
        CallButtonsMethods();
    }

    private void Update()
    {
        UpdateCurrentScore();
    }

    private void UpdateCurrentScore()
    {
        _currentScore.text = ScoreManager.Instance.CurrentScore.ToString();
    }

    private void CallButtonsMethods()
    {
        if (UIManager.Instance != null)
        {
            _pauseButton.onClick.AddListener(UIManager.Instance.PauseGame);
            _resumeButton.onClick.AddListener(UIManager.Instance.ResumeGame);
            _returnButton.onClick.AddListener(() => UIManager.Instance.LoadScene(0));
        }
        else
        {
            Debug.LogWarning("UIManager.Instance is null Ч кнопка не будет работать");
        }
    }
}
