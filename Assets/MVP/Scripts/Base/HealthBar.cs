using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [HideInInspector] public GameObject healthBarContainer;
    [HideInInspector] public Image healthBarDisplay;

    //[HideInInspector] public Health health;

    //public virtual void UpdateBar()
    //{
    //  if (healthBarDisplay)
    //  {
    //    healthBarDisplay.fillAmount = health.currentHealth / (float)health.maxHealth;

    //  }
    //}
    public virtual void UpdateBar(float _currentHealth, float _maxHealth)
    {
        if (healthBarDisplay)
        {
            healthBarDisplay.fillAmount = _currentHealth / _maxHealth;
        }
    }
}
