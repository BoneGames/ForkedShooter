using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GameSystems;

public class Pistol : Weapon
{
    #region Old
    //public GameObject bullet;
    //public Transform spawnPoint;


    //void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        GameObject clone = Instantiate(bullet, spawnPoint.position, spawnPoint.rotation);
    //        Bullet newBullet = clone.GetComponent<Bullet>();

    //        newBullet.Fire(transform.forward);
    //    }
    //}
    #endregion
    public float spread;
    public int magSize;
    public int damage = 5;
    PlayerNetworkSetup playerNetworkSetup;

    void Start()
    {
        playerNetworkSetup = GetComponentInParent<PlayerNetworkSetup>();
    }

    [Client]
    public override void Attack()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out hit))
        {
            // For reference to see where bullets hit;
            GameObject bullet = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), hit.point, Quaternion.identity);
            bullet.transform.localScale = new Vector3(.15f, .15f, .15f);
            if (hit.collider.tag == "Player")
            {
                // Server Command Method on Player object (has to be on object with network identity component)
                playerNetworkSetup.CmdPlayerShot(hit.collider.name, damage);
            }
        }
    }

}
