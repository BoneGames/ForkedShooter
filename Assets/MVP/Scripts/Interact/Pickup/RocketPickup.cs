using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketPickup : Pickup
{
    public int weaponIndexPosition, weaponID;
    Renderer[] rends;
    Collider[] cols;
    

    public AudioClip pickupFX;

    public override void Awake()
    {
        base.Awake();
        rends = GetComponentsInChildren<Renderer>();
        cols = GetComponentsInChildren<Collider>();
    }

    private void Update()
    {
        Rotate();
    }

    public override void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {
            RigidCharacterMovement player = other.GetComponent<RigidCharacterMovement>();
            if (!player.weapons[weaponIndexPosition].isEquipped)
            {
                // equip weapon
                player.weapons[weaponIndexPosition].isEquipped = true;
                // Make object invisible and not interactable
                for (int i = 0; i < rends.Length; i++)
                {
                    rends[i].enabled = false;
                }
                for (int i = 0; i < cols.Length; i++)
                {
                    cols[i].enabled = false;
                }
                // run SFX
                onPickup.Invoke();
                // destroiy pickup object once clip has played (double time to be safe)
                Destroy(gameObject, pickupFX.length * 2);
            }
            else
            {
                Debug.Log(player.weapons[weaponIndexPosition].GetType().Name + " Already Equipped");
            }
        }
    }

    public override void PlayClip()
    {
        aS.clip = pickupFX;
        base.PlayClip();
    }
}
