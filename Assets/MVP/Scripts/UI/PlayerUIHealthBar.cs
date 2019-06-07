using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerUIHealthBar : HealthBar
{
    public Image playerHealthBar;

    void Start()
    {
        playerHealthBar = GetComponent<Image>();
    }

    public override void UpdateBar(float _currentHealth, float _maxHealth)
    {
        playerHealthBar.fillAmount = _currentHealth / _maxHealth;
    }
}
