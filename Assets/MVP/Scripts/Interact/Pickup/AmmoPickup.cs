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
                if (player.currentWeapon.currentAmmo < player.currentWeapon.maxAmmo)
                {
                    player.currentWeapon.currentAmmo += ammoAmount;

                    if (player.currentWeapon.currentAmmo >= player.currentWeapon.maxAmmo)
                    {
                        player.currentWeapon.currentAmmo = player.currentWeapon.maxAmmo;
                    }

                    player.currentWeapon.UpdateAmmoDisplay();

                    Destroy(gameObject);
                }
            }
        }
    }
}
