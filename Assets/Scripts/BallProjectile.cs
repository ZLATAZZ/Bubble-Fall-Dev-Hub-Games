using UnityEngine;

public class BallProjectile : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private ObjectPool pool;
    private System.Action onStop;

    public Color color;
    private Rigidbody rb;

    public void Init(Vector3 dir, float spd, ObjectPool pool, Color color, System.Action onStop)
    {
        this.direction = dir;
        this.speed = spd;
        this.pool = pool;
        this.color = color;
        this.onStop = onStop;

        rb = GetComponent<Rigidbody>();
        rb.velocity = direction * speed;

        GetComponent<Renderer>().material.color = color;
    }

    void FixedUpdate()
    {
        // Reflect if hit left or right wall
        if (transform.position.x < -6f || transform.position.x > 6f)
        {
            direction.x *= -1;
            rb.velocity = direction * speed;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ball") || other.gameObject.CompareTag("Wall"))
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;

            // TODO: найти ближайшую ячейку в HexGrid и прикрепить туда
            FindAttachPoint();

            onStop?.Invoke();
        }
    }

    void FindAttachPoint()
    {
        // TODO: Алгоритм поиска ближайшей пустой ячейки в HexGrid
        // Например:
        // Vector3 nearest = HexGrid.Instance.FindNearestEmptyPosition(transform.position);
        // transform.position = nearest;
        // transform.SetParent(HexGrid.Instance.transform);
    }
}
