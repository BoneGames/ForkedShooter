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

  void Awake()
  {
    photonView = GetComponent<PhotonView>();
  }

  public override void Start()
  {
    base.Start();
  }

  // Takes damage from various bullet/projectile scripts and runs 'CheckDie()'.
  [PunRPC]
  public override void ChangeHealth(float value, Vector3 shotDir, Elements.Element ammoType)
  {
    if (!isGod)
    {
      value = CheckWeakness(value, ammoType);

      currentHealth -= value;
      Debug.Log("g");
      healthBar.UpdateBar();
      CheckDie();
    }
    // Turn to look at attacker
    transform.LookAt(shotDir);
  }


  // Self explanatory.
  public override void CheckDie()
  {
    if (currentHealth <= 0)
    {
      base.CheckDie();
      Destroy(gameObject);
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
