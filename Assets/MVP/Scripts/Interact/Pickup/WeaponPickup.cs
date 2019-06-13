using NaughtyAttributes;
using UnityEngine;

public class WeaponPickup : Pickup
{
    public int weaponIndexPosition;
    Renderer rend;
    Collider col;
    public float statVariation;

    public AudioClip pickupFX;

    public UniqueWeaponStats stats;
    
    public override void Awake()
    {
        base.Awake();
        rend = GetComponent<Renderer>();
        col = GetComponent<Collider>();

        if (stats == null)
        {
            Debug.Log("setting ststa");
            stats = ScriptableObject.CreateInstance<UniqueWeaponStats>();
            stats.Init(statVariation);
        }
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
                // apply unique stats to weapon on pickup
                player.weapons[weaponIndexPosition].ApplyUniqueWeaponStats(stats);
                player.weapons[weaponIndexPosition].uniqueStats = stats;
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
