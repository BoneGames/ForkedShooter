﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

public class EnemyHealth : Health
{
    // Set damage immunity on/off (handled from InvulTotem.cs).
    public bool ShowStates;
    [ShowIf("ShowStates")]
    [BoxGroup("Enemy States")]
    public bool isGod = false;
    [HideInInspector] public PhotonView photonView;

    public bool ShowDrops;
    [ShowIf("ShowDrops")]
    [BoxGroup("Enemy Drops")]
    public GameObject ammoBox, healthDrop;
    bool firstDrop = true;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    // Takes damage from various bullet/projectile scripts and runs 'CheckDie()'.
    [PunRPC]
    public override void ChangeHealth(float value, Vector3 shotDir, Elements.Element ammoType)
    {
        if (!isGod)
        {
            value = CheckWeakness(value, ammoType);
            Debug.Log(value);
            currentHealth -= value;
            healthBar.UpdateBar();
            CheckDie();
        }
        // Turn to look at attacker
        transform.LookAt(shotDir);
    }

    //public override void ChangeHealth(float value, Vector3 shotDir)
    //{
    //    if (!isGod)
    //    {
    //        currentHealth -= value;
    //        Debug.Log("g");
    //        healthBar.UpdateBar();
    //        CheckDie();
    //    }
    //    Turn to look at attacker
    //    transform.LookAt(shotDir);
    //}


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
        if (firstDrop)
        {
            firstDrop = false;
            int dropRate = Random.Range(1, 5);
            GameObject clone = null;

            switch (dropRate)
            {
                case 1:
                case 2:
                case 3:
                    clone = Instantiate(ammoBox, transform.position + (Vector3.up * 3), transform.rotation);
                    break;
                case 4:
                case 5:
                    clone = Instantiate(healthDrop, transform.position + (Vector3.up * 3), transform.rotation);
                    break;
                default:
                    break;
            }
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
