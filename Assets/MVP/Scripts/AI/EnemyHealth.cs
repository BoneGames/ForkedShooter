using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    // Set damage immunity on/off (handled from InvulTotem.cs).
    public bool isGod = false;
    public PhotonView photonView;

    void Awake()
    {  
        photonView = GetComponent<PhotonView>();
    }

    // Takes damage from various bullet/projectile scripts and runs 'CheckDie()'.
    [PunRPC]
    public override void ChangeHealth(int value)
    {
        if (!isGod)
        {
            currentHealth -= value;
            CheckDie();
        }
    }

    // Self explanatory.
    public override void CheckDie()
    {
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
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
