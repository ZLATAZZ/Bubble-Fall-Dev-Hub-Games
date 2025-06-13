using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private int currentScore = 0;
    public int CurrentScore => currentScore;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        ResetScore();
    }

    public void ResetScore()
    {
        currentScore = 0;
        UIManager.Instance.UpdateScoreUI(currentScore);
    }

    public void AddPoints(int points)
    {
        currentScore += points;
        UIManager.Instance.UpdateScoreUI(currentScore);
        SaveManager.Instance.TryUpdateBestScore(currentScore);
    }

    public void AddPopPoints(int poppedCount)
    {
        AddPoints(poppedCount * 10);
    }

    public void AddFallPoints(int droppedCount)
    {
        AddPoints(droppedCount * 10);
    }
}
