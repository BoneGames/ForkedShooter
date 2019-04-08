using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    // Set damage immunity on/off (handled from InvulTotem.cs).
    public bool isGod = false;
    public PhotonView photonView;

    [Header("Enemy Drops")]
    public GameObject ammoBox;
    public GameObject healthDrop;
    
    void Awake()
    {  
        photonView = GetComponent<PhotonView>();
    }

    // Takes damage from various bullet/projectile scripts and runs 'CheckDie()'.
    [PunRPC]
    public override void ChangeHealth(int value, Vector3 shotDir)
    {
        if (!isGod)
        {
            currentHealth -= value;
            healthBar.UpdateBar();
            CheckDie();
        }
    }

    // Self explanatory.
    public override void CheckDie()
    {
        if (currentHealth <= 0)
        {
            DropItem();

            Destroy(gameObject);
        }
    }

    void DropItem()
    {
        int dropRate = Random.Range(1, 5);
        GameObject clone;

        switch (dropRate)
        {
            case 1:
            case 2:
            case 3:
                clone = Instantiate(ammoBox, new Vector3(transform.position.x, transform.position.y + 3, transform.position.z), transform.rotation);
                break;
            case 4:
            case 5:
                clone = Instantiate(healthDrop, new Vector3(transform.position.x, transform.position.y + 3, transform.position.z), transform.rotation);
                break;
            default:
                break;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //Send health data to network
        if(stream.isWriting)
        {
            stream.SendNext(currentHealth);
			//stream.SendNext()
        }
        // recieve health data from network (other player)
        else if(stream.isReading)
        {
            currentHealth = (int)stream.ReceiveNext();
        }
    }
}
