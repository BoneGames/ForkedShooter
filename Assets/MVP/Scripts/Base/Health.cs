using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Health : MonoBehaviour
{
  public int maxHealth = 100;
  public int currentHealth;

  public int maxShield = 100;
  public int currentShield;
  public int carryOnDmg;

  [HideInInspector]
  public HealthBar healthBar;

  public UnityEvent onDeath;

  public virtual void Start()
  {
    currentHealth = maxHealth;
    currentShield = maxShield;
  }

  // Takes damage from various bullet/projectile scripts and runs 'CheckDie()'.
  [PunRPC]
  public virtual void ChangeHealth(int value, Vector3 shotDir)
  {
    currentHealth -= value;
    CheckDie();
  }

  // Self explanatory.
  public virtual void CheckDie()
  {
    onDeath.Invoke();
  }
}
