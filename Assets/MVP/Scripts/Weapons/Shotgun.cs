﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

public class Shotgun : Weapon
{
    [BoxGroup("Weapon Stats")]
    public int pellets = 6;

    public bool isReloading;

    public override void Attack()
    {
        if (currentMag > 0)
        {
            if (isReloading)
            {
                isReloading = false;
            }
            attackTimer = 0;
            canShoot = false;
            //SpawnMuzzleFlash();

            currentMag--;
            UpdateAmmoDisplay();

            shootPoint.transform.rotation = AimAtCrosshair();

            if (UI.aimUi.recoilHeight != 0)
                accuracy *= UI.aimUi.recoilHeight / 200;

            for (int i = 0; i < pellets; i++)
            {
                Ray spreadRay = new Ray(shootPoint.transform.position, shootPoint.transform.forward + AccuracyOffset(accuracy));
                RaycastBullet(spreadRay);
                OnFire();
            }
            RecoilMethod();
        }
        if (currentMag <= 0 && autoReload)
        {
            Reload();
        }
    }

    public override Quaternion AimAtCrosshair()
    {
        return base.AimAtCrosshair();
    }

    void RaycastBullet(Ray bulletRay)
    {
        RaycastHit hit;
        if (Physics.Raycast(bulletRay, out hit))
        {
            BulletTrail(hit.point, hit.distance, weaponElement);
            BulletAlert(transform.position, hit.point, loudness);

            if (hit.collider.CompareTag("Player"))
            {
                if (GameManager.isOnline)
                    hit.transform.GetComponent<PhotonView>().RPC("ChangeHealth", PhotonTargets.All, damage);
            }

            if (hit.collider.CompareTag("Enemy"))
            {
                //Debug.Log(hit.collider.name);
                if (hit.collider.GetComponent<AI_FoV_SearchLight>())
                {
                    Debug.Log("hit Drone light - it should be off now");
                    hit.collider.GetComponent<AI_FoV_SearchLight>().fovLight.enabled = false;
                    hit.collider.enabled = false;
                    hit.collider.GetComponent<AI_FoV_SearchLight>().viewRadius = 10;
                }
                hit.transform.GetComponent<Health>().ChangeHealth(damage, transform.position, weaponElement);
            }
        }
        else
        {
            BulletTrail(shootPoint.transform.position + (shootPoint.transform.forward + AccuracyOffset(accuracy)) * 200, 200, weaponElement);
        }
    }

    public override void Reload()
    {
        StartCoroutine(GradualReload(reloadSpeed));
    }

    IEnumerator GradualReload(float reloadSpeed)
    {
        isReloading = true;
        while (currentMag < magSize && isReloading)
        {
            if (currentReserves > 0)
            {
                currentMag++;
                currentReserves--;
            }

            UpdateAmmoDisplay();

            yield return new WaitForSeconds(reloadSpeed);
        }
        isReloading = false;
    }
}
