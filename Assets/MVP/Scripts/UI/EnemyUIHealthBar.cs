using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using BT;
public class EnemyUIHealthBar : HealthBar
{
    // public GameObject healthBarPrefab;
    public Vector3 offset;
    public Transform enemyTarget, UITarget, viewPoint;
    Image healthBarBG;
    public List<Image> healthBarStuff = new List<Image>();

    void Start()
    {
        healthBarBG = transform.parent.GetComponent<Image>();
        healthBarContainer = transform.parent.gameObject;
        healthBarDisplay = GetComponent<Image>();

        healthBarStuff.Add(healthBarBG);
        healthBarStuff.Add(healthBarDisplay);

        UITarget = Camera.main.transform.parent;
    }

    //private void Awake()
    //{
    //    enemyTarget = gameObject.transform;
    //}
    void HealthBarSwitch(bool _switch)
    {
        for (int i = 0; i < healthBarStuff.Count; i++)
        {
            healthBarStuff[i].enabled = _switch;
        }
    }
    void Update()
    {
        Debug.Log(1);
        healthBarContainer.transform.position = Camera.main.WorldToScreenPoint(enemyTarget.position + offset);
        if (enemyTarget == null)
        {
            print("My target died! I will destroy myself now");
            Destroy(gameObject);
        }

        if (enemyTarget)
        {
            if (enemyTarget.GetComponent<Renderer>().IsVisibleFrom(Camera.main))
                {
                Debug.Log("isVisibleFrom");
                if (BaneRays.ViewNotObstructed(viewPoint, UITarget, false))
                    {
                    Debug.Log("Bane");
                        healthBarContainer.SetActive(true);
                    HealthBarSwitch(true);
                    }
                }
            else if (!enemyTarget.GetComponent<Renderer>().IsVisibleFrom(Camera.main) || !BaneRays.ViewNotObstructed(viewPoint, UITarget, true))
            {
                //healthBarContainer.SetActive(false);
                HealthBarSwitch(false);
            }
        }
        //healthBarContainer.SetActive(GetComponent<Renderer>().IsVisibleFrom(Camera.main) ? true : false);
    }

    //void OnDestroy()
    //{
    //  Destroy(healthBarContainer);
    //}
}
