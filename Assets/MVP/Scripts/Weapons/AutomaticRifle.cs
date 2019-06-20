using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class AutomaticRifle : Weapon
{
    public override void Attack()
    {
        base.Attack();
        //if (currentMag > 0)
        //{
        //    Ray ray = new Ray(shootPoint.position, shootPoint.transform.forward);

        //    SpawnMuzzleFlash();
            
        //    Debug.Log("Reset shoot Timer");
        //    attackTimer = 0;

        //    canShoot = false;

        //    shootPoint.rotation = AimAtCrosshair();



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
        //                //Debug.Log(hit.collider.name);
        //                if (hit.collider.GetComponent<AI_FoV_SearchLight>())
        //                {
        //                    Debug.Log("hit Drone light - it should be off now");
        //                    hit.collider.GetComponent<AI_FoV_SearchLight>().fovLight.enabled = false;
        //                    hit.collider.enabled = false;
        //                    hit.collider.GetComponent<AI_FoV_SearchLight>().viewRadius = 10;
        //                }
        //                hit.transform.GetComponent<Health>().ChangeHealth(damage, transform.position, weaponElement);
        //                print("I hit an enemy");
        //            }

        //        }
        //    }
        //    BulletTrail(shootPoint.transform.position + (shootPoint.transform.forward + AccuracyOffset(accuracy)) * 200, 200, weaponElement);
        //    RecoilMethod();
        //    currentMag--;
        //    UpdateAmmoDisplay();
        //}
        //if (currentMag <= 0)
        //{
        //    StartCoroutine(ReloadTimed());
        //}
    }

    public override void Reload()
    {
        StartCoroutine(ReloadTimed());
    }
}
