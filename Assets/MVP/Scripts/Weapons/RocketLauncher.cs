﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GameSystems;

public class RocketLauncher : Weapon
{

    public float spread;
    public int magSize;

    [Client]
    public override void Attack()
    {
        Quaternion hitRotation = GetTargetNormal();

        GameObject clone = Instantiate(projectile, spawnPoint.position, spawnPoint.rotation);
        Projectile newBullet = clone.GetComponent<Projectile>();

        newBullet.hitRotation = hitRotation;
        newBullet.Fire(transform.forward);
    }
}
