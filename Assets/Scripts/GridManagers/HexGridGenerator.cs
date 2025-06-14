using System.Collections.Generic;
using UnityEngine;

public class HexGridGenerator3D : MonoBehaviour
{
    [System.Serializable]
    public class Shape
    {
        public string name;
        public bool[,] mask;
        public Vector2Int shapePosition;
        public Color color;
    }

    [Header("Prefabs & Pool")]
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private ObjectPool bulletPool;

    [Header("Grid Settings")]
    [SerializeField] private int columns = 10;
    [SerializeField] private int rows = 10;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float moveSpeed = 0.5f;

    [Header("References")]
    [SerializeField] private ColorsController colorsController;

    [Header("Debug")]
    [SerializeField] private bool drawGizmos = false;

    private float spawnTimer = 0f;
    private float radius;
    private int currentTopRowZ = 0;

    private void Start()
    {
        radius = CalculateOptimalRadius();
        CenterGrid(columns, rows);
    }

    private void Update()
    {
        MoveGridDownward();
        HandleSpawning();
    }

    private void MoveGridDownward()
    {
        transform.position += Vector3.back * moveSpeed * Time.deltaTime;
    }

    private void HandleSpawning()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            SpawnRow(currentTopRowZ);
            currentTopRowZ--;
            spawnTimer = 0f;
        }
    }

    private float CalculateOptimalRadius()
    {
        float screenWidth = 2f * Camera.main.orthographicSize * Screen.width / Screen.height;
        float xOffset = screenWidth / (columns - 0.5f);
        return xOffset / (2f * 0.866f); // 0.866 = cos(30°) for hex spacing
    }

    private void SpawnRow(int rowZ)
    {
        for (int x = 0; x < columns; x++)
        {
            SpawnBall(x, rowZ);
        }
    }

    private void SpawnBall(int col, int row)
    {
        Vector3 localPosition = GetHexWorldPosition(col, row);
        Vector3 worldPosition = localPosition + transform.position;

        GameObject ballGO = bulletPool.Get();
        BallController ball = ballGO.GetComponent<BallController>();

        ball.Init(colorsController.GetRandomColor(), bulletPool);
        ballGO.transform.position = worldPosition;
        ballGO.transform.SetParent(transform);

        GridManager.Instance.RegisterBall(ball, new Vector2Int(col, row));
    }

    private Vector3 GetHexWorldPosition(int col, int row)
    {
        float xOffset = radius * 2f * 0.866f; // Horizontal distance
        float zOffset = radius * 1.5f;        // Vertical distance

        float xPos = col * xOffset;
        if (Mathf.Abs(row) % 2 == 1)
            xPos += xOffset / 2f;

        float zPos = -row * zOffset;

        return new Vector3(xPos, 0f, zPos);
    }

    private void CenterGrid(int cols, int rows)
    {
        float xOffset = radius * 2f * 0.866f;
        float zOffset = radius * 1.5f;

        float totalWidth = cols * xOffset;
        float totalHeight = rows * zOffset;

        transform.position = new Vector3(
            -totalWidth / 2f + xOffset / 2f,
            0f,
            totalHeight / 2f - zOffset / 2f
        );
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos || !Application.isPlaying) return;

        Gizmos.color = Color.green;
        for (int x = 0; x < columns; x++)
        {
            for (int z = 0; z < rows; z++)
            {
                Vector3 pos = GetHexWorldPosition(x, z) + transform.position;
                Gizmos.DrawWireSphere(pos, radius * 0.9f);
            }
        }
    }
}
