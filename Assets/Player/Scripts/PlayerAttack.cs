    using System;
    using System.Collections.Generic;
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
        
        [Header("Referencias de ataque")]
        [SerializeField] private Transform attackZone;
        
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
        private Vector2 pendingShootOffset;

        [Header("Ataque especial")] 
        [SerializeField] GameObject specialBulletPrefab;
        public int killToChargeAttack;
        public int killCounter;
        public bool canDoSpecial;
        public int damageFromSpecial;
        
        [SerializeField] private float rayDistance = 8f;
        [SerializeField] private int rayDamage = 50;
        [SerializeField] private LayerMask rayHitMask;

        [SerializeField] private bool drawDebugRay = true;
        [SerializeField] private float rayLineDuration = 0.08f;
        [SerializeField] private float rayWidth = 0.25f;

        [SerializeField] private LayerMask wallStopMask;
        [SerializeField] private LayerMask enemyDamageMask;

        [SerializeField] private float specialBulletDuration = 0.12f;
        
        [SerializeField] private List<GameObject> CargaDeCruz;

        private Vector2 pendingRayDirection;
        private bool hasPendingRay;
        private Vector2 pendingRayOffset;
        
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
            for (int i = 0; i < CargaDeCruz.Count; i++)
            {
                CargaDeCruz[i].SetActive(false);
            }
            
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
                print("Shoot");
                StartShootAttack();
            }
            
            if (inputManager.IsButtonDown(BUTTONS.SELECT) && canDoSpecial) // Boton R
            {
                DoSpecial();    
            }
            
        }

        private Vector2 GetDirectionFromAttackZone()
        {
            if (attackZone == null)
            {
                if (playerMovement != null)
                {
                    return playerMovement.attackDirection;
                }

                return Vector2.right;
            }

            Vector2 dir = attackZone.position - transform.position;

            if (dir.sqrMagnitude <= 0.01f)
            {
                return Vector2.right;
            }

            // Si el AttackZone está más arriba que a los lados, dispara hacia arriba
            if (Mathf.Abs(dir.y) > Mathf.Abs(dir.x))
            {
                return Vector2.up;
            }

            // Si está más a la derecha o izquierda, dispara hacia ese lado
            return dir.x >= 0f ? Vector2.right : Vector2.left;
        }
        
        private Vector2 GetAttackOrigin()
        {
            if (firePoint != null)
            {
                return firePoint.position;
            }

            if (attackZone != null)
            {
                return attackZone.position;
            }

            return transform.position;
        }
        
        #region Ataque CAC

        private void DoCloseCombatAttack()
        {
            dmgZone.enabled = true;
            canAttack1 = false;
            attack1Counter = 0f;

            Vector2 currentAttackDirection = GetDirectionFromAttackZone();

            PlayAttackAnimation(attack1Hash, currentAttackDirection);
        }

        #endregion
        
        #region Disparo
        
        private void StartShootAttack()
        {
            canShoot = false;
            shootCounter = 0f;

            hasPendingShot = true;

            Vector2 currentAttackDirection = GetDirectionFromAttackZone();

            PlayAttackAnimation(attack2Hash, currentAttackDirection);
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

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(dmgToPlayer, false);
            }

            Vector2 shootDirection = GetDirectionFromAttackZone();
            Vector2 spawnPosition = GetAttackOrigin();

            GameObject bullet = Instantiate(
                bulletPrefab,
                spawnPosition,
                Quaternion.identity
            );

            Bullet bulletScript = bullet.GetComponent<Bullet>();

            if (bulletScript != null)
            {
                bulletScript.SetDirection(shootDirection);
            }
        }
        

        #endregion

        #region Disparo Especial

        public void DoSpecial()
        {
            if (!canDoSpecial) return;

            hasPendingRay = true;

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageFromSpecial, false);
            }

            canDoSpecial = false;
            killCounter = 0;

            Vector2 currentAttackDirection = GetDirectionFromAttackZone();

            PlayAttackAnimation(attack3Hash, currentAttackDirection);
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

            Vector2 origin = GetAttackOrigin();
            Vector2 direction = GetDirectionFromAttackZone();

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
                
                EnemyDeath enemyDeath = enemyCollider.GetComponentInParent<EnemyDeath>();

                if (enemyDeath != null)
                {
                    enemyDeath.GetDmgEnemy(rayDamage);
                }
                else
                {
                    Debug.LogWarning("No se encontró EnemyDeath en: " + enemyCollider.name);
                }
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

        public void AddToKillCounter()
        {
            killCounter++;
            if (killCounter <= killToChargeAttack)
            {
                CargaDeCruz[killCounter-1].SetActive(true);
            }
        }

        #endregion
        
        // Lammar animaciones
        private void PlayAttackAnimation(int attackHash, Vector2 attackDirection)
        {
            if (animator == null) return;

            bool attackingUp = attackDirection == Vector2.up;

            animator.SetFloat(attackUpHash, attackingUp ? 1f : 0f);
            animator.SetTrigger(attackHash);
        }
    }
