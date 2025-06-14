using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ћенеджер гекс-сетки, отслеживает расположение шаров на игровом поле.
/// </summary>
public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    private Dictionary<Vector2Int, BallController> grid = new();

    [SerializeField] private float radius = 0.5f;
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private int gridHeight = 10;

    private float hexWidth;
    private float hexHeight;

    public int GridWidth => gridWidth;
    public int GridHeight => gridHeight;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            return;

        Instance = this;
        DontDestroyOnLoad(gameObject);

        hexWidth = radius * 2f * 0.866f; // 0.866 = cos(30∞)
        hexHeight = radius * 2f;
    }

    /// <summary>
    /// ѕреобразует мировую позицию в координаты гекс-сетки.
    /// </summary>
    public Vector2Int GetNearestHexCoord(Vector3 worldPos)
    {
        float x = worldPos.x / hexWidth;
        float z = -worldPos.z / (hexHeight * 0.75f);

        int gridZ = Mathf.RoundToInt(z);
        int gridX = Mathf.RoundToInt(x - (gridZ % 2 == 0 ? 0 : 0.5f));

        return new Vector2Int(gridX, gridZ);
    }

    /// <summary>
    /// ѕреобразует координаты гекс-сетки в мировую позицию.
    /// </summary>
    public Vector3 GetWorldPosition(Vector2Int gridPos)
    {
        float x = gridPos.x * hexWidth;
        if (Mathf.Abs(gridPos.y) % 2 == 1)
            x += hexWidth / 2f;

        float z = -gridPos.y * (hexHeight * 0.75f);
        return new Vector3(x, 0f, z);
    }

    /// <summary>
    /// –егистрирует шар на указанных координатах.
    /// </summary>
    public void RegisterBall(BallController ball, Vector2Int coords)
    {
        ball.SetGridPosition(coords);
        grid[coords] = ball;
    }

    /// <summary>
    /// ”дал€ет шар с указанных координат.
    /// </summary>
    public void UnregisterBall(Vector2Int coords)
    {
        grid.Remove(coords);
    }

    public bool HasBallAt(Vector2Int coords) => grid.ContainsKey(coords);

    public BallController GetBallAt(Vector2Int coords)
    {
        grid.TryGetValue(coords, out var ball);
        return ball;
    }

    public Dictionary<Vector2Int, BallController> GetAllBalls() => grid;

    /// <summary>
    /// ¬озвращает соседние координаты с шарами в 6 направлени€х.
    /// </summary>
    public List<Vector2Int> GetNeighbors(Vector2Int center)
    {
        List<Vector2Int> neighbors = new();
        bool isOddRow = Mathf.Abs(center.y) % 2 == 1;

        Vector2Int[] evenOffsets = {
            new(-1,  0), new(-1, -1), new(0, -1),
            new(1,  0), new(0,  1), new(-1, 1)
        };

        Vector2Int[] oddOffsets = {
            new(-1,  0), new(0, -1), new(1, -1),
            new(1,  0), new(1,  1), new(0,  1)
        };

        var offsets = isOddRow ? oddOffsets : evenOffsets;

        foreach (var offset in offsets)
        {
            var neighbor = center + offset;
            if (grid.ContainsKey(neighbor))
                neighbors.Add(neighbor);
        }

        return neighbors;
    }

    /// <summary>
    /// ¬озвращает все координаты, где наход€тс€ шары.
    /// </summary>
    public List<Vector2Int> GetAllBallCoords()
    {
        return new List<Vector2Int>(grid.Keys);
    }
}
