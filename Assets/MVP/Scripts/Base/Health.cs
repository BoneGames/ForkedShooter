using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using NaughtyAttributes;

public abstract class Health : MonoBehaviour
{
  public bool ShowHealth;

  [ShowIf("ShowHealth")] [BoxGroup("Health")]
  public int maxHealth = 100, currentHealth;

  public bool ShowShields;
  [ShowIf("ShowShields")] [BoxGroup("Shields")]
  public int maxShield = 100, currentShield, carryOnDmg;

  [HideInInspector]
  public HealthBar healthBar;

  public bool ShowEvents;
  [ShowIf("ShowEvents")] [BoxGroup("Events")]
  public UnityEvent onDeath, onHeal, onDamage;

  public virtual void Start()
  {
    currentHealth = maxHealth;
    currentShield = maxShield;
  }

  // Takes damage from various bullet/projectile scripts and runs 'CheckDie()'.
  [PunRPC]
  public virtual void ChangeHealth(int value, Vector3 shotDir)
  {
    if (value != 0)
    {
      if (value < 0)
      {
        onHeal.Invoke();
      }
      if (value > 0)
      {
        onDamage.Invoke();
      }
    }
    currentHealth -= value;
    CheckDie();
  }

  // Self explanatory.
  public virtual void CheckDie()
  {
    onDeath.Invoke();
  }
}
