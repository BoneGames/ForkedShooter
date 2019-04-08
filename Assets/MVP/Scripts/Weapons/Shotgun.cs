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
        if (currentMag > 0)
        {
            SpawnMuzzleFlash();

            UpdateAmmoDisplay();

            for (int i = 0; i < pellets; i++)
            {
                Vector3 spread = Vector3.zero;

                spread += transform.up * Random.Range(-accuracy, accuracy);
                spread += transform.right * Random.Range(-accuracy, accuracy);

                Ray spreadRay = new Ray(spawnPoint.transform.position, spawnPoint.transform.forward + spread);
                RaycastBullet(spreadRay);
            }
            currentMag--;
        }
        if (currentMag <= 0)
        {
            Reload();
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
                if(GameManager.isOnline)
                hit.transform.GetComponent<PhotonView>().RPC("ChangeHealth", PhotonTargets.All, damage);
            }

            if(hit.collider.CompareTag("Enemy"))
            {
                hit.transform.GetComponent<Health>().ChangeHealth(damage, transform.position);
            }
        }
    }

    public override void Reload()
    {
        print("Reloading...");

        StartCoroutine(GradualReload(reloadSpeed, 7));
    }

    IEnumerator GradualReload(float reloadSpeed, int seven)
    {
        while (currentMag < magSize)
        {
            currentMag++;
            UpdateAmmoDisplay();

            yield return new WaitForSeconds(reloadSpeed);
        }

        //for (int i = currentMag; i < magSize; i++)
        //{
        //    print(string.Format("Current Mag is {0}, magSize is {1}, i is {2}", currentMag, magSize, i));

        //    currentMag++;
        //    yield return new WaitForSeconds(1);
        //    print("1 second passed...");

        //    if (currentMag < magSize)
        //    {
        //        StartCoroutine(GradualReload(reloadSpeed, 7));
        //    }
        //}
    }
}
