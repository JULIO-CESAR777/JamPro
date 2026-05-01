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
           

            //if (player != null)
            //{
            //    player.TakeDamage(damage);
           //

            Destroy(gameObject);
        }
    }

}
