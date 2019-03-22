using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystems;

public class AI_WeaponTEST : Weapon
{
    #region Variable
    // Check in AI_ScoutDrone.cs for visibleTargets.
    public AI_ScoutDrone contact;
    #endregion

    #region Functions 'n' Methods
    // Where we initialize / Start things.
    void Start()
    {
        // Grab 'contact's component, and start Coroutine.
        contact = gameObject.GetComponent<AI_ScoutDrone>();
        StartCoroutine("Shoot");
    }

    // Where we define shooting.
    public override void Attack()
    {
        // If we've found a target...
        if (contact.fov.visibleTargets.Count > 0)
        {
            // Fire bullets at it.
            GameObject clone = Instantiate(projectile, spawnPoint.position, spawnPoint.rotation);
            Bullet newBullet = clone.GetComponent<Bullet>();

            newBullet.Fire(spawnPoint.transform.forward);
            newBullet.sourceAgent = this.gameObject;
            print("Firing.");
        }
        // Otherwise stop firing.
        else
        {
            print("Target lost.");
        }

    }

    // Where we run Attack.
    IEnumerator Shoot()
    {
        // While the Coroutine is running...
        while (true)
        {
            Attack();
            yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
        }
    } 
    #endregion
}
