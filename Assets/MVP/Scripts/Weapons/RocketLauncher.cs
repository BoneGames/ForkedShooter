using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystems;

public class RocketLauncher : Weapon
{
    Quaternion startRotation;
    public Transform rocketSpawn;
    Transform lookOrigin;

    InsideCollider internalCheck;

    public override void Start()
    {
        base.Start();
        lookOrigin = Camera.main.transform;
        startRotation = spawnPoint.localRotation;
        rocketSpawn = spawnPoint;

        internalCheck = rocketSpawn.GetComponent<InsideCollider>();
    }

    public override void Attack()
    {
        if (currentMag > 0)
        {
            bool insideMesh = internalCheck.InsideMesh(lookOrigin, spawnPoint);
            // if spawnPoint is inside mesh
            if (insideMesh)
            {
                print("Rocket is inside ground, time to assplode");
                rocketSpawn = GetExplosionPoint();
            }
            else
            {
                // set spawn point as standard point at end of gun
                rocketSpawn = spawnPoint;
            }

            Quaternion hitRotation = GetTargetNormal();

            SpawnMuzzleFlash();

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

            newBullet.hitRotation = hitRotation;
            newBullet.damage = damage;

            if (RigidCharacterMovement.isAiming)
            {
                Vector3 aimPoint = Vector3.zero;
                // creates a Camera ray that matches the scope needle
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height / 1.75f, 0));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    if (hit.collider)
                    {
                        aimPoint = hit.point;
                    }
                }
                spawnPoint.LookAt(aimPoint);
            }
            else
            {
                spawnPoint.localRotation = startRotation;
            }

            newBullet.Fire(spawnPoint.forward);

            currentMag--;

            UpdateAmmoDisplay();
        }

        if (currentMag <= 0)
        {
            Reload();
        }
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
