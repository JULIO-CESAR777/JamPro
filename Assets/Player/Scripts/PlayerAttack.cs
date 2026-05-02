using System;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    #region Singleton

    private static PlayerAttack  instance;
    public static PlayerAttack GetInstance() => instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    #endregion
    
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

    [Header("Ataque especial")] 
    public int killToChargeAttack;
    public int killCounter;
    public bool canDoSpecial;
    
    [SerializeField] private float rayDistance = 8f;
    [SerializeField] private float rayDamage = 50f;
    [SerializeField] private LayerMask rayHitMask;

    [SerializeField] private bool drawDebugRay = true;
    [SerializeField] private LineRenderer rayLine;
    [SerializeField] private float rayLineDuration = 0.08f;

    private Vector2 pendingRayDirection;
    private bool hasPendingRay;

    private bool isShowingRayLine;
    private float rayLineCounter;
    
    
    [Header("Animaciones")]
    private Animator animator;
    [SerializeField] private string attack1ParameterName = "Attack1";
    [SerializeField] private string attackUpParameterName = "AttackUp";
    [SerializeField] private string attack2ParameterName = "Attack2";
    [SerializeField] private string attack3ParameterName = "Attack3";
    
    private int attack1Hash;
    private int attackUpHash;
    private int attack2Hash;
    private int attack3Hash;


    private void Start()
    {
        // Cuerpo a cuerpo
        attack1Counter = 0f;
        dmgZone.enabled = false;
        canAttack1 = true;
        
        // Disparo regular
        canShoot =  true;
        shootCounter = 0f;
        
        // Ataque especial
        killCounter = 0;
        canDoSpecial = false;
        
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
        attack3Hash = Animator.StringToHash(attack3ParameterName);

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

        // Disparo especial
        if (isShowingRayLine)
        {
            rayLineCounter += Time.deltaTime;

            if (rayLineCounter >= rayLineDuration)
            {
                isShowingRayLine = false;

                if (rayLine != null)
                {
                    rayLine.enabled = false;
                }
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

        if (killCounter == killToChargeAttack)
        {
            canDoSpecial = true;
            killCounter = 0;
        }
        if (inputManager.IsButtonDown(BUTTONS.SELECT) && canDoSpecial) // Boton R
        {
            DoSpecial();    
        }
        
    }

    #region Ataque CAC

    private void DoCloseCombatAttack()
    {
        dmgZone.enabled = true;
        canAttack1 = false;
        attack1Counter = 0f;

        PlayAttackAnimation(attack1Hash);
    }

    #endregion
    
    #region Disparo
    
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
    

    #endregion

    #region Disparo Especial

    public void DoSpecial()
    {
        if (!canDoSpecial) return;

        pendingRayDirection = Vector2.right;

        if (playerMovement != null)
        {
            pendingRayDirection = playerMovement.attackDirection;
        }

        if (pendingRayDirection.sqrMagnitude <= 0.01f)
        {
            pendingRayDirection = Vector2.right;
        }

        hasPendingRay = true;

        // Si quieres que el especial se consuma al usarlo:
        canDoSpecial = false;
        killCounter = 0;

        PlayAttackAnimation(attack3Hash);
    }
    
    public void CastSpecialRay()
    {
        if (!hasPendingRay) return;
        if (gm != null && gm.gameState == GameState.Pause) return;

        hasPendingRay = false;

        if (firePoint == null)
        {
            Debug.LogWarning("No hay FirePoint asignado para el rayo.", this);
            return;
        }

        Vector2 origin = firePoint.position;
        Vector2 direction = pendingRayDirection.normalized;

        RaycastHit2D hit;

        if (rayHitMask.value == 0)
        {
            hit = Physics2D.Raycast(origin, direction, rayDistance);
        }
        else
        {
            hit = Physics2D.Raycast(origin, direction, rayDistance, rayHitMask);
        }

        Vector2 endPoint = origin + direction * rayDistance;

        if (hit.collider != null)
        {
            endPoint = hit.point;

            // Intenta hacer daño a cualquier objeto que tenga un método TakeDamage
            hit.collider.SendMessageUpwards(
                "TakeDamage",
                rayDamage,
                SendMessageOptions.DontRequireReceiver
            );

            Debug.Log("Rayo golpeó a: " + hit.collider.name);
        }

        if (drawDebugRay)
        {
            Debug.DrawLine(origin, endPoint, Color.red, 0.25f);
        }

        ShowRayLine(origin, endPoint);
    }

    #endregion
    
    // Lammar animaciones
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
    
    
    // Castear rayo
    private void ShowRayLine(Vector2 startPoint, Vector2 endPoint)
    {
        if (rayLine == null) return;

        rayLine.enabled = true;
        rayLine.positionCount = 2;

        rayLine.SetPosition(0, startPoint);
        rayLine.SetPosition(1, endPoint);

        rayLineCounter = 0f;
        isShowingRayLine = true;
    }


    
    
}
