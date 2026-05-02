using System;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    // Referencias
    InputManager inputManager;
    GameManager gm;
    PlayerMovement playerMovement;
    PlayerHealth playerHealth;
    
    [Header("Ataque cuerpo a cuerpo")]
    [SerializeField] Collider2D dmgZone;
    public bool canAttack1;
    
    public int CloseCombatDmg = 25;
    
    public float attack1Cooldown = .4f; // Tiempo que se tarda en volver a atacar
    private float attack1Counter; // Contador
    public float attack1Duration = 0.3f; // Lo que dura el collider encendido
    
    [Header("Disparo Regular")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePoint;
    public bool canShoot;

    public float shootCooldown;

    public float dmgToPlayer = 10f;
    private float shootCounter;

    private void Start()
    {
        // Cuerpo a cuerpo
        attack1Counter = 0f;
        dmgZone.enabled = false;
        canAttack1 = true;
        
        // Disparo regular
        canShoot =  true;
        shootCounter = 0f;
        
        // Manangers
        gm = GameManager.GetInstance();
        inputManager = InputManager.GetInstance();
        playerMovement = GetComponent<PlayerMovement>();
        playerHealth = PlayerHealth.GetInstance();
        
    }

    void Update()
    {
        if (gm.gameState == GameState.Pause) return;
        
        // Counter de ataque cuerpo a cuerpo
        if (!canAttack1)
        {
            attack1Counter += Time.deltaTime;

            // Apaga la zona de daño
            if (attack1Counter >= attack1Duration)
            {
                dmgZone.enabled = false;
            }
            
            // Deja atacar de nuevo
            if (attack1Counter >= attack1Cooldown)
            {
                attack1Counter = 0f;
                canAttack1 = true;
            }
        }
        
        // Counter del disparo simple
        if (!canShoot)
        {
            shootCounter += Time.deltaTime;
            if (shootCounter >= shootCooldown)
            {
                shootCounter = 0f;
                canShoot = true;
            }
        }

        if (inputManager.IsButton(BUTTONS.LEFT_CLICK) && canAttack1)
        {
            dmgZone.enabled = true;
            canAttack1 = false;
        }

        if (inputManager.IsButton(BUTTONS.RIGHT_CLICK) && canShoot)
        {
            canShoot = false;
            Shoot();
        }
        
    }
    
    private void Shoot()
    {
        canShoot = false;
        
        playerHealth.TakeDamage(dmgToPlayer);

        Vector2 shootDirection = Vector2.right;

        if (playerMovement != null)
        {
            shootDirection = playerMovement.attackDirection;
        }

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.identity
        );

        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if (bulletScript != null)
        {
            bulletScript.SetDirection(shootDirection);
        }
    }
    
}
