using System.Collections.Generic;
using UnityEngine;

public class BallClusterManager : MonoBehaviour
{
    public static BallClusterManager Instance { get; private set; }

    public int matchCount = 3;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void CheckAndRemoveCluster(Vector2Int startCoords)
    {
        BallController startBall = GridManager.Instance.GetBallAt(startCoords);
        if (startBall == null) return;

        var sameColorBalls = FindConnectedBalls(startCoords, startBall.Color);

        if (sameColorBalls.Count >= matchCount)
        {
            foreach (var coord in sameColorBalls)
            {
                BallController ball = GridManager.Instance.GetBallAt(coord);
                if (ball != null)
                {
                    Destroy(ball.gameObject); // Здесь можно сделать pool или анимацию
                    GridManager.Instance.UnregisterBall(coord);
                }
            }
        }
    }

    private HashSet<Vector2Int> FindConnectedBalls(Vector2Int startCoords, Color targetColor)
    {
        var visited = new HashSet<Vector2Int>();
        var queue = new Queue<Vector2Int>();
        queue.Enqueue(startCoords);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (visited.Contains(current)) continue;

            BallController ball = GridManager.Instance.GetBallAt(current);
            if (ball != null && ball.Color == targetColor)
            {
                visited.Add(current);

                foreach (var neighbor in GridManager.Instance.GetNeighbors(current))
                {
                    if (!visited.Contains(neighbor))
                        queue.Enqueue(neighbor);
                }
            }
        }

        return visited;
    }
}
