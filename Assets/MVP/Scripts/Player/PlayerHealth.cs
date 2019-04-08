using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    PhotonView photonView;
    string photonID;
    [HideInInspector]
    public GameObject shotDirectionArm;
    public float shotIndicatorDelay;

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
    public override void ChangeHealth(int value, Vector3 shotDir)
    {
        currentHealth -= value;

        Debug.Log(this.name + " ChangeHealth Method Called. Health reduced by: " + value + ", and now is: " + currentHealth);

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        ShotDirection(shotDir);

        CheckDie();
    }

    void ShotDirection(Vector3 incoming)
    {
        // Angle between other pos vs player
        Vector3 incomingDir = (transform.position - incoming).normalized;

        // Flatten to plane
        Vector3 otherDir = new Vector3(-incomingDir.x, 0f, -incomingDir.z);
        var playerFwd = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
 
        // Direction between player fwd and incoming object
        var angle = Vector3.SignedAngle(playerFwd, otherDir, Vector3.up);
        shotDirectionArm.transform.rotation = Quaternion.Euler(0,0, -angle);
        StartCoroutine("ShotDirectionActive");
    }

    IEnumerator ShotDirectionActive()
    {
        shotDirectionArm.SetActive(true);
        yield return new WaitForSeconds(shotIndicatorDelay);
        shotDirectionArm.SetActive(false);
    }

    // Self explanatory.
    public override void CheckDie()
    {
        healthBar.UpdateBar();

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
