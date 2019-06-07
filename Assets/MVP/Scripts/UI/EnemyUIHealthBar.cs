using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using BT;
public class EnemyUIHealthBar : HealthBar
{
    public Vector3 offset;
    public Transform enemyTarget, UITarget, viewPoint;
    Image[] healthBars = new Image[2];
    Renderer rend;

    void Start()
    {
        // Get Health Bar Images
        healthBarDisplay = GetComponent<Image>();
        healthBars = transform.parent.GetComponentsInChildren<Image>();

        healthBarContainer = transform.parent.gameObject;

        // For rendering Health bars only when seen
        UITarget = Camera.main.transform.parent;
        rend = enemyTarget.GetComponent<Renderer>();

        // Register this Bar as the Enemies script to send Damage info to
        enemyTarget.GetComponent<EnemyHealth>().RegisterHealthBarEventDelegate(this);
    }

    public void UpdateHealthBar(float _currentHealth, float _maxHealth)
    {
        healthBarDisplay.fillAmount = _currentHealth / _maxHealth;
    }

    void HealthBarSwitch(bool _switch)
    {
        for (int i = 0; i < healthBars.Length; i++)
        {
            healthBars[i].enabled = _switch;
        }
    }
    void Update()
    {
        // Destroy HealthBar is Enemy Dies
        if (enemyTarget == null)
        {
            print("My target died! I will destroy myself now");
            Destroy(healthBarContainer);
        }
        // Update HealthBar Position
        healthBarContainer.transform.position = Camera.main.WorldToScreenPoint(enemyTarget.position + offset);
        
        // Update HealthBar Canvas Rendering
        if (enemyTarget)
        {
            if (rend.IsVisibleFrom(Camera.main) && BaneRays.ViewNotObstructed(viewPoint, UITarget, false))
            {
                HealthBarSwitch(true);
            }
            else if (!rend.IsVisibleFrom(Camera.main) || !BaneRays.ViewNotObstructed(viewPoint, UITarget, false))
            {
                HealthBarSwitch(false);
            }
        }
    }
}
