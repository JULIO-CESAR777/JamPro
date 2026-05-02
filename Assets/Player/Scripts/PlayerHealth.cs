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
    public bool isDead;
    
    [Header("Vida")]
    public float health;
    public float damagePerSecond = 6.66f;
    
    [Header("Visuales")]
    [SerializeField] Slider slider;

    private void Start()
    {
        health = maxHealth;
        isDead = false;
        gm = GameManager.GetInstance();
        
        if (gm != null)
        {
            gm.onChangeGameState += OnChangeGameStateCallback;

            if (gm.gameState == GameState.Pause)
                isPaused = true;
        }
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

    public void TakeDamage(float damage)
    {
        health -= damage;
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
        gm.ChangeGameState(GameState.Pause);
        isDead = true;
    }
}
