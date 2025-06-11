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
            // Прилепиться к ближайшей позиции
            AttachToGrid();

            OnBallAttached?.Invoke();
        }
    }

    void AttachToGrid()
    {
        // TODO: Реализовать прикрепление к сетке
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().isKinematic = true;
        transform.SetParent(GameObject.Find("HexGrid").transform);
    }

    public void MoveForward()
    {
        transform.position += Vector3.forward * 1f * Time.deltaTime;
    }
}
