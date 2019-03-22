using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public int health = 100;

    public virtual void ChangeHealth(int value)
    {
        health -= value;
        CheckDie();
    }

    public virtual void CheckDie()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
