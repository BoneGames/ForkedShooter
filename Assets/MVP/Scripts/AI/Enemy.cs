using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Health
{
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
            Destroy(gameObject);
        }
    }
}
