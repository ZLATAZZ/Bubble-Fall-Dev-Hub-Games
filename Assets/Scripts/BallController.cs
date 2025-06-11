using UnityEngine;

public class BallController : MonoBehaviour
{
    public delegate void BallEvent();
    public event BallEvent OnBallAttached;

    public Color color;
    private ObjectPool pool;

    public void Init(Color newColor, ObjectPool pool)
    {
        this.pool = pool;
        this.color = newColor;
        GetComponent<Renderer>().material.color = newColor;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball") || collision.gameObject.CompareTag("Wall"))
        {
            // ����������� � ��������� �������
            AttachToGrid();

            OnBallAttached?.Invoke();
        }
    }

    void AttachToGrid()
    {
        // TODO: ����������� ������������ � �����
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().isKinematic = true;
        transform.SetParent(GameObject.Find("HexGrid").transform);
    }

    public void Shoot(Vector3 direction, float force)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false; // �������� ������
        rb.velocity = direction.normalized * force;
    }

}
