using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    public bool canAttack1;
    public int CloseCombatDmg = 25;
    
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            print("ataca");
        }
    }
}
