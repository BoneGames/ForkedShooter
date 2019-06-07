using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class PlayerHealth : Health
{
    PhotonView photonView;
    string photonID;

    public bool ShowShotIndicator;
    [ShowIf("ShowShotIndicator")] [BoxGroup("Shot Indicator")]
    public Color normal, fire, water, grass;


    public override void Start()
    {
        base.Start();
        shield = GetComponentInChildren<ShieldController>();
        photonView = GetComponent<PhotonView>();
        if (photonView)
        {
            photonID = photonView.viewID.ToString().Substring(0, 1);
            this.name = "Player_" + photonID;
            FindObjectOfType<PhotonHealthMoniter>().Register(gameObject);
        }
    }

    // Takes damage from various bullet/projectile scripts and runs 'CheckDie()'.
    [PunRPC]
    public override void ChangeHealth(float _value, Vector3 _shotDir, Elements.Element ammoType)
    {
        if (currentShield > 0)
        {
            shield.gameObject.SetActive(true);

            if (currentShield > _value)
            {
                currentShield -= _value;
            }
            else if (_value >= currentShield)
            {
                carryOnDmg = _value - currentShield;
                currentShield -= _value;
                currentHealth -= carryOnDmg;

                currentShield = currentShield < 0 ? 0 : currentShield;

                CheckDie();
            }
        }
        else if (currentShield <= 0)
        {
            currentShield = 0;
            shield.gameObject.SetActive(false);

            if (currentHealth > 0)
            {
                currentHealth -= _value;

                //print(_value > 0 ? string.Format("Health reduced by {0} and is now {1}", _value, currentHealth) : string.Format("Health healed by {0} and is now {1}", -_value, currentHealth));

                if (currentHealth > maxHealth)
                {
                    currentHealth = maxHealth;
                }

                //If you're actually being damaged (negative means healing)
                if (_value > 0)
                {
                    UI.shotDirection.ShotIndicator(_shotDir, ammoType);
                    //StopCoroutine(ShotDirectionActive(_shotDir, ammoType));
                    //StartCoroutine(ShotDirectionActive(_shotDir, ammoType));
                }

                CheckDie();
            }
        }
    }


    // Self explanatory.
    public override void CheckDie()
    {
        //healthBar.UpdateBar(currentHealth, maxHealth);
        updateHealthBar.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            this.gameObject.GetComponent<RigidCharacterMovement>().StartCoroutine("Respawn");
            base.CheckDie();
            // show respawn text
            UI.deathMessage.StartRespawnText();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //Send health data to network
        if (stream.isWriting)
        {
            stream.SendNext(currentHealth);
            //stream.SendNext()
        }
        // recieve health data from network (other player)
        else if (stream.isReading)
        {
            currentHealth = (int)stream.ReceiveNext();
        }
    }
}
