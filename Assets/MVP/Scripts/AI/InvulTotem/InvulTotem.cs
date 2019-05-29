    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Safety measure.
[RequireComponent(typeof(SphereCollider))]
public class InvulTotem : Health
{
    #region Variables
    [Header("Master Control")]
    // Size of AoE (Area of Effect).
    public float radius = 10f;

    // [Header("References")]
    private LayerMask enemyMask;
    private SphereCollider col;
    private Transform drawAoE; // ← SET TO A CHILD OBJECT! DO NOT SET TO SCRIPT'S OWN TRANSFORM!
    #endregion

    #region Functions 'n' Methods

    // Where we grab and set our Component References and our AoE size.
    #region Start()
    // Start is called just before any of the Update methods is called the first time
    void Start()
    {
        enemyMask = LayerMask.GetMask("Enemy");

        col = gameObject.GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = radius;

        drawAoE = transform.Find("AoE").GetComponent<Transform>();
        drawAoE.localScale = new Vector3(radius * 2f, radius * 2f, radius * 2f);
    } 
    #endregion

    // Where we take damage and check if we can die.
    public override void ChangeHealth(int value, Vector3 shotDir)
    {
        currentHealth -= value;
        CheckDie();
        Debug.Log("Totem Hit. Remaining health: " + currentHealth);
    }

    // Where we go to die...
    public override void CheckDie()
    {
        if (currentHealth <= 0)
        {
            // Run DisableTotem() before we die so we don't get any perma-buffed super enemies!
            DisableTotem();
            Destroy(gameObject);
        }
    }

    // Where we give/take buffs to/from enemies when they enter/exit the AoE respectively.
    #region OnTriggers...
    // OnTriggerEnter is called when the Collider other enters the trigger
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            EnemyHealth god = other.GetComponent<EnemyHealth>();
            god.isGod = true;
            //print("isGod");
        }
    }

    // OnTriggerExit is called when the Collider other has stopped touching the trigger
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            EnemyHealth god = other.GetComponent<EnemyHealth>();
            god.isGod = false;
            //print("!isGod");
        }
    }
    #endregion

    // Where we disable buffs on any enemies inside our AoE before it's destroyed.
    #region DisableTotem()
    void DisableTotem()
    {
        // Grab every enemy inside our AoE.
        Collider[] enemyInAOE = Physics.OverlapSphere(transform.position, radius, enemyMask);

        // Disable the buff on each enemy.
        for (int i = 0; i < enemyInAOE.Length; i++)
        {
            EnemyHealth enemy = enemyInAOE[i].GetComponent<EnemyHealth>();
            enemy.isGod = false;
            //print("LOST isGod");
        }
    } 
    #endregion

    #endregion
}
