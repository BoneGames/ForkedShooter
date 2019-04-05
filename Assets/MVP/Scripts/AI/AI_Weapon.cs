using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystems;

public class AI_Weapon : Weapon
{
    #region Variable
    // Check in AI_ScoutDrone.cs for visibleTargets.
    public BehaviourAI contact;
    public float reloadTime;
    #endregion

    #region Functions 'n' Methods
    // Where we initialize / Start things.
    void Start()
    {
        // Grab 'contact's component, give full ammo, and start Coroutine.
        contact = GetComponentInParent<BehaviourAI>();
        StartCoroutine("Shoot");
    }

    // Where we define shooting.
    public override void Attack()
    {
        Debug.Log("Attack");
        // If there is a player in our line of sight, and we still have ammo to work with...
        if (isOnline && contact.fov.visibleTargets.Count > 0 && currentMag > 0)
        {
            
            RaycastHit hit;
            Ray ray = new Ray(spawnPoint.position, spawnPoint.transform.forward);
            
            if (Physics.Raycast(ray, out hit))
            {
                // For reference to see where bullets hit;
                //GameObject bullet = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), hit.point, Quaternion.identity);
                //bullet.GetComponent<Renderer>().material.color = Color.red;
                //bullet.transform.localScale = new Vector3(.15f, .15f, .15f);

                if (isOnline)
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        hit.transform.GetComponent<PhotonView>().RPC("ChangeHealth", PhotonTargets.All, damage);
                    }
                }
                else
                {
                    print("I'm firing!");
                    Debug.DrawRay(spawnPoint.position, spawnPoint.forward, Color.red);

                    if (hit.collider.tag == "Player")
                    {
                        hit.transform.GetComponent<Health>().ChangeHealth(damage);
                        //print("I hit an enemy");
                    }
                }
                // // Fire bullets at it.
                // GameObject clone = Instantiate(projectile, spawnPoint.position, spawnPoint.rotation);
                // Bullet newBullet = clone.GetComponent<Bullet>();

                // newBullet.Fire(spawnPoint.transform.forward);
                // newBullet.sourceAgent = this.gameObject;
                //print("Firing.");
                currentMag--;
                Debug.Log(currentMag);
            }
        }
        // If we run out of ammo, start reloading and stop shooting.
        if (currentMag == 0)
        {
            StartCoroutine("StartReload", reloadTime);
            StopCoroutine("Shoot");
        }
    }

    /// Garbage (old 'BurstFire()' and 'Shoot()').
    /// // Where we run Attack() multiple times.
    /// IEnumerator BurstFire()
    /// {
    ///     while (true)
    ///     {
    ///         Attack();
    ///         yield return new WaitForSeconds(0.1f);
    ///         Attack();
    ///         yield return new WaitForSeconds(0.1f);
    ///         Attack();
    ///         yield return new WaitForSeconds(0.1f);
    ///         StopCoroutine("BurstFire");
    ///     }
    /// }
    /// 
    /// // Where we run BurstFire().
    /// IEnumerator Shoot()
    /// {
    ///     // While the Coroutine is running...
    ///     while (true)
    ///     {
    ///         StartCoroutine("BurstFire");
    ///         yield return new WaitForSeconds(Random.Range(0.5f, 1f));
    ///     }
    /// }

    // Where we run Attack() multiple times.
    IEnumerator BurstFire(int burstCount, float burstDelay)
    {
        //Rather than calling the same function three times successively, we do a loop calling them up to a given value
        //This reduces hardcoding and allows the function to be modified more easily outside of the code, or when called.
        for (int i = 0; i < burstCount; i++)
        {
            Attack();

            yield return new WaitForSeconds(burstDelay);
        }
    }

    private void Update()
    {
         Debug.DrawRay(spawnPoint.position, spawnPoint.transform.forward);
    }
    // Where we run BurstFire().
    IEnumerator Shoot()
    {
        while (true)
        {
            Debug.Log("shootroutine");
            // Run BurstFire every 0.5 ↔ 1.0 seconds.

            //This form of StartCoroutine doesn't use the string name to run it
            //This makes it more consistent with other functions, and simplifies 
            //passing in more paramaters 
            StartCoroutine(BurstFire(3, .1f));
            yield return new WaitForSeconds(Random.Range(0.5f, 1f));
        }
    }

    // Where we run Reload().
    IEnumerator StartReload(float reloadTime)
    {
        while (true)
        {
            // Wait (reloadTime) seconds before reloading, then you can start shooting again.
            yield return new WaitForSeconds(reloadTime);
            Reload();
            StartCoroutine("Shoot");
            StopCoroutine("StartReload");
        }
    }
    #endregion
}
