using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using NaughtyAttributes;

[System.Serializable]
public class Event2Floats : UnityEvent<float, float> { }
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

    public UIHandler UI;


    //[HideInInspector]
    public HealthBar healthBar;

    public bool ShowEvents;
    [ShowIf("ShowEvents"), BoxGroup("Events")]
    public UnityEvent onDeath, onHeal, onDamage;
    public Event2Floats updateHealthBar;

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
                updateHealthBar.Invoke(currentHealth, maxHealth);
            }
            if (value > 0)
            {
                onDamage.Invoke();
                updateHealthBar.Invoke(currentHealth, maxHealth);
            }
        }
        CheckDie();
    }
   

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
        //print(_val);

        return _val;
    }

    public void SetShield()
    {
        if (shield)
        {
            shield.SetShieldElement(shieldElement);

            if (currentShield <= 0)
            {
                currentShield = 0;
                shield.gameObject.SetActive(false);
            }
        }
    }

    public enum Element
    {
        Normal, //333333
        Fire, //542C00
        Water, //00676A
        Grass //032B00
    }
}
