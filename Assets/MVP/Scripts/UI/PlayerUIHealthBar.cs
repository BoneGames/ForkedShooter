using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHealthBar : HealthBar
{
    public GameObject healthBarBG;

    // Use this for initialization
    void Start()
    {
        healthBarDisplay = healthBarBG.transform.GetChild(0).GetComponent<Image>();
        health = GetComponent<Health>();
        health.healthBar = this;
    }

    public override void UpdateBar()
    {
        healthBarDisplay.fillAmount = health.currentHealth / (float)health.maxHealth;
    }
}
