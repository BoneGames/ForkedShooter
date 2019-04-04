using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : Pickup
{
    public int healthAmount;

    public override void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player.currentHealth < player.maxHealth)
            {
                player.ChangeHealth(-healthAmount);

                Destroy(gameObject);
            }
        }
    }
}
