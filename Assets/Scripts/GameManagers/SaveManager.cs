using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int bestScore = 0;
    public float musicVolume = 1.0f;
    public float sfxVolume = 1.0f;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public SaveData CurrentData { get; private set; } = new SaveData();

    private string savePath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        savePath = Path.Combine(Application.persistentDataPath, "save.json");
        Load();
    }

    /// <summary>
    /// Сохраняет текущие данные в файл JSON
    /// </summary>
    public void Save()
    {
        if (CurrentData == null)
        {
            Debug.LogError("Попытка сохранить пустые данные.");
            return;
        }

        try
        {
            string json = JsonUtility.ToJson(CurrentData, true);
            File.WriteAllText(savePath, json);
        }
        catch (IOException e)
        {
            Debug.LogError($"Ошибка при сохранении данных: {e.Message}");
        }
    }

    /// <summary>
    /// Загружает данные из файла, либо создает новый файл при первом запуске
    /// </summary>
    public void Load()
    {
        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                CurrentData = JsonUtility.FromJson<SaveData>(json) ?? new SaveData();
            }
            catch (IOException e)
            {
                Debug.LogError($"Ошибка при загрузке данных: {e.Message}");
                CurrentData = new SaveData();
            }
        }
        else
        {
            CurrentData = new SaveData();
            Save(); // Создание файла при первом запуске
        }
    }

    /// <summary>
    /// Обновляет лучший счёт, если новый больше текущего
    /// </summary>
    public void TryUpdateBestScore(int score)
    {
        if (score > CurrentData.bestScore)
        {
            CurrentData.bestScore = score;
            Save();

            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateBestScoreUI(score);
            }
            else
            {
                Debug.LogWarning("UIManager.Instance не найден — невозможно обновить UI.");
            }
        }
    }
}
