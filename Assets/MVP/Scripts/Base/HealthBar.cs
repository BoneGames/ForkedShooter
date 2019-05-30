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
    print("Health Bar Update Requested");
    if (healthBarDisplay)
    {
      print(string.Format("Health Bar Update Successful, current health is {0}, out of {1}", health.currentHealth, health.maxHealth));
      healthBarDisplay.fillAmount = health.currentHealth / (float)health.maxHealth;
    }
  }
}
