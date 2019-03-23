using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class InvulTotem : MonoBehaviour
{
    #region Variables
    public float radius = 10f;
    public SphereCollider col;
    public Transform drawAoE;
    #endregion

    #region Functions 'n' Methods
    // Start is called just before any of the Update methods is called the first time
    void Start()
    {
        col = gameObject.GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = radius;

        drawAoE = transform.Find("AoE").GetComponent<Transform>();
        drawAoE.localScale = new Vector3(radius * 2f, radius * 2f, radius * 2f);
    }

    // This function is called when the MonoBehaviour will be destroyed
    void OnDestroy()
    {
        EnemyHealth god = GetComponent<EnemyHealth>();
    }


    #region OnTriggers...
    // OnTriggerEnter is called when the Collider other enters the trigger
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            EnemyHealth god = other.GetComponent<EnemyHealth>();
            god.isGod = true;
            print("isGod");
        }
    }

    // OnTriggerExit is called when the Collider other has stopped touching the trigger
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            EnemyHealth god = other.GetComponent<EnemyHealth>();
            god.isGod = false;
            print("!isGod");
        }
    } 
    #endregion
    #endregion
}
