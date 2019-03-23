using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    // Set damage immunity on/off (handled from InvulTotem.cs).
    public bool isGod = false;

    // Takes damage from various bullet/projectile scripts and runs 'CheckDie()'.
    public override void ChangeHealth(int value)
    {
        if (!isGod)
        {
            currentHealth -= value;
            CheckDie();
        }
    }

    // Self explanatory.
    public override void CheckDie()
    {
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
