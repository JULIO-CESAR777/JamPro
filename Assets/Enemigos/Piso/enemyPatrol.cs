using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    public Animator anim;
    private Rigidbody2D rb;
    private Transform currentPoint;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Elegir el punto inicial
        float distA = Vector2.Distance(transform.position, pointA.position);
        float distB = Vector2.Distance(transform.position, pointB.position);

        currentPoint = distA < distB ? pointA : pointB;
    }

    void FixedUpdate()
    {
        Vector2 direction = (currentPoint.position - transform.position).normalized;

        rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);

        if (Vector2.Distance(transform.position, currentPoint.position) < 0.15f)
        {
            currentPoint = currentPoint == pointA ? pointB : pointA;
        }
    }
}