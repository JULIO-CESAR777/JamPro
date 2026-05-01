using System;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Ataque cuerpo a cuerpo")]
    [SerializeField] GameObject dmgZone;
    public bool canAttack1;
    public int CloseCombatDmg = 25;
    public float attack1Cooldown = .4f;
    public float attack1Counter;

    private void Start()
    {
        attack1Counter = 0f;
    }

    void Update()
    {
        
        
        if (Input.GetKey(KeyCode.Mouse0))
        {
            dmgZone.SetActive(true);
        }
    }
}
