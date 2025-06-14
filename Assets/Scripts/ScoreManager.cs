using UnityEngine;

/// <summary>
/// Отвечает за счёт игрока во время игры.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private int currentScore = 0;
    public int CurrentScore => currentScore;

    private void Awake()
    {
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
        currentScore = 0;
    }

    /// <summary>
    /// Добавляет указанное количество очков.
    /// </summary>
    public void AddPoints(int points)
    {
        currentScore += points;

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.TryUpdateBestScore(currentScore);
        }
        else
        {
            Debug.LogWarning("SaveManager.Instance is null — невозможно обновить лучший счёт.");
        }
    }

    /// <summary>
    /// Добавляет очки за взорванные шары.
    /// </summary>
    public void AddPopPoints(int poppedCount)
    {
        AddPoints(poppedCount * 10);
    }
}
