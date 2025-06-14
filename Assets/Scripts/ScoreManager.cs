using UnityEngine;

/// <summary>
/// �������� �� ���� ������ �� ����� ����.
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
    /// ��������� ��������� ���������� �����.
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
            Debug.LogWarning("SaveManager.Instance is null � ���������� �������� ������ ����.");
        }
    }

    /// <summary>
    /// ��������� ���� �� ���������� ����.
    /// </summary>
    public void AddPopPoints(int poppedCount)
    {
        AddPoints(poppedCount * 10);
    }
}
