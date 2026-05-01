using UnityEngine;

public class EnemyFollowX : MonoBehaviour
{
    public Transform player;
    public float speed = 3f;
    public float attackDistance = 1.5f;

    private Rigidbody2D rb;
    private Animator animator;

    private bool isAttacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distance = player.position.x - transform.position.x;

        // Si está cerca → atacar
        if (Mathf.Abs(distance) <= attackDistance)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            if (!isAttacking)
            {
                isAttacking = true;
                animator.SetTrigger("attack");
            }
        }
        else
        {
            isAttacking = false;

            float direction = Mathf.Sign(distance);
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        }
    }
}