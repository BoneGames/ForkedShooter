using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystems;
public class AmmoPickup : Pickup
{
    public int ammoAmount;

    public override void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            RigidCharacterMovement player = other.GetComponent<RigidCharacterMovement>();
            if (player.currentWeapon != null)
            {
                player.currentWeapon.currentAmmo += ammoAmount;
                Destroy(gameObject);
            }
        }
    }

}
