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
        [SerializeField] GameObject specialBulletPrefab;
        public int killToChargeAttack;
        public int killCounter;
        public bool canDoSpecial;
        
        [SerializeField] private float rayDistance = 8f;
        [SerializeField] private float rayDamage = 50f;
        [SerializeField] private LayerMask rayHitMask;

        [SerializeField] private bool drawDebugRay = true;
        [SerializeField] private float rayLineDuration = 0.08f;
        [SerializeField] private float rayWidth = 0.25f;

        [SerializeField] private LayerMask wallStopMask;
        [SerializeField] private LayerMask enemyDamageMask;

        [SerializeField] private float specialBulletDuration = 0.12f;

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
            
            if (killCounter >= killToChargeAttack)
            {
                canDoSpecial = true;
            }
            
            
            // Deteccion de Inputs
            if (inputManager.IsButtonDown(BUTTONS.LEFT_CLICK) && canAttack1)
            {
                DoCloseCombatAttack();
            }

            if (inputManager.IsButtonDown(BUTTONS.RIGHT_CLICK) && canShoot)
            {
                StartShootAttack();
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

            // Consumir ataque especial
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

            if (direction.sqrMagnitude <= 0.01f)
            {
                direction = Vector2.right;
            }

            Vector2 maxEndPoint = origin + direction * rayDistance;
            Vector2 finalEndPoint = maxEndPoint;

            // 1. Primero revisamos si el rayo toca una pared.
            // Este raycast SOLO revisa la layer de pared.
            RaycastHit2D wallHit = Physics2D.Raycast(
                origin,
                direction,
                rayDistance,
                wallStopMask
            );

            if (wallHit.collider != null)
            {
                finalEndPoint = wallHit.point;
            }

            // 2. Dañar enemigos en el camino.
            // Esto NO detiene el rayo. Solo detecta enemigos entre origin y finalEndPoint.
            float distanceToEnd = Vector2.Distance(origin, finalEndPoint);

            RaycastHit2D[] enemyHits = Physics2D.CircleCastAll(
                origin,
                rayWidth,
                direction,
                distanceToEnd,
                enemyDamageMask
            );

            for (int i = 0; i < enemyHits.Length; i++)
            {
                Collider2D enemyCollider = enemyHits[i].collider;

                if (enemyCollider == null) continue;

                enemyCollider.SendMessageUpwards(
                    "TakeDamage",
                    rayDamage,
                    SendMessageOptions.DontRequireReceiver
                );

                Debug.Log("Rayo dañó a: " + enemyCollider.name);
            }

            // 3. Instanciar el prefab visual del rayo.
            SpawnSpecialBulletVisual(origin, finalEndPoint);

            if (drawDebugRay)
            {
                Debug.DrawLine(origin, finalEndPoint, Color.red, 0.3f);
            }
        }
        
        private void SpawnSpecialBulletVisual(Vector2 startPoint, Vector2 endPoint)
        {
            if (specialBulletPrefab == null)
            {
                Debug.LogWarning("No hay Special Bullet Prefab asignado.", this);
                return;
            }

            Vector2 direction = endPoint - startPoint;
            float distance = direction.magnitude;

            if (distance <= 0.01f) return;

            Vector2 middlePoint = startPoint + direction * 0.5f;

            GameObject specialBullet = Instantiate(
                specialBulletPrefab,
                middlePoint,
                Quaternion.identity
            );

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            specialBullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            SpecialBulletVisual visual = specialBullet.GetComponent<SpecialBulletVisual>();

            if (visual != null)
            {
                visual.SetVisualLength(distance, specialBulletDuration);
            }
            else
            {
                // Si no tiene script, intentamos escalarlo directamente.
                Vector3 scale = specialBullet.transform.localScale;
                scale.x = distance;
                specialBullet.transform.localScale = scale;

                Destroy(specialBullet, specialBulletDuration);
            }
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
    }
