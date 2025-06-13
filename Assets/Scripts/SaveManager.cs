using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int bestScore = 0;
    public float volume = 1.0f;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public SaveData CurrentData { get; private set; }

    private string savePath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        savePath = Path.Combine(Application.persistentDataPath, "save.json");
        Load();
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(CurrentData, true);
        File.WriteAllText(savePath, json);
    }

    public void Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            CurrentData = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            CurrentData = new SaveData();
            Save(); // создать первый раз
        }
    }

    public void TryUpdateBestScore(int score)
    {
        if (score > CurrentData.bestScore)
        {
            CurrentData.bestScore = score;
            Save();
            UIManager.Instance.UpdateBestScoreUI(score);
        }
    }
}
