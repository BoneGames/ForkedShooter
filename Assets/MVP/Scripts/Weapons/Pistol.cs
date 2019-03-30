﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystems;

public class Pistol : Weapon
{
    public float spread;

    public override void Attack()
    {
        if (currentMag > 0)
        {
            RaycastHit hit;
            Ray ray = new Ray(spawnPoint.position, spawnPoint.transform.forward);
            if (Physics.Raycast(ray, out hit))
            {
                // For reference to see where bullets hit;
                //GameObject bullet = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), hit.point, Quaternion.identity);
                //bullet.GetComponent<Renderer>().material.color = Color.red;
                //bullet.transform.localScale = new Vector3(.15f, .15f, .15f);

                if (hit.collider.CompareTag("Player"))
                {
                    hit.transform.GetComponent<PhotonView>().RPC("ChangeHealth", PhotonTargets.All, damage);
                }
            }
            currentMag--;
        }
        if(currentMag <= 0 )
        {
            Reload();
        }
    }
}
