using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    // Managers
    private GameManager gm;
    private InputManager inputManager;
    private Rigidbody2D rb;
    private Animator animator;

    [SerializeField] private bool isPaused = false;

    private float horizontal;
    private float vertical;
    private float facingDirection = 1f;

    private bool jumpHeld;
    private bool jumpReleased;

    [Header("Movimiento")]
    public float speed = 5f;
    public float acceleration = 40f;
    public float deceleration = 50f;
    public float airAcceleration = 25f;
    public float airDeceleration = 20f;
    
    [Header("Salto")]
    public float jumpPower = 8f;
    public float riseMultiplier = 1.8f;
    public float fallMultiplier = 2.5f;
    public float minimalFallMultiplier = 2f;
    public float jumpCutMultiplier = 0.5f;

    [Header("Ground Check")]
    public bool isGround;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Mejoras de salto")]
    [SerializeField] private float coyoteTime = 0.12f;
    [SerializeField] private float jumpBufferTime = 0.12f;

    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    [Header("Dash")]
    public float dashPower = 14f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.6f;
    public bool isDashing;
    
    [Header("Zona de ataque")]
    [SerializeField] private Transform attackZone;
    [SerializeField] private float verticalAttackThreshold = 0.5f;

    [SerializeField] private Vector2 sideAttackPosition = new Vector2(0.8f, 0f);
    [SerializeField] private Vector2 upAttackPosition = new Vector2(0f, 0.9f);

    [SerializeField] private Vector2 sideAttackSize = new Vector2(0.9f, 0.6f);
    [SerializeField] private Vector2 upAttackSize = new Vector2(0.8f, 0.8f);
    
    [Header("Animaciones")]
    [SerializeField] private string movementParameterName = "Movement";
    private int movementHash;
    
    private BoxCollider2D attackZoneCollider;

    public Vector2 attackDirection { get; private set; } = Vector2.right;

    private bool canDash = true;
    private float originalGravityScale;
    private Vector3 originalScale;
    
    private Vector2 savedVelocity;
    private float savedAngularVelocity;
    private RigidbodyType2D savedBodyType;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        movementHash = Animator.StringToHash(movementParameterName);
        
        originalGravityScale = rb.gravityScale;
        originalScale = transform.localScale;
        
        if (attackZone != null)
        {
            attackZoneCollider = attackZone.GetComponent<BoxCollider2D>();
        }
    }

    private void Start()
    {
        isDashing = false;
        originalScale = transform.localScale;

        gm = GameManager.instance;
        if (gm != null)
        {
            gm.onChangeGameState += OnChangeGameStateCallback;

            if (gm.gameState == GameState.Pause)
                isPaused = true;
        }

        inputManager = InputManager.GetInstance();
    }

    private void OnDestroy()
    {
        if (gm != null)
        {
            gm.onChangeGameState -= OnChangeGameStateCallback;
        }
    }
    
    public void OnChangeGameStateCallback(GameState newState)
    {
        isPaused = newState != GameState.Play;
        if (rb == null) return;

        if (isPaused)
        {
            savedVelocity = rb.linearVelocity;
            savedAngularVelocity = rb.angularVelocity;
            savedBodyType = rb.bodyType;

            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            // Esto evita que la gravedad lo siga moviendo.
            rb.bodyType = RigidbodyType2D.Kinematic;
            
        }
        else
        {
            rb.bodyType = savedBodyType;

            rb.linearVelocity = savedVelocity;
            rb.angularVelocity = savedAngularVelocity;
            
        }

        if (!PlayerHealth.GetInstance().isDead)
        {
            animator.speed = isPaused ? 0f : 1f;
        }
        
    }

    private void Update()
    {

        if (inputManager.IsButtonDown(BUTTONS.START) && !PlayerHealth.GetInstance().isDead)
        {
            gm.ChangeGameState(isPaused ? GameState.Play : GameState.Pause);
        }
        
        if (isPaused) return;
        if (inputManager == null) return;

        // Inputs
        horizontal = inputManager.GetAXis(AXIS.HORIZONTAL);
        vertical = inputManager.GetAXis(AXIS.VERTICAL);
        
        // Animaciones
        UpdateAnimations();

        // Direcciones
        HandleFacingDirection();
        HandleAttackZoneDirection();

        // Manejo del salto
        jumpHeld = inputManager.IsButton(BUTTONS.SPACE);

        if (inputManager.IsButtonDown(BUTTONS.SPACE))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (inputManager.IsButtonUp(BUTTONS.SPACE))
        {
            jumpReleased = true;
        }

        // Dash
        if (inputManager.IsButtonDown(BUTTONS.SHIFT) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private void FixedUpdate()
    {
        if (isPaused) return;
        if (inputManager == null) return;

        CheckGrounded();

        if (isDashing)
        {
            return;
        }

        Move();
        HandleJump();
        CalculateJumpFall();

        jumpReleased = false;
    }

    #region  Movimiento

    private void Move()
    {
        float targetSpeed = horizontal * speed;

        float accelRate;

        if (Mathf.Abs(horizontal) > 0.01f)
        {
            accelRate = isGround ? acceleration : airAcceleration;
        }
        else
        {
            accelRate = isGround ? deceleration : airDeceleration;
        }

        float newXVelocity = Mathf.MoveTowards(
            rb.linearVelocity.x,
            targetSpeed,
            accelRate * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector2(newXVelocity, rb.linearVelocity.y);
    }
    
    #endregion

    #region Salto
    
    private void HandleJump()
    {
        if (isGround)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.fixedDeltaTime;
        }

        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            Jump();
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }

        if (jumpReleased && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                rb.linearVelocity.y * jumpCutMultiplier
            );
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
    }

    private void CalculateJumpFall()
    {
        // Cuando va cayendo
        if (rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity += Vector2.up * 
                                 (Physics2D.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime);
        }
        // Cuando va subiendo y todavía mantiene el botón
        else if (rb.linearVelocity.y > 0f && jumpHeld)
        {
            rb.linearVelocity += Vector2.up * 
                                 (Physics2D.gravity.y * (riseMultiplier - 1f) * Time.fixedDeltaTime);
        }
        // Cuando va subiendo pero ya soltó el botón
        else if (rb.linearVelocity.y > 0f && !jumpHeld)
        {
            rb.linearVelocity += Vector2.up * 
                                 (Physics2D.gravity.y * (minimalFallMultiplier - 1f) * Time.fixedDeltaTime);
        }
    }
    
    #endregion

    #region Dash

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float previousGravityScale = rb.gravityScale;
        rb.gravityScale = 0f;

        rb.linearVelocity = new Vector2(facingDirection * dashPower, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = previousGravityScale;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }
    
    #endregion

    #region Direcciones

    private void HandleFacingDirection()
    {
        if (horizontal > 0.01f)
        {
            SetFacingDirection(1f);
        }
        else if (horizontal < -0.01f)
        {
            SetFacingDirection(-1f);
        }
    }

    private void SetFacingDirection(float direction)
    {
        if (facingDirection == direction) return;

        facingDirection = direction;

        Vector3 newScale = transform.localScale;
        newScale.x = Mathf.Abs(originalScale.x) * facingDirection;
        transform.localScale = newScale;
    }

    private void HandleAttackZoneDirection()
    {
        if (attackZone == null) return;

        if (vertical > verticalAttackThreshold)
        {
            // Ataque hacia arriba
            attackDirection = Vector2.up;

            attackZone.localPosition = new Vector3(
                upAttackPosition.x,
                upAttackPosition.y,
                0f
            );

            if (attackZoneCollider != null)
            {
                attackZoneCollider.size = upAttackSize;
            }
        }
        else
        {
            // Ataque hacia el lado al que mira el jugador
            attackDirection = new Vector2(facingDirection, 0f);

            attackZone.localPosition = new Vector3(
                Mathf.Abs(sideAttackPosition.x),
                sideAttackPosition.y,
                0f
            );

            if (attackZoneCollider != null)
            {
                attackZoneCollider.size = sideAttackSize;
            }
        }
    }

    #endregion
    
    private void CheckGrounded()
    {
        if (groundCheck == null)
        {
            isGround = false;
            return;
        }

        isGround = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    // Animaciones
    private void UpdateAnimations()
    {
        if (animator == null) return;

        float movementValue = Mathf.Abs(rb.linearVelocity.x);

        animator.SetFloat(movementHash, movementValue);
    }
    
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}