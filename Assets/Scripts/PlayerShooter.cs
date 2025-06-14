using UnityEngine;

/// <summary>
/// Отвечает за выстрел шаров игроком.
/// </summary>
public class PlayerShooter : MonoBehaviour
{
    public static PlayerShooter Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Transform shootPoint;
    [SerializeField] private ObjectPool ballsPool;
    [SerializeField] private LaserController laser;
    [SerializeField] private ColorsController colorsController;
    [SerializeField] private GameObject gridParent;

    [Header("Settings")]
    [SerializeField] private float shootForce = 10f;
    [SerializeField] private int matchCount = 3;

    private bool isAiming = false;
    private GameObject currentBall;

    /// <summary>
    /// Родитель для шаров в сцене.
    /// </summary>
    public GameObject GridParent => gridParent;

    /// <summary>
    /// Минимальное количество шаров для матча.
    /// </summary>
    public int MatchCount => matchCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        HandleInput();
    }

    /// <summary>
    /// Обработка нажатий мыши.
    /// </summary>
    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PrepareBall();
        }

        if (Input.GetMouseButtonUp(0) && isAiming)
        {
            FireLaser();
            isAiming = false;
        }
    }

    /// <summary>
    /// Получает шар из пула и инициализирует его.
    /// </summary>
    private void PrepareBall()
    {
        currentBall = ballsPool.Get();
        currentBall.transform.position = shootPoint.position;
        currentBall.transform.rotation = Quaternion.identity;

        var ballCtrl = currentBall.GetComponent<BallController>();
        ballCtrl.Init(colorsController.GetRandomColor(), ballsPool);

        isAiming = true;
    }

    /// <summary>
    /// Выполняет выстрел в направлении лазера.
    /// </summary>
    private void FireLaser()
    {
        if (currentBall == null) return;

        Vector3 direction = laser.LaserDirection.normalized;
        currentBall.GetComponent<BallController>().Shoot(direction, shootForce);
    }
}
