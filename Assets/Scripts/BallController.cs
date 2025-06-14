using System.Collections;
using UnityEngine;

/// <summary>
/// Управляет поведением шара: столкновения, выстрел, привязка к сетке.
/// </summary>
public class BallController : MonoBehaviour
{
    public delegate void BallEvent();
    public event BallEvent OnBallAttached;

    private ObjectPool _pool;
    private Color _color;

    public Vector2Int GridPosition { get; private set; }
    public Color Color => _color;

    /// <summary>
    /// Инициализация шара цветом и пулом.
    /// </summary>
    public void Init(Color newColor, ObjectPool pool)
    {
        _pool = pool;
        _color = newColor;
        GetComponent<Renderer>().material.color = newColor;
    }

    /// <summary>
    /// Устанавливает позицию шара в сетке.
    /// </summary>
    public void SetGridPosition(Vector2Int pos)
    {
        GridPosition = pos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            SoundManager.Instance.PlayBallHit();
            AttachToGrid();
        }

        if (collision.gameObject.CompareTag("Finish"))
        {
            GameSceneUIController.Instance.GameOver();
        }
    }

    /// <summary>
    /// Привязывает шар к ближайшей ячейке сетки и запускает проверку кластера.
    /// </summary>
    private void AttachToGrid()
    {
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        // Привязываем к визуальному родителю сетки
        transform.SetParent(PlayerShooter.Instance.GridParent.transform);

        // Определяем ближайшую позицию в гекс-сетке и снапим
        Vector2Int gridPos = GridManager.Instance.GetNearestHexCoord(transform.position);
        Vector3 snappedPos = GridManager.Instance.GetWorldPosition(gridPos);

        transform.position = snappedPos;
        SetGridPosition(gridPos);

        GridManager.Instance.RegisterBall(this, gridPos);

        // Вместо запуска корутины тут вызываем метод менеджера, который запустит корутину
        BallClusterManager.Instance.StartClusterCheckAfterSleep(this, gridPos);

        OnBallAttached?.Invoke();
    }

    /// <summary>
    /// Запускает шар в указанном направлении с заданной силой.
    /// </summary>
    public void Shoot(Vector3 direction, float force)
    {
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.velocity = direction.normalized * force;

        SoundManager.Instance.PlayLaserFire();
        EffectManager.Instance.SpawnReflect(transform.position, direction);
    }

    /// <summary>
    /// Возвращает шар в пул и деактивирует его.
    /// </summary>
    public void Deactivate()
    {
        _pool.ReturnToPool(gameObject);
    }
}
