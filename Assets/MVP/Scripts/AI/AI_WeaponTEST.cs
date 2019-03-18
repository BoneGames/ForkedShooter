using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystems;

public class AI_WeaponTEST : Weapon
{
    public AI_ScoutDrone contact;

    void Update()
    {
        Attack();
    }

    public override void Attack()
    {
        if (contact.fov.visibleTargets.Count > 0)
        {
            StartCoroutine("Shoot");
            print("Firing.");
        }
        else
        {
            StopCoroutine("Shoot");
            print("Target lost.");
        }

    }

    IEnumerator Shoot()
    {
        while (true)
        {
            GameObject clone = Instantiate(projectile, spawnPoint.position, spawnPoint.rotation);
            Bullet newBullet = clone.GetComponent<Bullet>();

            newBullet.Fire(spawnPoint.transform.forward);
            yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
        }
    }
}
