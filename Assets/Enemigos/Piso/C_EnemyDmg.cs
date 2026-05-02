using UnityEngine;

public class C_EnemyDmg : MonoBehaviour
{
    public float damage = 20f;
    public float knockbackForce = 12f;
    public float knockbackUpForce = 3f;

    private EnemyPatrol enemyPatrol;
    private Collider2D playerInside;

    private void Awake()
    {
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerHealth>() != null)
        {
            playerInside = collision;
        }
        DealDamage();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == playerInside)
        {
            playerInside = null;
        }
    }

    // Llamar desde Animation Event
    public void DealDamage()
    {
        if (playerInside == null) return;

        PlayerHealth player = playerInside.GetComponent<PlayerHealth>();
        if (player == null) return;

        PlayerHealth.GetInstance().TakeDamage(15);

        Rigidbody2D playerRb = playerInside.GetComponent<Rigidbody2D>();

        if (playerRb != null)
        {
            Vector2 direction = (playerInside.transform.position - transform.position).normalized;
            direction.y = knockbackUpForce;
            direction.Normalize();

            playerRb.linearVelocity = Vector2.zero;
            playerRb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        }

        Debug.Log("DañoPlayer");
    }
}