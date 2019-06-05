using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;
public class Pistol : Weapon
{
    public override void Attack()
    {
        if (currentMag > 0)
        {
            RaycastHit hit;
            spawnPoint.rotation = AimAtCrosshair();
            Ray ray = new Ray(spawnPoint.position, spawnPoint.transform.forward + AccuracyOffset(accuracy));

            OnFire();

            //SpawnMuzzleFlash();

            //audioWep.PlayOneShot(sfx[0]);
            //pitchShifter.Tweet();

            if (Physics.Raycast(ray, out hit))
            {
                BulletTrail(hit.point, hit.distance);

                BulletAlert(transform.position, hit.point, loudness);

                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = hit.point;
                sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

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
            BulletTrail(spawnPoint.transform.position + (spawnPoint.transform.forward + AccuracyOffset(accuracy)) * 200, 200);
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

    void BulletTrail(Vector3 target, float distance)
    {
        GameObject bulletPath = Instantiate(lineRendPrefab, spawnPoint.position, spawnPoint.rotation);
        bulletPath.transform.SetParent(spawnPoint);
        BulletPath script = bulletPath.GetComponent<BulletPath>();
        script.target = target;
        script.distance = distance;
    }

    public override void Reload()
    {
        StartCoroutine(ReloadTimed());
    }
}
