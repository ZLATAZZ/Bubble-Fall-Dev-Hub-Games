using System;
using UnityEngine;

public class BallSc : MonoBehaviour
{
    private Color ballColor;
    private Rigidbody2D rb;
    private bool launched = false;
    private Action<GameObject> onStopCallback;

    private float stopVelocityThreshold = 0.1f;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.angularDrag = 0f;
            rb.drag = 0.5f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    /// <summary>
    /// Set the visual color of the ball.
    /// </summary>
    /// <param name="color">Color to assign</param>
    public void SetColor(Color color)
    {
        ballColor = color;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = ballColor;
        }
    }

    /// <summary>
    /// Get the ball color.
    /// </summary>
    /// <returns>Ball's Color</returns>
    public Color GetColor()
    {
        return ballColor;
    }

    /// <summary>
    /// Launch the ball in a direction with a specified speed.
    /// </summary>
    /// <param name="direction">Normalized direction vector</param>
    /// <param name="speed">Speed multiplier</param>
    /// <param name="onStop">Callback invoked when ball stops moving</param>
    public void Launch(Vector3 direction, float speed, Action<GameObject> onStop)
    {
        if (rb == null) return;

        onStopCallback = onStop;
        launched = true;
        rb.isKinematic = false;
        rb.velocity = direction * speed;
    }

    void Update()
    {
        if (!launched) return;

        if (rb.velocity.magnitude < stopVelocityThreshold)
        {
            // Ball has nearly stopped moving
            launched = false;
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
            onStopCallback?.Invoke(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!launched) return;

        // Stop moving on collision
        launched = false;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        onStopCallback?.Invoke(gameObject);
    }
}

