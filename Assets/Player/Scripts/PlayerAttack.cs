using System;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Ataque cuerpo a cuerpo")]
    [SerializeField] Collider2D dmgZone;
    public bool canAttack1;
    
    public int CloseCombatDmg = 25;
    
    public float attack1Cooldown = .4f;
    public float attack1Counter;
    public float attack1Duration = 0.3f;

    private void Start()
    {
        attack1Counter = 0f;
        dmgZone.enabled = false;
        canAttack1 = true;
    }

    void Update()
    {
        if (!canAttack1)
        {
            attack1Counter += Time.deltaTime;
            if (attack1Counter >= attack1Cooldown)
            {
                attack1Counter = 0f;
                canAttack1 = true;
            }
        }


        if (Input.GetKey(KeyCode.Mouse0) && canAttack1)
        {
            dmgZone.enabled = true;

            
        }
    }
}
