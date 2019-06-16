using System.Collections;
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
            //SpawnMuzzleFlash();

            currentMag--;
            UpdateAmmoDisplay();

            //audioWep.PlayOneShot(sfx[0]);
            //pitchShifter.Tweet();

            //OnFire();

            shootPoint.transform.rotation = AimAtCrosshair();

            for (int i = 0; i < pellets; i++)
            {
                Ray spreadRay = new Ray(shootPoint.transform.position, shootPoint.transform.forward + AccuracyOffset(accuracy));
                RaycastBullet(spreadRay);
                OnFire();
            }
        }
        //if (currentMag <= 0)
        //{
        //  Reload();
        //}
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
            BulletTrail(hit.point, hit.distance);
            BulletAlert(transform.position, hit.point, loudness);

            if (hit.collider.CompareTag("Player"))
            {
                if (GameManager.isOnline)
                    hit.transform.GetComponent<PhotonView>().RPC("ChangeHealth", PhotonTargets.All, damage);
            }

            if (hit.collider.CompareTag("Enemy"))
            {
                hit.transform.GetComponent<Health>().ChangeHealth(damage, transform.position, weaponElement);
            }
        }
        else
        {
            BulletTrail(shootPoint.transform.position + (shootPoint.transform.forward + AccuracyOffset(accuracy)) * 200, 200);
        }
    }

    void BulletTrail(Vector3 target, float distance)
    {
        GameObject bulletPath = Instantiate(lineRendPrefab, shootPoint.position, shootPoint.rotation);
        bulletPath.transform.SetParent(null);
        BulletPath script = bulletPath.GetComponent<BulletPath>();
        script.target = target;
        script.distance = distance;
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
