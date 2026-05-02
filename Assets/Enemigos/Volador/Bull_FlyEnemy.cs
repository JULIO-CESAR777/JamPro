using UnityEngine;

public class Bull_FlyEnemy : MonoBehaviour
{
   
    public float speed = 6f;
    public int damage = 10;

    void Update()
    {
        transform.position += Vector3.down * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Player"))
        {


            PlayerHealth.GetInstance().TakeDamage(15, true);

            Destroy(gameObject);
        }
    }

}
