using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystems;

public class Shotgun : Weapon
{
    public int pellets = 6;

    public override void Attack()
    {
        if (currentMag > 0)
        {
            SpawnMuzzleFlash();

            UpdateAmmoDisplay();

            for (int i = 0; i < pellets; i++)
            {
                Vector3 spread = Vector3.zero;

                spread += transform.up * Random.Range(-accuracy, accuracy);
                spread += transform.right * Random.Range(-accuracy, accuracy);

                Ray spreadRay = new Ray(spawnPoint.transform.position, spawnPoint.transform.forward + spread);
                RaycastBullet(spreadRay);
            }
            currentMag--;
            UpdateAmmoDisplay();
        }
        if (currentMag <= 0)
        {
            Reload();
        }
    }
    
    void RaycastBullet(Ray bulletRay)
    {
        RaycastHit hit;
        if (Physics.Raycast(bulletRay, out hit))
        {
            BulletTrail(hit.point, hit.distance);

            if (hit.collider.CompareTag("Player"))
            {
                if(GameManager.isOnline)
                hit.transform.GetComponent<PhotonView>().RPC("ChangeHealth", PhotonTargets.All, damage);
            }

            if(hit.collider.CompareTag("Enemy"))
            {
                hit.transform.GetComponent<Health>().ChangeHealth(damage, transform.position);
            }
        }
    }

    void BulletTrail(Vector3 target, float distance)
    {
        GameObject bulletPath = Instantiate(lineRendPrefab, spawnPoint.position, spawnPoint.rotation);
        bulletPath.transform.SetParent(null);
        BulletPath script = bulletPath.GetComponent<BulletPath>();
        script.target = target;
        script.distance = distance;
    }

    public override void Reload()
    {
        StartCoroutine(GradualReload(reloadSpeed, 7));
    }

    IEnumerator GradualReload(float reloadSpeed, int seven)
    {
        while (currentMag < magSize)
        {
            currentMag++;
            currentAmmo--;
            UpdateAmmoDisplay();

            yield return new WaitForSeconds(reloadSpeed);
        }
    }
}
