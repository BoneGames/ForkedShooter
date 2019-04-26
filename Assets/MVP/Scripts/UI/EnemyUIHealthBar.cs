using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUIHealthBar : HealthBar
{
    public GameObject healthBarPrefab;
    public Vector3 offset;

    Transform target;

    // Use this for initialization
    void Start()
    {
        healthBarContainer = Instantiate(healthBarPrefab);
        healthBarContainer.transform.SetParent(GameObject.Find("Canvas").transform, false);

        healthBarDisplay = healthBarContainer.transform.GetChild(0).GetComponent<Image>();
        health = target.GetComponent<Health>();
        health.healthBar = this;
    }

    private void Awake()
    {
        target = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        healthBarContainer.transform.position = Camera.main.WorldToScreenPoint(target.position + offset);
        if (target == null)
        {
            print("My target died!");
        }

        healthBarContainer.SetActive(GetComponent<Renderer>().IsVisibleFrom(Camera.main) ? true : false);
    }

    void OnDestroy()
    {
        Destroy(healthBarContainer);
    }
}
