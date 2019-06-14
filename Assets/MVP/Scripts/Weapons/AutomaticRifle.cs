using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class AutomaticRifle : Weapon
{
    

    public override void Attack()
    {

        if (currentMag > 0)
        {

            RaycastHit hit;
            Ray ray = new Ray(shootPoint.position, shootPoint.transform.forward);

            SpawnMuzzleFlash();
            
            Debug.Log("Reset shoot Timer");
            attackTimer = 0;

            canShoot = false;

            if (Physics.Raycast(ray, out hit))
            {
                BulletTrail(hit.point, hit.distance);

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
                        print("I hit an enemy");
                    }

                }
            }

            currentMag--;

            UpdateAmmoDisplay();
        }
        if (currentMag <= 0)
        {
            StartCoroutine(ReloadTimed());
        }
    }




    void BulletTrail(Vector3 target, float distance)
    {
        GameObject bulletPath = Instantiate(lineRendPrefab, shootPoint.position, shootPoint.rotation);
        bulletPath.transform.SetParent(shootPoint);
        BulletPath script = bulletPath.GetComponent<BulletPath>();
        script.target = target;
        script.distance = distance;
    }

    public override void Reload()
    {
        StartCoroutine(ReloadTimed());
    }

   
}
