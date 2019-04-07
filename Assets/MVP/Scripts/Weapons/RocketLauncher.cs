using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystems;

public class RocketLauncher : Weapon
{
    public float spread;

    public override void Attack()
    {
        Quaternion hitRotation = GetTargetNormal();

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
        newBullet.Fire(transform.forward);
    }
}
