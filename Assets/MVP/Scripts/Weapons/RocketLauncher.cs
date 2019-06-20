using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using NaughtyAttributes;

public class RocketLauncher : Weapon
{
    //Quaternion startRotation;
    [BoxGroup("References")]
    public Transform rocketSpawn;
    Transform lookOrigin;


    public override void Awake()
    {
        base.Awake();
        //startRotation = spawnPoint.localRotation;
        lookOrigin = Camera.main.transform;
    }

    public override void Attack()
    {
        Debug.Log("ROCKET ATTACK");
        if (currentMag > 0)
        {

            bool insideMesh = internalCheck.InsideMesh(Camera.main.transform, shootPoint);
            // if spawnPoint is inside mesh
            if (insideMesh)
            {
                print("Rocket is inside ground, time to assplode");
                rocketSpawn = GetExplosionPoint();
            }
            else
            {
                // set spawn point as standard point at end of gun
                rocketSpawn = shootPoint;
            }

            //SpawnMuzzleFlash();

            OnFire();

            GameObject clone;
            if (GameManager.isOnline)
            {
                clone = PhotonNetwork.Instantiate("Explosive", rocketSpawn.position, rocketSpawn.rotation, 0);
            }
            else
            {
                clone = Instantiate(projectile, rocketSpawn.position, rocketSpawn.rotation);
            }

            Projectile newBullet = clone.GetComponent<Projectile>();

            if (GameManager.isOnline)
            {
                newBullet.firedBy = GetComponentInParent<PhotonView>().gameObject.name;
            }

            newBullet.hitRotation = GetTargetNormal();
            newBullet.damage = damage;

            shootPoint.rotation = AimAtCrosshair();

          

            newBullet.Fire(shootPoint.transform.forward + AccuracyOffset(accuracy));
            RecoilMethod();

            currentMag--;

            UpdateAmmoDisplay();
        }

        if (currentMag <= 0)
        {
            Reload();
        }
    }

    public override Quaternion AimAtCrosshair()
    {
        return base.AimAtCrosshair();
    }

    Transform GetExplosionPoint()
    {
        RaycastHit hit;
        if (Physics.Raycast(lookOrigin.position, lookOrigin.transform.forward, out hit))
        {
            rocketSpawn = Camera.main.transform;
            //GameObject bullet = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), hit.point, Quaternion.identity);
        }
        return rocketSpawn;
    }

    public override void Reload()
    {
        if (!(currentMag == magSize))
        {
            StartCoroutine(ReloadTimed());
        }
    }

    public override void SpawnMuzzleFlash()
    {
        if (muzzle)
        {
            GameObject _flash = Instantiate(muzzle, rocketSpawn.position, rocketSpawn.rotation);
            _flash.transform.SetParent(null);
            _flash.transform.localScale = new Vector3(4, 4, 4);
            Destroy(_flash, 3);
        }
    }

}
