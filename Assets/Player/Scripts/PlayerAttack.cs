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
    private Vector2 pendingShootDirection;
    private bool hasPendingShot;
    
    
    [Header("Animaciones")]
    private Animator animator;
    [SerializeField] private string attack1ParameterName = "Attack1";
    [SerializeField] private string attackUpParameterName = "AttackUp";
    [SerializeField] private string attack2ParameterName = "Attack2";
    
    private int attack1Hash;
    private int attackUpHash;
    private int attack2Hash;


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
        
        // Animaciones
        animator = GetComponent<Animator>();
        
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        
        attack1Hash = Animator.StringToHash(attack1ParameterName);
        attackUpHash = Animator.StringToHash(attackUpParameterName);
        attack2Hash = Animator.StringToHash(attack2ParameterName);


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

        if (inputManager.IsButtonDown(BUTTONS.LEFT_CLICK) && canAttack1)
        {
            DoCloseCombatAttack();
        }

        if (inputManager.IsButtonDown(BUTTONS.RIGHT_CLICK) && canShoot)
        {
            StartShootAttack();
        }
        
    }
    
    
    private void DoCloseCombatAttack()
    {
        dmgZone.enabled = true;
        canAttack1 = false;
        attack1Counter = 0f;

        PlayAttackAnimation(attack1Hash);
    }
    
    private void PlayAttackAnimation(int attackHash)
    {
        if (animator == null) return;

        bool attackingUp = false;

        if (playerMovement != null)
        {
            attackingUp = playerMovement.attackDirection == Vector2.up;
        }

        animator.SetFloat(attackUpHash, attackingUp ? 1f : 0f);
        animator.SetTrigger(attackHash);
    }
    
    private void StartShootAttack()
    {
        canShoot = false;
        shootCounter = 0f;

        pendingShootDirection = Vector2.right;

        if (playerMovement != null)
        {
            pendingShootDirection = playerMovement.attackDirection;
        }

        hasPendingShot = true;

        PlayAttackAnimation(attack2Hash);
    }
    
    public void Shoot()
    {
        if (!hasPendingShot) return;
        if (gm != null && gm.gameState == GameState.Pause) return;

        hasPendingShot = false;

        if (bulletPrefab == null)
        {
            Debug.LogWarning("No hay Bullet Prefab asignado.", this);
            return;
        }

        if (firePoint == null)
        {
            Debug.LogWarning("No hay FirePoint asignado.", this);
            return;
        }

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(dmgToPlayer);
        }

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.identity
        );

        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if (bulletScript != null)
        {
            bulletScript.SetDirection(pendingShootDirection);
        }
    }
    
    
}
