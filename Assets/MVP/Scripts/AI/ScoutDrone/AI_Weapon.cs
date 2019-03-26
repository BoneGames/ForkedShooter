using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystems;

public class AI_Weapon : Weapon
{
    #region Variable
    // Check in AI_ScoutDrone.cs for visibleTargets.
    public AI_ScoutDrone contact;
    public float reloadTime;
    #endregion

    #region Functions 'n' Methods
    // Where we initialize / Start things.
    void Start()
    {
        // Grab 'contact's component, give full ammo, and start Coroutine.
        contact = gameObject.GetComponent<AI_ScoutDrone>();
        currentAmmo = ammo;
        StartCoroutine("Shoot");
    }

    // Where we define shooting.
    public override void Attack()
    {
        if (contact.fov.visibleTargets.Count > 0 && currentAmmo != 0)    
        {
            // Fire bullets at it.
            GameObject clone = Instantiate(projectile, spawnPoint.position, spawnPoint.rotation);
            Bullet newBullet = clone.GetComponent<Bullet>();

            newBullet.Fire(spawnPoint.transform.forward);
            newBullet.sourceAgent = this.gameObject;
            //print("Firing.");
            currentAmmo--;
            Debug.Log(currentAmmo);
        }

        // If we run out of ammo, start reloading and stop shooting.
        if (currentAmmo == 0)
        {
            StartCoroutine("StartReload", reloadTime);
            StopCoroutine("Shoot");
        }
    }

    // Where we run Attack() multiple times.
    IEnumerator BurstFire()
    {
        while (true)
        {
            Attack();
            yield return new WaitForSeconds(0.1f);
            Attack();
            yield return new WaitForSeconds(0.1f);
            Attack();
            yield return new WaitForSeconds(0.1f);
            StopCoroutine("BurstFire");
        }
    }

    // Where we run BurstFire().
    IEnumerator Shoot()
    {
        // While the Coroutine is running...
        while (true)
        {
            StartCoroutine("BurstFire");
            yield return new WaitForSeconds(Random.Range(0.5f, 1f));
        }
    }
    // Where we run Reload().
    IEnumerator StartReload(float reloadTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(reloadTime);
            Reload();
            StartCoroutine("Shoot");
            StopCoroutine("StartReload");
        }
    }
    #endregion
}
