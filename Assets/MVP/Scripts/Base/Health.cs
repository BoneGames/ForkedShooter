﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using NaughtyAttributes;

public abstract class Health : MonoBehaviour
{
  public bool ShowHealth;

  [ShowIf("ShowHealth"), BoxGroup("Health")]
  public float maxHealth = 100, currentHealth;

  public bool ShowShields;
  [ShowIf("ShowShields"), BoxGroup("Shields")]
  public ShieldController shield;
  [ShowIf("ShowShields"), BoxGroup("Shields")]
  public float maxShield = 100, currentShield, carryOnDmg;
  [ShowIf("ShowShields"), BoxGroup("Shields")]
  public Elements.Element shieldElement;


  //[HideInInspector]
  public HealthBar healthBar;

  public bool ShowEvents;
  [ShowIf("ShowEvents"), BoxGroup("Events")]
  public UnityEvent onDeath, onHeal, onDamage;

  public virtual void Start()
  {
    currentHealth = maxHealth;
    currentShield = maxShield;

    SetShield();
  }

  // Takes damage from various bullet/projectile scripts and runs 'CheckDie()'.
  [PunRPC]
  public virtual void ChangeHealth(float value, Vector3 shotDir, Elements.Element ammoType)
  {
    Debug.Log(1);
    //if (ammoType.ToString() == shieldElement.ToString())
    //{
    //    value += 5;
    //}

    //else if (ammoType.ToString() != shieldElement.ToString())
    //{
    //    return;
    //}

    value = CheckWeakness(value, ammoType);

    currentHealth -= value;

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
    CheckDie();
  }
  //public virtual void ChangeHealth(float value, Vector3 shotDir)
  //{
  //    Debug.Log(2);
  //    if (value != 0)
  //    {
  //        currentHealth -= value;

  //        if (value < 0)
  //        {
  //            onHeal.Invoke();
  //        }
  //        if (value > 0)
  //        {
  //            onDamage.Invoke();
  //        }
  //    }
  //    CheckDie();
  //}

  //Self explanatory.
  public virtual void CheckDie()
  {
    if (currentHealth <= 0)
    {
      onDeath.Invoke();
    }
  }

  public float CheckWeakness(float _val, Elements.Element ammoType)
  {
    print("I am checking shield weakness!");
    if (ammoType.ToString() == "Fire" && shieldElement.ToString() == "Grass")
    {
      _val = _val * 1.25f;
    }

    else if (ammoType.ToString() == "Water" && shieldElement.ToString() == "Fire")
    {
      _val = _val * 1.25f;
    }

    else if (ammoType.ToString() == "Grass" && shieldElement.ToString() == "Water")
    {
      _val = _val * 1.25f;
    }
    print(_val);

    return _val;
  }

  public void SetShield()
  {
        if(shield)
    shield.SetShieldElement(shieldElement);
  }

  public enum Element
  {
    Normal,
    Fire,
    Water,
    Grass
  }
}
