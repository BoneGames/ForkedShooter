using NaughtyAttributes;
using UnityEngine;

public class WeaponPickup : Pickup
{
    public int weaponIndexPosition;
    Renderer rend;
    Collider col;
    Renderer[] rends;
    Collider[] cols;
    public float statVariation;
    bool isRocket;

    public AudioClip pickupFX;

    public UniqueWeaponStats stats;
    
    public override void Awake()
    {
        isRocket = name.Contains("Rocket") ? true : false;

        base.Awake();
        //SetBeamScale();

        if(isRocket)
        {
            rends = GetComponentsInChildren<Renderer>();
            cols = GetComponentsInChildren<Collider>();
        }
        else
        {
            rend = GetComponentInChildren<Renderer>();
            col = GetComponent<Collider>();
        }

        if (stats == null)
        {
            stats = ScriptableObject.CreateInstance<UniqueWeaponStats>();
            stats.Init(statVariation);
        }
    }

    void SetBeamScale()
    {
        float xScale = 0.0175f/this.transform.localScale.x;
        beam.transform.localScale = new Vector3(xScale, xScale * 100, xScale * 100);
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
                if(isRocket)
                {
                    for (int i = 0; i < rends.Length; i++)
                    {
                        rends[i].enabled = false;
                    }
                    for (int i = 0; i < cols.Length; i++)
                    {
                        cols[i].enabled = false;
                    }
                }
                else
                {
                    this.rend.enabled = false;
                    this.col.enabled = false;
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
