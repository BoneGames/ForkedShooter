using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystems;

public class Shotgun : Weapon
{
    public int pellets = 6;
    public float reloadSpeed;



    public override void Attack()
    {
        
        for (int i = 0; i < pellets; i++)
        {
            Vector3 spread = Vector3.zero;

            spread += transform.up * Random.Range(-accuracy, accuracy);
            spread += transform.right * Random.Range(-accuracy, accuracy);

            //GameObject clone = Instantiate(projectile, spawnPoint.position, spawnPoint.rotation);
            //Projectile newBullet = clone.GetComponent<Projectile>();

            //newBullet.Fire(direction + spread);
            Ray spreadRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward + spread);
            RaycastBullet(spreadRay);
        }   
    }

    void RaycastBullet(Ray bulletRay)
    {
        RaycastHit hit;
        if (Physics.Raycast(bulletRay, out hit))
        {
            // For reference to see where bullets hit;
            GameObject bullet = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), hit.point, Quaternion.identity);
            bullet.GetComponent<Renderer>().material.color = Color.red;
            bullet.transform.localScale = new Vector3(.15f, .15f, .15f);

            if (hit.collider.CompareTag("Player"))
            {
                hit.transform.GetComponent<PhotonView>().RPC("ChangeHealth", PhotonTargets.All, damage);
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
