using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    public Animator anim;

    private Rigidbody2D rb;
    private Transform currentPoint;
    private float originalScaleX;

    [SerializeField] GameObject dmgZone;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        float distA = Vector2.Distance(transform.position, pointA.position);
        float distB = Vector2.Distance(transform.position, pointB.position);

        currentPoint = distA < distB ? pointA : pointB;

        originalScaleX = Mathf.Abs(transform.localScale.x);
    }

    void FixedUpdate()
    {
        Vector2 direction = (currentPoint.position - transform.position).normalized;

        rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);

        

        if (Vector2.Distance(rb.position, currentPoint.position) < 0.15f)
        {
            currentPoint = currentPoint == pointA ? pointB : pointA;
            if (direction.x > 0.01f)
            {
                transform.localScale = new Vector3(-originalScaleX, transform.localScale.y, transform.localScale.z);
            }
            else if (direction.x < -0.01f)
            {
                transform.localScale = new Vector3(originalScaleX, transform.localScale.y, transform.localScale.z);
            }
        }

        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
}