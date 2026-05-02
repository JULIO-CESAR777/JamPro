using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    private EnemySpawner spawner;

    public int health = 100;

    
    public void SetSpawner(EnemySpawner enemySpawner)
    {
        spawner = enemySpawner;
    }

    public void Die()
    {
        if (spawner != null)
            spawner.EnemyDied();

        PlayerAttack.GetInstance().killCounter++;

        Destroy(gameObject);
    }

    public void GetDmgEnemy(int dmg)
    {
        health -= dmg;
        if (health <= 0)
        {

            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if(collision.CompareTag("DmgZone"))
        {
            GetDmgEnemy(PlayerAttack.GetInstance().CloseCombatDmg);
            print("CACAttack");
        }
    }
}