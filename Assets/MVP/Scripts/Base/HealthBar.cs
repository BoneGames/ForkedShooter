using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [HideInInspector] public GameObject healthBarContainer;
    [HideInInspector] public Image healthBarDisplay;

    [HideInInspector] public Health health;

    public virtual void UpdateBar()
    {
        if (healthBarDisplay)
        {
            healthBarDisplay.fillAmount = health.currentHealth / (float)health.maxHealth;
        }
    }
}
