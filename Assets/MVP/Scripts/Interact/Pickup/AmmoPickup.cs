using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : Pickup
{
    public AmmoType.AmmoTypes ammotype;
    public AmmoType.AmmoTypes[] pickupType;
    public int ammoAmount;
    

    public void Start()
    {
        ammotype = pickupType[Random.Range(0,3)];
    }
    public override void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            RigidCharacterMovement player = other.GetComponent<RigidCharacterMovement>();

            if (player.currentWeapon.ammoType == ammotype && player.currentWeapon != null)
            {
                if (player.currentWeapon.currentReserves < player.currentWeapon.maxReserves)
                {
                    player.currentWeapon.currentReserves += ammoAmount;

                    if (player.currentWeapon.currentReserves >= player.currentWeapon.maxReserves)
                    {
                        player.currentWeapon.currentReserves = player.currentWeapon.maxReserves;
                    }

                    player.currentWeapon.UpdateAmmoDisplay();

                    Destroy(gameObject);
                }
            }
        }
    }
}
