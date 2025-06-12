using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public delegate void BallEvent();
    public event BallEvent OnBallAttached;

    private ObjectPool pool;
    public Vector2Int GridPosition { get; set; }

    private Color color;
    public Color Color => color; // защищённый доступ

    public void SetGridPosition(Vector2Int pos)
    {
        GridPosition = pos;
    }

    public void Init(Color newColor, ObjectPool pool)
    {
        this.pool = pool;
        this.color = newColor;
        GetComponent<Renderer>().material.color = newColor;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball") || collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Ceiling"))
        {
            AttachToGrid();
        }
    }

    void AttachToGrid()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        gameObject.transform.SetParent(PlayerShooter.Instance._gridParent.transform);

        // Получаем ближайшую координату в гекс-сетке
        Vector2Int gridPos = GridManager.Instance.GetNearestHexCoord(transform.position);
        Vector3 snappedPos = GridManager.Instance.GetWorldPosition(gridPos);

        // Регистрируем мяч
        GridManager.Instance.RegisterBall(this, gridPos);

        // Проверка на совпадения (кластер)
        BallClusterManager.Instance.CheckAndRemoveCluster(gridPos);

        // Вызываем ивент
        OnBallAttached?.Invoke();
    }

    public void Shoot(Vector3 direction, float force)
    {

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.None; // Сбрасываем заморозку
        rb.isKinematic = false;
        rb.velocity = direction.normalized * force;
    }
}
