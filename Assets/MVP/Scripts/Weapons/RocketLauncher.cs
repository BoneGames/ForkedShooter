using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystems;

public class RocketLauncher : Weapon
{
    Quaternion startRotation;

    public override void Start()
    {
        base.Start();

        startRotation = spawnPoint.localRotation;
    }

    public override void Attack()
    {
        if (currentMag > 0)
        {
            Quaternion hitRotation = GetTargetNormal();

            SpawnMuzzleFlash();

            GameObject clone;
            if (GameManager.isOnline)
            {
                clone = PhotonNetwork.Instantiate("Explosive", spawnPoint.position, spawnPoint.rotation, 0);
            }
            else
            {
                clone = Instantiate(projectile, spawnPoint.position, spawnPoint.rotation);
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

                        // TESTING
                        //GameObject bullet = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), hit.point, Quaternion.identity);
                        //bullet.GetComponent<Renderer>().material.color = Color.red;
                        //bullet.transform.localScale = new Vector3(.15f, .15f, .15f);
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
            //Reload();
        }
    }

    public override void Reload()
    {
        if (!(currentMag == magSize))
        {
            print("I need reloading");
            StartCoroutine(ReloadTimed());
        }
    }

    public override void SpawnMuzzleFlash()
    {
        if (muzzle)
        {
            GameObject _flash = Instantiate(muzzle, spawnPoint.transform);
            _flash.transform.SetParent(null);
            _flash.transform.localScale = new Vector3(4, 4, 4);
            Destroy(_flash, 3);
        }
    }
}
