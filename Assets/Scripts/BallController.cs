using System.Collections;
using UnityEngine;

/// <summary>
/// ��������� ���������� ����: ������������, �������, �������� � �����.
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
    /// ������������� ���� ������ � �����.
    /// </summary>
    public void Init(Color newColor, ObjectPool pool)
    {
        _pool = pool;
        _color = newColor;
        GetComponent<Renderer>().material.color = newColor;
    }

    /// <summary>
    /// ������������� ������� ���� � �����.
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
    /// ����������� ��� � ��������� ������ ����� � ��������� �������� ��������.
    /// </summary>
    private void AttachToGrid()
    {
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        // ����������� � ����������� �������� �����
        transform.SetParent(PlayerShooter.Instance.GridParent.transform);

        // ���������� ��������� ������� � ����-����� � ������
        Vector2Int gridPos = GridManager.Instance.GetNearestHexCoord(transform.position);
        Vector3 snappedPos = GridManager.Instance.GetWorldPosition(gridPos);

        transform.position = snappedPos;
        SetGridPosition(gridPos);

        GridManager.Instance.RegisterBall(this, gridPos);

        // ������ ������� �������� ��� �������� ����� ���������, ������� �������� ��������
        BallClusterManager.Instance.StartClusterCheckAfterSleep(this, gridPos);

        OnBallAttached?.Invoke();
    }

    /// <summary>
    /// ��������� ��� � ��������� ����������� � �������� �����.
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
    /// ���������� ��� � ��� � ������������ ���.
    /// </summary>
    public void Deactivate()
    {
        _pool.ReturnToPool(gameObject);
    }
}
