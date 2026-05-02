using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Configuración")]
    public float speed = 12f;

    private Vector2 direction = Vector2.right;

    private GameManager gm;
    private bool isPaused;

    private Vector2 savedVelocity;
    private float savedAngularVelocity;
    private RigidbodyType2D savedBodyType;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        gm = GameManager.GetInstance();

        if (gm != null)
        {
            gm.onChangeGameState += OnChangeGameStateCallback;
            OnChangeGameStateCallback(gm.gameState);
        }
    }

    private void OnDestroy()
    {
        if (gm != null)
        {
            gm.onChangeGameState -= OnChangeGameStateCallback;
        }
    }

    public void SetDirection(Vector2 newDirection)
    {
        if (newDirection.sqrMagnitude <= 0.01f)
        {
            newDirection = Vector2.right;
        }

        direction = newDirection.normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        if (!isPaused)
        {
            rb.linearVelocity = direction * speed;
        }
    }

    private void FixedUpdate()
    {
        if (isPaused) return;

        rb.linearVelocity = direction * speed;
    }

    private void OnChangeGameStateCallback(GameState newState)
    {
        bool shouldPause = newState != GameState.Play;

        if (shouldPause == isPaused) return;

        isPaused = shouldPause;

        if (isPaused)
        {
            savedVelocity = rb.linearVelocity;
            savedAngularVelocity = rb.angularVelocity;
            savedBodyType = rb.bodyType;

            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        else
        {
            rb.bodyType = savedBodyType;

            rb.linearVelocity = savedVelocity;
            rb.angularVelocity = savedAngularVelocity;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isPaused) return;

        if (other.CompareTag("Enemy") || other.CompareTag("End"))
        {
            PlayerHealth.GetInstance().Heal(15);
            Destroy(gameObject);
        }
    }
}