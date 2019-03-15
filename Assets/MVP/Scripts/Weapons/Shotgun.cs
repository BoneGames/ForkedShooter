using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystems;
using UnityEngine.Networking;

public class Shotgun : Weapon
{
    public int pellets = 6;
    public float reloadSpeed;

    public override void Attack()
    {
        // old code

        //for (int i = 0; i < pellets; i++)
        //{
        //    Vector3 direction = transform.forward;
        //    Vector3 spread = Vector3.zero;

        //    spread += transform.up * Random.Range(-accuracy, accuracy);
        //    spread += transform.right * Random.Range(-accuracy, accuracy);

        //    GameObject clone = Instantiate(projectile, spawnPoint.position, spawnPoint.rotation);
        //    Projectile newBullet = clone.GetComponent<Projectile>();

        //    newBullet.Fire(direction + spread);
        //}

        for (int i = 0; i < pellets; i++)
        {
            Vector3 direction = transform.forward;
            Vector3 spread = Vector3.zero;

            spread += transform.up * Random.Range(-accuracy, accuracy);
            spread += transform.right * Random.Range(-accuracy, accuracy);

            Ray spreadRay = new Ray(transform.position, transform.forward + spread);
            RaycastBullet(spreadRay);
        }
    }

    [Client]
    void RaycastBullet(Ray _ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(_ray, out hit))
        {
            GameObject bullet = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), hit.point, Quaternion.identity);
            bullet.transform.localScale = new Vector3(.15f, .15f, .15f);

            if (hit.collider.tag == "Player")
            {
                // Server Command Method in Weapon Base Class
                test(hit.collider.name,
                    this.name);
                //CmdPlayerShot(
                //    hit.collider.name, 
                //    this.name);
            }
        }
    }

    public override void Reload()
    {
        float timer = 3f;
        while (currentAmmo < ammo)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                currentAmmo += 1;
                timer = 3f;
            }
        }
    }
    //public GameObject bullet;
    //public Transform spawnPoint;

    //// Use this for initialization
    //void Start()
    //{

    //}


    //void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        GameObject clone = Instantiate(bullet, spawnPoint.position, spawnPoint.rotation);
    //        Bullet newBullet = clone.GetComponent<Bullet>();

    //        newBullet.Fire(transform.forward);
    //    }
    //}
}
