using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{

    PhotonView photonView;
    string photonID;

    void Start()
    {  
        photonView = GetComponent<PhotonView>();
        if (photonView)
        {
            photonID = photonView.viewID.ToString().Substring(0, 1);
            this.name = "Player_" + photonID;
            FindObjectOfType<PhotonHealthMoniter>().Register(gameObject);
        }
    }
    private void Update()
    {
        CheckDie();
    }

    // Takes damage from various bullet/projectile scripts and runs 'CheckDie()'.
    [PunRPC]
    public override void ChangeHealth(int value)
    {
        currentHealth -= value;
        Debug.Log(this.name + " ChangeHealth Method Called. Health reduced by: " + value + ", and now is: " + currentHealth);

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        CheckDie();
    }

    // Self explanatory.
    public override void CheckDie()
    {
        if (currentHealth <= 0)
        {
            this.gameObject.GetComponent<RigidCharacterMovement>().StartCoroutine("Respawn");
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
