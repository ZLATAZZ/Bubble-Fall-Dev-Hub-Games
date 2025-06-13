using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [HideInInspector] public static ScoreManager Instance { get; private set; }

    private int currentScore = 0;
    public int CurrentScore => currentScore;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        currentScore = 0;
    }

    public void AddPoints(int points)
    {
        currentScore += points;
        SaveManager.Instance.TryUpdateBestScore(currentScore);
    }

    public void AddPopPoints(int poppedCount)
    {
        AddPoints(poppedCount * 10);
    }

    
}
