using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using NaughtyAttributes;

public abstract class Health : MonoBehaviour
{
  public bool ShowHealth;

  [ShowIf("ShowHealth")]
  [BoxGroup("Health")]
  public float maxHealth = 100, currentHealth;

  public bool ShowShields;
  [ShowIf("ShowShields")]
  [BoxGroup("Shields")]
  public float maxShield = 100, currentShield, carryOnDmg;
  public Elements.Element shieldElement;

  [HideInInspector]
  public HealthBar healthBar;

  public bool ShowEvents;
  [ShowIf("ShowEvents")]
  [BoxGroup("Events")]
  public UnityEvent onDeath, onHeal, onDamage;

  public virtual void Start()
  {
    currentHealth = maxHealth;
    currentShield = maxShield;
  }

  // Takes damage from various bullet/projectile scripts and runs 'CheckDie()'.
  [PunRPC]
  public virtual void ChangeHealth(float value, Vector3 shotDir, Elements.Element ammoType)
  {
    //if (ammoType.ToString() == shieldElement.ToString())
    //{
    //    value += 5;
    //}

    //else if (ammoType.ToString() != shieldElement.ToString())
    //{
    //    return;
    //}

    if (ammoType.ToString() == "Fire" && shieldElement.ToString() == "Metal")
    {
      value = Mathf.FloorToInt(value * 1.25f);
    }

    else if (ammoType.ToString() == "Water" && shieldElement.ToString() == "Fire")
    {
      value = Mathf.FloorToInt(value * 1.25f);
    }

    else if (ammoType.ToString() == "Earth" && shieldElement.ToString() == "Water")
    {
      value = Mathf.FloorToInt(value * 1.25f);
    }

    else if (ammoType.ToString() == "Air" && shieldElement.ToString() == "Earth")
    {
      value = Mathf.FloorToInt(value * 1.25f);
    }

    else if (ammoType.ToString() == "Metal" && shieldElement.ToString() == "Air")
    {
      value = Mathf.FloorToInt(value * 1.25f);
    }

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
  public virtual void ChangeHealth(float value, Vector3 shotDir)
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

  public enum Element
  {
    Normal,
    Fire,
    Water,
    Earth,
    Air,
    Metal
  }
}
