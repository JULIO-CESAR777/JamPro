using UnityEngine;

public class DetectEnemyDmg : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
           EnemyDeath enemy = collision.gameObject.GetComponent<EnemyDeath>();

            if (enemy != null)
            {

                enemy.GetDmgEnemy(PlayerAttack.GetInstance().CloseCombatDmg);
                PlayerHealth.GetInstance().Heal(5);
                print("da�odelplayer");
            }
            

        }
    }
}
