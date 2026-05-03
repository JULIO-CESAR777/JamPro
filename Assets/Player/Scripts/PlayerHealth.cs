using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private static PlayerHealth instance;
    public static PlayerHealth GetInstance() => instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    
    private int maxHealth = 100;
    private GameManager gm;
    private bool isPaused = false;
    private Animator animator;
    public bool win;
    
    [Header("Vida")]
    public float health;
    public float damagePerSecond = 6.66f;
    public bool isDead;
    [SerializeField] private string dieParameterName = "isDead";
    [SerializeField] private string damageParameterName = "isDamage";
    
    private int isDeadHash;
    private int isDamageHash;
    
    [Header("Visuales")]
    [SerializeField] Slider slider;
    
    
    private void Start()
    {
        health = maxHealth;
        isDead = false;
        win = false;
        gm = GameManager.GetInstance();
        
        if (gm != null)
        {
            gm.onChangeGameState += OnChangeGameStateCallback;

            if (gm.gameState == GameState.Pause)
                isPaused = true;
        }
        animator = GetComponent<Animator>();
        isDeadHash = Animator.StringToHash(dieParameterName);
        isDamageHash = Animator.StringToHash(damageParameterName);
    }

    public void OnChangeGameStateCallback(GameState newState)
    {
        isPaused = newState != GameState.Play;
    }

    private void Update()
    {
        if (isPaused) return;
        
        health -= damagePerSecond * Time.deltaTime;
        slider.value = health;
        
        if (health <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(float damage, bool fromEnemy)
    {
        health -= damage;
        if (fromEnemy)
        {
            animator.SetTrigger(isDamageHash);
        }
    }

    public void Heal(float heal)
    {
        health += heal;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public void Die()
    {
        isDead = true;
        gm.ChangeGameState(GameState.Pause);
        animator.SetTrigger(isDeadHash);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Win"))
        {
            win = true;
            gm.ChangeGameState(isPaused ? GameState.Play : GameState.Pause);
        }
    }
}
