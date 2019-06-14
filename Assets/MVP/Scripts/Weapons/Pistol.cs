using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Reflection;
using BT;
public class Pistol : Weapon
{

    public AmmoType.AmmoTypes ammotype;

    private void Start()
    {
        ammotype = AmmoType.AmmoTypes.Normal;
    }
    public override void Attack()
    {
        Debug.Log("fire pistol");
        if (currentMag > 0)
        {
            attackTimer = 0;
            canShoot = false;
            RaycastHit hit;
            shootPoint.rotation = AimAtCrosshair();
            Ray ray = new Ray(shootPoint.position, shootPoint.transform.forward + AccuracyOffset(accuracy));

            OnFire();

            if (Physics.Raycast(ray, out hit))
            {
                BulletTrail(hit.point, hit.distance, weaponElement);

                BulletAlert(transform.position, hit.point, loudness);

                if (GameManager.isOnline)
                {
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        hit.transform.GetComponent<PhotonView>().RPC("ChangeHealth", PhotonTargets.All, damage);
                    }
                }
                else
                {
                    if (hit.collider.tag == "Enemy")
                    {
                        hit.transform.GetComponent<Health>().ChangeHealth(damage, transform.position, weaponElement);
                        //print("I hit an enemy");
                    }

                }
            }
            else
            {
                // bullet trail shoots into sky
                BulletTrail(shootPoint.transform.position + (shootPoint.transform.forward + AccuracyOffset(accuracy)) * 200, 200, weaponElement);
            }

            currentMag--;

            UpdateAmmoDisplay();
        }
        if (currentMag <= 0)
        {
            StartCoroutine(ReloadTimed());
        }
    }

    public override Quaternion AimAtCrosshair()
    {
        return base.AimAtCrosshair();
    }

    public override void Reload()
    {
        StartCoroutine(ReloadTimed());
    }
}
