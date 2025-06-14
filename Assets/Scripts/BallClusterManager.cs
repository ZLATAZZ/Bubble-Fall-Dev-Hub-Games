using System.Collections;
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

    /// <summary>
    /// «апускает проверку и удаление кластера после того, как шар остановитс€ (корутина запускаетс€ здесь, где объект активен)
    /// </summary>
    public void StartClusterCheckAfterSleep(BallController ball, Vector2Int gridPos)
    {
        StartCoroutine(ClusterCheckCoroutine(ball, gridPos));
    }

    private IEnumerator ClusterCheckCoroutine(BallController ball, Vector2Int gridPos)
    {
        float timeout = 3f;  // 3 секунды максимум
        float timer = 0f;

        Rigidbody rb = ball.GetComponent<Rigidbody>();

        while (!rb.IsSleeping() && timer < timeout)
        {
            yield return null;
            timer += Time.deltaTime;
        }

        // ƒаже если не заснул, подождЄм ещЄ чуть-чуть, чтоб физика устаканилась
        yield return new WaitForSeconds(0.05f);

        CheckAndRemoveCluster(gridPos);
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
            ScoreManager.Instance.AddPopPoints(sameColorBalls.Count);
            SaveManager.Instance.Save();

            foreach (var coord in sameColorBalls)
            {
                BallController ball = GridManager.Instance.GetBallAt(coord);
                if (ball != null && ball.Color == targetColor)
                {
                    ball.Deactivate();
                    GridManager.Instance.UnregisterBall(coord);
                }
            }

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
                    ball.Deactivate();

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
