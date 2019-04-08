using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : Pickup
{
    public int healthAmount;

    public void Update()
    {
        Rotate();
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();

            if (player.currentHealth < player.maxHealth)
            {
                player.ChangeHealth(-healthAmount, transform.position);

                if(player.currentHealth > player.maxHealth)
                {
                    player.currentHealth = player.maxHealth;
                }

                Destroy(gameObject);
            }
        }
    }   
}
