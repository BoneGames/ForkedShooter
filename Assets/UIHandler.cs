using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class UIHandler : MonoBehaviour
{
    public bool ShowElementColors;
    [BoxGroup("Element Colors"), ShowIf("ShowElementColors")]
    public Color normal, fire, water, grass;

    [HideInInspector] public ShotDirection shotDirection;
    [HideInInspector] public DeathMessage deathMessage;
    [HideInInspector] public HealthBar healthBar;

    public GameObject healthBarPrefab;

    public void SpawnEnemyHealthBars(Transform _enemy, Transform _viewPoint)
    {
        // spawn health bar
        GameObject healthBarContainer = Instantiate(healthBarPrefab);
        // set parent
        healthBarContainer.transform.SetParent(GameObject.Find("EnemyHealthBars").transform, false);

        // name nicely in hierarchy
        healthBarContainer.name = this.gameObject.name + " " + healthBarContainer.name;
        if (healthBarContainer.name.Contains("(Clone)"))
        {
            healthBarContainer.name = healthBarContainer.name.Replace("(Clone)", "");
        }

        EnemyUIHealthBar script = healthBarContainer.GetComponentInChildren<EnemyUIHealthBar>();
        // set owner co-ordinate
        script.enemyTarget = _enemy;
        // set look point for UI visibility
        script.viewPoint = _viewPoint;
        //healthBarDisplay = healthBarContainer.transform.GetChild(0).GetComponent<Image>();
    }

    public virtual void Awake()
    {
        shotDirection = GetComponentInChildren<ShotDirection>();
        deathMessage = GetComponentInChildren<DeathMessage>();
        healthBar = GetComponentInChildren<HealthBar>();
    }
}
