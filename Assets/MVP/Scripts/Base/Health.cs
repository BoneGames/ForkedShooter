using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    [HideInInspector]
    public HealthBar healthBar;


    private void Start()
    {
        currentHealth = maxHealth;
    }

    // Takes damage from various bullet/projectile scripts and runs 'CheckDie()'.
    [PunRPC]
    public virtual void ChangeHealth(int value)
    {
        currentHealth -= value;
        CheckDie();
    }

    // Self explanatory.
    public virtual void CheckDie()
    {
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
