using System.Collections.Generic;
using UnityEngine;

public class BallClusterManager : MonoBehaviour
{
    public static BallClusterManager Instance { get; private set; }

    public int matchCount = 3;

    private HashSet<Vector2Int> removedThisFrame = new HashSet<Vector2Int>();

   

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void LateUpdate()
    {
        removedThisFrame.Clear(); // очищаем в конце кадра
    }


    public void CheckAndRemoveCluster(Vector2Int startCoords)
    {
        BallController startBall = GridManager.Instance.GetBallAt(startCoords);
        if (startBall == null) return;

        
        Color targetColor = startBall.Color;

        var sameColorBalls = FindConnectedBalls(startCoords, targetColor);

        if (removedThisFrame.Contains(startCoords))
            return;
        removedThisFrame.UnionWith(sameColorBalls);

        if (sameColorBalls.Count >= matchCount)
        {
            // ƒобавл€ем очки за лопнувшие
            ScoreManager.Instance.AddPopPoints(sameColorBalls.Count);
            SaveManager.Instance.Save();

            foreach (var coord in sameColorBalls)
            {
                BallController ball = GridManager.Instance.GetBallAt(coord);
                if (ball != null && ball.Color == targetColor)
                {
                    Destroy(ball.gameObject);
                    GridManager.Instance.UnregisterBall(coord);
                }
            }

            // ѕотом рон€ем осыпавшиес€
            DropFloatingBalls();
        }

    }



    public void CheckAroundCluster(Vector2Int center)
    {
        var neighbors = GridManager.Instance.GetNeighbors(center);
        foreach (var neighbor in neighbors)
        {
            CheckAndRemoveCluster(neighbor);
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

    public void DropFloatingBalls()
    {
        var allBalls = GridManager.Instance.GetAllBallCoords();
        HashSet<Vector2Int> connectedToTop = new HashSet<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        int droppedCount = 0;

        foreach (var coord in allBalls)
        {
            if (coord.y == 0)
            {
                queue.Enqueue(coord);
                connectedToTop.Add(coord);
            }
        }

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            foreach (var neighbor in GridManager.Instance.GetNeighbors(current))
            {
                if (!connectedToTop.Contains(neighbor) && GridManager.Instance.HasBallAt(neighbor))
                {
                    connectedToTop.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        foreach (var coord in allBalls)
        {
            if (!connectedToTop.Contains(coord))
            {
                BallController ball = GridManager.Instance.GetBallAt(coord);
                if (ball != null)
                {
                    GridManager.Instance.UnregisterBall(coord);

                    Rigidbody rb = ball.GetComponent<Rigidbody>();
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    rb.constraints = RigidbodyConstraints.None;

                    ball.transform.SetParent(null);
                    Destroy(ball.gameObject, 2f);

                    droppedCount++;
                }
            }
        }

        if (droppedCount > 0)
        {
            ScoreManager.Instance.AddPopPoints(droppedCount);
            SaveManager.Instance.Save();
        }
            

    }



}
