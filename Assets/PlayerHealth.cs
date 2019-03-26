using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    private void Update()
    {
        CheckDie();
    }

    // Takes damage from various bullet/projectile scripts and runs 'CheckDie()'.
    public override void ChangeHealth(int value)
    {
        currentHealth -= value;
        CheckDie();
    }

    // Self explanatory.
    public override void CheckDie()
    {
        if (currentHealth <= 0)
        {
            this.gameObject.GetComponent<RigidCharacterMovement>().Respawn();
        }
    }
}
