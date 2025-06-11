using UnityEngine;

public class Ball : MonoBehaviour
{
    private ObjectPool pool;

    public Color Color { get; private set; }

    public void Init(Color color, ObjectPool pool)
    {
        this.pool = pool;
        this.Color = color;

        var renderer = GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = color;
    }

    public void DestroyBall()
    {
        pool.ReturnToPool(gameObject);
    }
}
