using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : Pickup
{
    public int weaponIndexPosition;
    Renderer rend;
    Collider col;

    public AudioClip pickupFX;
    
    public override void Awake()
    {
        base.Awake();
        rend = GetComponent<Renderer>();
        col = GetComponent<Collider>();
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
                this.rend.enabled = false;
                this.col.enabled = false;
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
