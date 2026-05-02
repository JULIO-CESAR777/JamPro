using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    public Animator anim;

    [Header("Ataque")]
    public float damage = 20f;
    public float knockbackForce = 12f;
    public float knockbackUpForce = 3f;
    public float attackDuration = 0.25f;

    private Rigidbody2D rb;
    private Transform currentPoint;
    private float originalScaleX;

    private bool isAttacking;
    private float attackCounter;

    [SerializeField] GameObject dmgZone;

    void Start()
    {
        
            rb = GetComponent<Rigidbody2D>();

            if (pointA != null && pointB != null)
            {
                float distA = Vector2.Distance(transform.position, pointA.position);
                float distB = Vector2.Distance(transform.position, pointB.position);

                currentPoint = distA < distB ? pointA : pointB;
            }

            originalScaleX = Mathf.Abs(transform.localScale.x);
            attackCounter = 0f;
        
    }

    void FixedUpdate()
    {
        if (isAttacking)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        Vector2 direction = (currentPoint.position - transform.position).normalized;

        rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);

        if (Vector2.Distance(rb.position, currentPoint.position) < 0.15f)
        {
            currentPoint = currentPoint == pointA ? pointB : pointA;
        }

        if (direction.x > 0.01f)
        {
            transform.localScale = new Vector3(-originalScaleX, transform.localScale.y, transform.localScale.z);
        }
        else if (direction.x < -0.01f)
        {
            transform.localScale = new Vector3(originalScaleX, transform.localScale.y, transform.localScale.z);
        }
    }

    void Update()
    {
        if (!isAttacking) return;

        attackCounter += Time.deltaTime;

        if (attackCounter >= attackDuration)
        {
            isAttacking = false;
            attackCounter = 0f;

            if (dmgZone != null)
                dmgZone.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();

        if (player == null) return;

        player.TakeDamage((int)damage);

        Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

        if (playerRb != null)
        {
            Vector2 direction = (collision.transform.position - transform.position).normalized;
            direction.y = knockbackUpForce;
            direction.Normalize();

            playerRb.linearVelocity = Vector2.zero;
            playerRb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        }

        isAttacking = true;
        attackCounter = 0f;

        if (dmgZone != null)
            dmgZone.SetActive(true);

        Debug.Log("DañoPlayer");
    }

    public void StartAttack()
    {
        isAttacking = true;
        attackCounter = 0f;

        if (dmgZone != null)
            dmgZone.SetActive(true);
    }

    public void SetPatrolPoints(Transform a, Transform b)
    {
        pointA = a;
        pointB = b;

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        float distA = Vector2.Distance(transform.position, pointA.position);
        float distB = Vector2.Distance(transform.position, pointB.position);

        currentPoint = distA < distB ? pointA : pointB;
    }

    //CUANDO MUERA USAR ESTO GetComponent<EnemyDeath>().Die();
}