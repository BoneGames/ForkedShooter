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
                try
                {
                    BulletAlert(transform.position, hit.point, loudness);

                }catch
                {

                }
    
                //Debug.Log("I hit: "+hit.transform.name);

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
                        Debug.Log(hit.collider.name);
                        if(hit.collider.GetComponent<AI_FoV_SearchLight>())
                        {
                            Debug.Log("light");
                            hit.collider.GetComponent<AI_FoV_SearchLight>().fovLight.enabled = false;
                            hit.collider.enabled = false;
                            hit.collider.GetComponent<AI_FoV_SearchLight>().viewRadius = 10;
                            //hit.transform.rotation.eulerAngles = new Vector3()
                        }
                        hit.transform.GetComponent<Health>().ChangeHealth(damage, transform.position, weaponElement);
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
