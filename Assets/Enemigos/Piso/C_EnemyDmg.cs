using UnityEngine;

public class C_EnemyDmg : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();

        if (player != null)
        {
           

            player.TakeDamage(20);
            Debug.Log("DañoPlayer");


        }
    }

   
}
