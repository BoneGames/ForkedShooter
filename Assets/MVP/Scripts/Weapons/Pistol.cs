using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystems;
public class Pistol : Weapon
{
  public override void Attack()
  {
    if (currentMag > 0)
    {
      RaycastHit hit;
      spawnPoint.rotation = AimAtCrosshair();
      Ray ray = new Ray(spawnPoint.position, spawnPoint.transform.forward);

      SpawnMuzzleFlash();

      audioWep.PlayOneShot(sfx[0]);

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
            hit.transform.GetComponent<Health>().ChangeHealth(damage, transform.position);
            //print("I hit an enemy");
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
