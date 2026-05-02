using UnityEngine;

public class MovementVolador : MonoBehaviour
{
    //Sistema de guardado
    private GameManager gm;
    [SerializeField] private bool isPaused = false;
    private Transform player;

    private Animator anim;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public float shootCooldown = 1.2f;
    public float tolerance = 1f;
    public float moveSpeed = 3f;

    private bool playerInsideZone = false;
    private float nextShootTime;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        anim= GetComponent<Animator>();

        gm = GameManager.instance;
        if (gm != null)
        {
            gm.onChangeGameState += OnChangeGameStateCallback;

            if (gm.gameState == GameState.Pause)
                isPaused = true;
        }

        if (playerObj != null)
        {
            print("Si encontre");
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (!playerInsideZone || player == null || isPaused)
            return;

        float xDistance = Mathf.Abs(transform.position.x - player.position.x);

        if (transform.position.y > player.position.y && xDistance < tolerance)
        {
            if (Time.time >= nextShootTime)
            {
                Shoot();
                nextShootTime = Time.time + shootCooldown;
            }
        }
        else
        {
            float directionx = Mathf.Sign(player.position.x - transform.position.x);

            transform.position += new Vector3(directionx * moveSpeed * Time.deltaTime, 0, 0);
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


     public void OnChangeGameStateCallback(GameState newState)
    {
        isPaused = newState != GameState.Play;

        anim.speed = isPaused ? 0f : 1f;
    }
    //CUANDO MUERA USAR ESTO GetComponent<EnemyDeath>().Die();


}