﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Sniper : Weapon
{
    public override void Attack()
    {
        base.Attack();
        //if (currentMag > 0)
        //{
        //    // reset shot rate controller
        //    attackTimer = 0;
        //    canShoot = false;
        //    // start aim from centre of crosshair (not directly forward due to holding offset)
        //    shootPoint.transform.rotation = AimAtCrosshair();
        //    // muzzle flash & sound FX
        //    onFire.Invoke();
        //    // raycast with (in)accuracy
        //    Ray ray = new Ray(shootPoint.position, shootPoint.transform.forward + AccuracyOffset(accuracy));
        //    if (Physics.Raycast(ray, out RaycastHit hit))
        //    {
        //        BulletTrail(hit.point, hit.distance, weaponElement);

        //        if (GameManager.isOnline)
        //        {
        //            if (hit.collider.CompareTag("Enemy"))
        //            {
        //                hit.transform.GetComponent<PhotonView>().RPC("ChangeHealth", PhotonTargets.All, damage);
        //            }
        //        }
        //        else
        //        {
        //            if (hit.collider.tag == "Enemy")
        //            {
        //                // disable drone light on impact (also reduces drone look length)
        //                if (hit.collider.GetComponent<AI_FoV_SearchLight>())
        //                {
        //                    Debug.Log("hit Drone light - it should be off now");
        //                    hit.collider.GetComponent<AI_FoV_SearchLight>().fovLight.enabled = false;
        //                    hit.collider.enabled = false;
        //                    hit.collider.GetComponent<AI_FoV_SearchLight>().viewRadius = 10;
        //                }
        //                // Deal Damage
        //                hit.transform.GetComponent<Health>().ChangeHealth(damage, transform.position, weaponElement);
        //                print("I hit an enemy");
        //            }

        //        }
        //    }
        //    else
        //    {
        //        // bullet trail into sky
        //        BulletTrail(shootPoint.transform.position + (shootPoint.transform.forward + AccuracyOffset(accuracy)) * 200, 200, weaponElement);
        //    }
        //    RecoilMethod();
        //    // Reduce ammo
        //    currentMag--;
        //    UpdateAmmoDisplay();
        //}
        //if (currentMag <= 0 && autoReload)
        //{
        //    StartCoroutine(ReloadTimed());
        //}
    }

    public override void Reload()
    {
        StartCoroutine(ReloadTimed());
    }
}