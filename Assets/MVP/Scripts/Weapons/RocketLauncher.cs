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

    
    public override void Attack()
    {
        //Quaternion _hitRotation = playerNetworkSetup.GetTargetNormal();
        playerNetworkSetup.CmdSpawnRocket(spawnPoint.position, spawnPoint.rotation, damage);
    }
}
