using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class PlayerHealth : Health
{
    PhotonView photonView;
    string photonID;

    public bool ShowShotIndicator;
    [ShowIf("ShowShotIndicator")]
    [BoxGroup("Shot Indicator")]
    public GameObject shotDirectionArm;
    [ShowIf("ShowShotIndicator")]
    [BoxGroup("Shot Indicator")]
    public float shotIndicatorDelay;

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
                    StopCoroutine(ShotDirectionActive(_shotDir));
                    StartCoroutine(ShotDirectionActive(_shotDir));
                }

                CheckDie();
            }
        }
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
        shotDirectionArm.transform.rotation = Quaternion.Euler(0, 0, -angle);
    }

    IEnumerator ShotDirectionActive(Vector3 incoming)
    {
        shotDirectionArm.SetActive(true);
        float timer = 0;
        while (timer < shotIndicatorDelay)
        {
            timer += Time.deltaTime;

            // Angle between other pos vs player
            Vector3 incomingDir = (transform.position - incoming).normalized;

            // Flatten to plane
            Vector3 otherDir = new Vector3(-incomingDir.x, 0f, -incomingDir.z);
            var playerFwd = Vector3.ProjectOnPlane(transform.forward, Vector3.up);

            // Direction between player fwd and incoming object
            var angle = Vector3.SignedAngle(playerFwd, otherDir, Vector3.up);
            shotDirectionArm.transform.rotation = Quaternion.Euler(0, 0, -angle);
            yield return null;
        }
        shotDirectionArm.SetActive(false);
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
