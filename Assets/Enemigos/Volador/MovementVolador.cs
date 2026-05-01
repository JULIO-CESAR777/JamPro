using UnityEngine;

public class MovementVolador : MonoBehaviour
{
    private Transform player;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public float shootCooldown = 1.2f;

    private bool playerInsideZone = false;
    private float nextShootTime;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            print("Si encontre");
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (!playerInsideZone || player == null)
            return;

        if (transform.position.y > player.position.y && transform.position.x == player.position.x)
        {
            if (Time.time >= nextShootTime)
            {
                Shoot();
                nextShootTime = Time.time + shootCooldown;
            }
        }
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
    }

    public void SetPlayerInsideZone(bool inside)
    {
        playerInsideZone = inside;
    }
}