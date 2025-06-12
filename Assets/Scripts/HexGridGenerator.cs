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

    public GameObject ballPrefab;
    public ObjectPool bulletPool;

    public float radius = 0.5f;
    public float moveSpeed = 0.5f;

    public List<Shape> shapes = new List<Shape>();
    [SerializeField] private int cols = 10;
    [SerializeField] private int rows = 10;

    public float spawnInterval = 2f;
    private float spawnTimer;
    private int topRowZ;

    public ColorsController colorsController;

    void Start()
    {
        radius = CalculateRadiusToFit();
        CenterGrid(cols, rows);

        topRowZ = 0;

        Debug.Log(topRowZ);

    }

    void Update()
    {
        transform.position += Vector3.back * moveSpeed * Time.deltaTime;
        

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            SpawnTopRow();
            spawnTimer = 0f;
        }
    }

    float CalculateRadiusToFit()
    {
        int maxCols = 10;
        float screenWidth = 2f * Camera.main.orthographicSize * Screen.width / Screen.height;
        float xOffset = screenWidth / (maxCols - 0.5f);
        return xOffset / (2f * 0.866f);
    }


    void SpawnTopRow()
    {

        for (int x = 0; x < cols; x++)
        {
            SpawnBall(x, topRowZ);
        }

        topRowZ--;
    }


    void SpawnBall(int x, int z)
    {
        

        Vector3 localPos = HexToWorldPosition(x, z);
        Vector3 worldPos = localPos + transform.position;

        GameObject ball = bulletPool.Get();
        BallController ballComp = ball.GetComponent<BallController>();
        ballComp.Init(colorsController.GetRandomColor(), bulletPool);


        ball.transform.position = worldPos;
        ball.transform.SetParent(transform);

        GridManager.Instance.RegisterBall(ballComp, new Vector2Int(x, z));

    }


    Vector3 HexToWorldPosition(int x, int z)
    {
        float xOffset = radius * 2f * 0.866f;
        float zOffset = radius * 1.5f;

        float xPos = x * xOffset;
        if (Mathf.Abs(z) % 2 == 1) xPos += xOffset / 2f;
        float zPos = -z * zOffset;
        return new Vector3(xPos, 0f, zPos);
    }


    void CenterGrid(int cols, int rows)
    {
        float xOffset = radius * 2f * 0.866f;
        float zOffset = radius * 1.5f;

        float totalWidth = cols * xOffset;
        float totalHeight = rows * zOffset;

        transform.position = new Vector3(-totalWidth / 2f + xOffset / 2f, 0f, totalHeight / 2f - zOffset / 2f);
    }

}
