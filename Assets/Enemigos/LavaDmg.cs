using UnityEngine;

public class LavaDmg : MonoBehaviour
{
    [Header("Ataque")]
    public float damage = 50f;
    public float knockbackForce = 12f;
    public float knockbackUpForce = 3f;
   

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();

        if (player == null) return;


        player.TakeDamage((int)damage, true);

        Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

        if (playerRb != null)
        {
            Vector2 direction = (collision.transform.position - transform.position).normalized;
            direction.y = knockbackUpForce;
            direction.Normalize();

            playerRb.linearVelocity = Vector2.zero;
            playerRb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        }





       

    }
}
