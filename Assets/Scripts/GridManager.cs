using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    private Dictionary<Vector2Int, BallController> grid = new Dictionary<Vector2Int, BallController>();

    [SerializeField] private float radius = 0.5f;
    private float hexWidth;
    private float hexHeight;
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private int gridHeight = 10;

    public int GridWidth => gridWidth;
    public int GridHeight => gridHeight;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // ¬ычисление размеров гексагона
        hexWidth = radius * 2f * 0.866f; // cos(30∞)
        hexHeight = radius * 2f;
    }

    // ѕреобразует мировую позицию в координаты гекс-сетки
    public Vector2Int GetNearestHexCoord(Vector3 worldPos)
    {
        float x = worldPos.x / hexWidth;
        float z = -worldPos.z / (hexHeight * 0.75f); // учЄт вертикального смещени€

        int gridZ = Mathf.RoundToInt(z);
        int gridX = Mathf.RoundToInt(x - (gridZ % 2 == 0 ? 0 : 0.5f));

        return new Vector2Int(gridX, gridZ);
    }

    // ѕреобразует координаты гекс-сетки в мировую позицию
    public Vector3 GetWorldPosition(Vector2Int gridPos)
    {
        float x = gridPos.x * hexWidth;
        if (Mathf.Abs(gridPos.y) % 2 == 1)
            x += hexWidth / 2f;

        float z = -gridPos.y * (hexHeight * 0.75f);

        return new Vector3(x, 0f, z);
    }

    public void RegisterBall(BallController ball, Vector2Int coords)
    {
        ball.SetGridPosition(coords);
        if (!grid.ContainsKey(coords))
        {
            grid.Add(coords, ball);
        }
        else
        {
            grid[coords] = ball; // перезаписываем если уже есть
        }
    }

    public void UnregisterBall(Vector2Int coords)
    {
        if (grid.ContainsKey(coords))
        {
            grid.Remove(coords);
        }
    }

    public bool HasBallAt(Vector2Int coords)
    {
        return grid.ContainsKey(coords);
    }

    public BallController GetBallAt(Vector2Int coords)
    {
        grid.TryGetValue(coords, out BallController ball);
        return ball;
    }

    public Dictionary<Vector2Int, BallController> GetAllBalls()
    {
        return grid;
    }

    // ¬озвращает соседей по 6 направлени€м гекс-сетки
    public List<Vector2Int> GetNeighbors(Vector2Int center)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        // четный или нечетный р€д вли€ет на смещение
        bool isOddRow = Mathf.Abs(center.y) % 2 == 1;

        Vector2Int[] evenOffsets = new Vector2Int[]
        {
            new Vector2Int(-1,  0),
            new Vector2Int(-1, -1),
            new Vector2Int( 0, -1),
            new Vector2Int( 1,  0),
            new Vector2Int( 0,  1),
            new Vector2Int(-1,  1)
        };

        Vector2Int[] oddOffsets = new Vector2Int[]
        {
            new Vector2Int(-1,  0),
            new Vector2Int( 0, -1),
            new Vector2Int( 1, -1),
            new Vector2Int( 1,  0),
            new Vector2Int( 1,  1),
            new Vector2Int( 0,  1)
        };

        Vector2Int[] offsets = isOddRow ? oddOffsets : evenOffsets;

        foreach (var offset in offsets)
        {
            Vector2Int neighbor = center + offset;
            if (grid.ContainsKey(neighbor))
                neighbors.Add(neighbor);
        }

        return neighbors;
    }

    public List<Vector2Int> GetAllBallCoords()
    {
        List<Vector2Int> coords = new List<Vector2Int>();
        foreach (var kvp in grid)
        {
            coords.Add(kvp.Key);
        }
        return coords;
    }

}
