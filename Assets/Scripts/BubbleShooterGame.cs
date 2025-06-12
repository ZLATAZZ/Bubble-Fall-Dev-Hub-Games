using System.Collections.Generic;
using UnityEngine;

public class BubbleShooterGame : MonoBehaviour
{
    [Header("Prefabs & Setup")]
    public GameObject ballPrefab;
    public Transform ballSpawnPoint;

    [Header("Colors Settings")]
    public Color[] ballColors;

    [Header("Ball Chain Settings")]
    public float chainSpeed = 1.5f;
    public float ballSpacing = 0.6f;

    [Header("Ball Launch Settings")]
    public float launchSpeed = 10f;
    public float launchCooldown = 0.5f;

    // Internal
    private List<GameObject> ballChain = new List<GameObject>();
    private GameObject launchingBall;
    private bool canLaunch = true;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        // Initialize chain with some balls coming toward player
        InvokeRepeating(nameof(SpawnBallToChain), 1f, 2f);
    }

    void Update()
    {
        if (launchingBall == null && canLaunch)
        {
            HandleLaunchInput();
        }

        MoveChainForward();
    }

    void SpawnBallToChain()
    {
        // Spawn a new ball at the end of chain
        Vector3 spawnPos;

        if (ballChain.Count == 0)
            spawnPos = new Vector3(0, 4f, 0);
        else
        {
            // Position new ball behind last ball to form a chain
            GameObject lastBall = ballChain[ballChain.Count - 1];
            spawnPos = lastBall.transform.position + new Vector3(0, ballSpacing, 0);
        }

        GameObject newBall = Instantiate(ballPrefab, spawnPos, Quaternion.identity, transform);
        Ball ballComp = newBall.GetComponent<Ball>();
        ballComp.SetColor(ballColors[Random.Range(0, ballColors.Length)]);
        ballChain.Add(newBall);
    }

    void MoveChainForward()
    {
        // Move all balls toward the player (downward)
        if (ballChain.Count == 0) return;

        Vector3 moveVector = Vector3.down * chainSpeed * Time.deltaTime;

        foreach (var ball in ballChain)
        {
            ball.transform.position += moveVector;
        }
    }

    void HandleLaunchInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Calculate shooting direction based on mouse position relative to spawn point
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 shootDir = (mouseWorldPos - ballSpawnPoint.position);
            shootDir.z = 0;
            shootDir.Normalize();

            LaunchBall(shootDir);
        }
    }

    void LaunchBall(Vector3 direction)
    {
        if (!canLaunch) return;

        launchingBall = Instantiate(ballPrefab, ballSpawnPoint.position, Quaternion.identity, transform);
        Ball ballComp = launchingBall.GetComponent<Ball>();
        ballComp.SetColor(ballColors[Random.Range(0, ballColors.Length)]);
        ballComp.Launch(direction, launchSpeed, OnBallStopped);

        canLaunch = false;
        Invoke(nameof(ResetLaunch), launchCooldown);
    }

    void ResetLaunch()
    {
        canLaunch = true;
    }

    void OnBallStopped(GameObject ball)
    {
        // When launched ball stops moving, attach to chain if near other balls

        // Find closest ball to snap position
        Vector3 stickPos = FindStickPosition(ball);

        ball.transform.position = stickPos;

        ballChain.Add(ball);

        CheckAndBurst(ball);

        launchingBall = null;
    }

    Vector3 FindStickPosition(GameObject ball)
    {
        // Simple approach: snap to position near closest ball in chain within range
        float snapDistance = 0.7f;
        Vector3 ballPos = ball.transform.position;

        GameObject closest = null;
        float closestDist = 9999f;

        foreach (var otherBall in ballChain)
        {
            float dist = Vector3.Distance(ballPos, otherBall.transform.position);
            if (dist < closestDist && dist < snapDistance)
            {
                closestDist = dist;
                closest = otherBall;
            }
        }

        if (closest == null)
        {
            // If no close ball, keep position where stopped
            return ballPos;
        }
        else
        {
            // Snap to a position adjacent to closest ball, offset so balls don't overlap
            Vector3 dir = ballPos - closest.transform.position;
            if (dir == Vector3.zero) dir = Vector3.up;
            dir = dir.normalized;
            return closest.transform.position + dir * ballSpacing;
        }
    }

    void CheckAndBurst(GameObject ball)
    {
        // Detect groups of 3 or more balls of same color connected including the newly added ball

        Ball ballComp = ball.GetComponent<Ball>();
        if (!ballComp) return;

        Color targetColor = ballComp.GetColor();

        HashSet<GameObject> connected = new HashSet<GameObject>();
        FindConnectedSameColor(ball, targetColor, connected);

        if (connected.Count >= 3)
        {
            // Burst all connected balls of same color
            foreach (var b in connected)
            {
                ballChain.Remove(b);
                Destroy(b);
            }
        }
    }

    void FindConnectedSameColor(GameObject ball, Color color, HashSet<GameObject> connected)
    {
        if (connected.Contains(ball)) return;

        Ball ballComp = ball.GetComponent<Ball>();
        if (ballComp == null) return;

        if (!ColorsEqual(ballComp.GetColor(), color)) return;

        connected.Add(ball);

        // Find neighbors in chain within ballSpacing * 1.2 approximate radius
        float connectionRadius = ballSpacing * 1.2f;
        Vector3 ballPos = ball.transform.position;

        foreach (var otherBall in ballChain)
        {
            if (otherBall == ball) continue;
            if (connected.Contains(otherBall)) continue;

            float dist = Vector3.Distance(ballPos, otherBall.transform.position);
            if (dist <= connectionRadius)
            {
                FindConnectedSameColor(otherBall, color, connected);
            }
        }
    }

    bool ColorsEqual(Color a, Color b)
    {
        // Compare colors with tolerance to avoid floating point errors
        float tolerance = 0.05f;
        return Mathf.Abs(a.r - b.r) < tolerance &&
               Mathf.Abs(a.g - b.g) < tolerance &&
               Mathf.Abs(a.b - b.b) < tolerance;
    }
}

// Ball script controls launching and color
public class Ball : MonoBehaviour
{
    private Color ballColor;
    private Rigidbody2D rb;
    private bool launched = false;
    private System.Action<GameObject> onStopCallback;

    private float stopVelocityThreshold = 0.1f;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.angularDrag = 0;
            rb.drag = 0.5f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    public void SetColor(Color color)
    {
        ballColor = color;
        if (spriteRenderer != null)
            spriteRenderer.color = ballColor;
    }

    public Color GetColor()
    {
        return ballColor;
    }

    public void Launch(Vector3 direction, float speed, System.Action<GameObject> onStop)
    {
        if (rb == null) return;

        onStopCallback = onStop;
        launched = true;
        rb.velocity = direction * speed;
    }

    void Update()
    {
        if (!launched) return;

        if (rb.velocity.magnitude < stopVelocityThreshold)
        {
            // Stopped moving or nearly so
            launched = false;
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
            onStopCallback?.Invoke(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!launched) return;

        // On collision with other balls or boundaries, stop
        launched = false;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        onStopCallback?.Invoke(gameObject);
    }
}

