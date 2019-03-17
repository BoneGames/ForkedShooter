using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GameSystems;

public class RocketLauncher : Weapon
{

    public float spread;
    public int magSize;
    public PlayerNetworkSetup playerNetworkSetup;
    public float x,y,z;

    
    public override void Attack()
    {
        //Quaternion _hitRotation = playerNetworkSetup.GetTargetNormal();
        Quaternion rotation = spawnPoint.rotation * Quaternion.Euler(x,y,z);
        playerNetworkSetup.CmdSpawnRocket(spawnPoint.position, rotation, damage);
    }
}
