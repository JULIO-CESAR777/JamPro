using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    private EnemySpawner spawner;

    public int health = 100;

    private Animator anim;

    
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
            anim.SetTrigger("Die");
            Die();
        }
        else
        {
            anim.SetTrigger("Injured");

        }
    }

  

  
  

}