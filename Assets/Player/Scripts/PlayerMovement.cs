using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    // Managers
    private GameManager gm;
    private InputManager inputManager;
    private Rigidbody2D rb;

    [SerializeField] private bool isPaused = false;

    private float horizontal;
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

    private bool canDash = true;
    private float originalGravityScale;
    private Vector3 originalScale;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravityScale = rb.gravityScale;
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

        if (isPaused)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void Update()
    {
        if (isPaused) return;
        if (inputManager == null) return;

        horizontal = inputManager.GetAXis(AXIS.HORIZONTAL);
        print("horizontal: " + horizontal);

        if (horizontal > 0.01f)
        {
            facingDirection = 1f;
        }
        else if (horizontal < -0.01f)
        {
            facingDirection = -1f;
        }
        transform.localScale = new Vector3(facingDirection, 1f, 1f);

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
        if (rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity += Vector2.up * (Physics2D.gravity.y *
                                 (fallMultiplier - 1f) * Time.fixedDeltaTime);
        }
        else if (rb.linearVelocity.y > 0f && !jumpHeld)
        {
            rb.linearVelocity += Vector2.up * (Physics2D.gravity.y *
                                 (minimalFallMultiplier - 1f) * Time.fixedDeltaTime);
        }
    }

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

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}