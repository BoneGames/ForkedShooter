using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystems;

public class Pistol : Weapon
{
    #region Old
    //public GameObject bullet;
    //public Transform spawnPoint;


    //void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        GameObject clone = Instantiate(bullet, spawnPoint.position, spawnPoint.rotation);
    //        Bullet newBullet = clone.GetComponent<Bullet>();

    //        newBullet.Fire(transform.forward);
    //    }
    //}
    #endregion
    public float spread;
    public int magSize;

    public int currentMag;
    public int tempMag;

    public override void Attack()
    {
        if (currentMag > 0)
        {
            GameObject clone = Instantiate(projectile, spawnPoint.position, spawnPoint.rotation);
            Bullet newBullet = clone.GetComponent<Bullet>();
            newBullet.damage = damage;

            newBullet.Fire(transform.forward);
            currentMag--;
        }
        if(currentMag <= 0 )
        {
            Reload();
        }
    }

    public override void Reload()
    {
        if (currentAmmo > 0)
        {
            if (currentAmmo >= magSize)
            {
                currentAmmo -= magSize - currentMag;

                currentMag = magSize;
            }
            if (currentAmmo < magSize)
            {
                tempMag = currentAmmo;
                currentMag = tempMag;
                currentAmmo -= tempMag;
            }
        }
    }

}
