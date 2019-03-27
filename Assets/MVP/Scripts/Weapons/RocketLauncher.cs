using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystems;

public class RocketLauncher : Weapon
{
    public float spread;
    public int magSize;

    public override void Attack()
    {
        Quaternion hitRotation = GetTargetNormal();

        GameObject clone = PhotonNetwork.Instantiate("Explosive", spawnPoint.position, spawnPoint.rotation, 0);
        Projectile newBullet = clone.GetComponent<Projectile>();
        newBullet.firedBy = GetComponentInParent<PhotonView>().gameObject.name;

        newBullet.hitRotation = hitRotation;
        newBullet.Fire(transform.forward);
    }
}
