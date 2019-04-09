using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystems;
using System.Linq;

public class Pistol : Weapon
{

    public override void Attack()
    {
        
        if (currentMag > 0)
        {
            RaycastHit hit;
            Ray ray = new Ray(spawnPoint.position, spawnPoint.transform.forward);

            SpawnMuzzleFlash();
            UpdateAmmoDisplay();

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
                    //print("I'm firing!");
                    //Debug.DrawRay(spawnPoint.position, spawnPoint.forward, Color.red);

                    if (hit.collider.tag == "Enemy")
                    {
                        hit.transform.GetComponent<Health>().ChangeHealth(damage, transform.position);
                        print("I hit an enemy");
                    }

                }
            }
            currentMag--;
        }
        if(currentMag <= 0 )
        {
            Reload();
        }
    }

    void BulletTrail(Vector3 target, float distance)
    {
        GameObject bulletPath = Instantiate(lineRendPrefab, spawnPoint.position, spawnPoint.rotation);
        bulletPath.transform.SetParent(spawnPoint);
        BulletPath script = bulletPath.GetComponent<BulletPath>();
        script.target = target;
        script.distance = distance;
    }
}
