using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AI_Weapon : Weapon
{
    public int elementIndex = 0;
    public Elements.Element[] elementArray = new Elements.Element[] { Elements.Element.Normal, Elements.Element.Fire, Elements.Element.Water, Elements.Element.Grass };
    #region Variables
    // Check in AI_ScoutDrone.cs for visibleTargets.
    [Header("AI Weapon Variables")]
    public BehaviourAI contact; // Contact with the BehaviourAI conditions.
    public GameObject hitParticle;

    [Header("Burst Fire")]
    //public int burstCount; // Number of shots fired per burst (1 = semi-automatic).
    public float burstDelay; // Time between each shot fired in a burst.
    public float reloadTime; // Self explanatory.

    //public Elements.Element element;
    #endregion

    #region Functions 'n' Methods


    // Where we run Attack() multiple times.
    IEnumerator BurstFire(int burstCount, float burstDelay)
    {
        // burst fire loop
        for (int i = 0; i < burstCount; i++)
        {
            Attack();
            yield return new WaitForSeconds(burstDelay);
        }
    }

    // Where we run BurstFire(). - accessed from Behaviour_AI
    public void Shoot(int _shots)
    {
       StartCoroutine(BurstFire(_shots, burstDelay)); 
    }

    // Where we define shooting.
    public override void Attack()
    {
        //Debug.Log("Attack");
        // If there is a player in our line of sight, and we still have ammo to work with...
        if (currentMag > 0)
        {
            RaycastHit hit;
            Ray ray = new Ray(spawnPoint.position, spawnPoint.transform.forward);

            SpawnMuzzleFlash();

            Vector3 direction = transform.forward;

            direction.x += Random.Range(-accuracy, accuracy);
            direction.y += Random.Range(-accuracy, accuracy);

            if (Physics.Raycast(ray.origin, direction, out hit))
            {
                if (lineRendPrefab)
                {
                    BulletTrail(hit.point, hit.distance);
                }
                SpawnHitParticle(hit.point);

                if (GameManager.isOnline)
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        hit.transform.GetComponent<PhotonView>().RPC("ChangeHealth", PhotonTargets.All, damage);
                    }
                }
                else
                {
                    //print("I'm firing!");
                    Debug.DrawRay(spawnPoint.position, spawnPoint.forward * 10, Color.magenta, 1);

                    if (hit.collider.tag == "Player")
                    {
                        elementIndex++;
                        if(elementIndex >= elementArray.Length-1)
                        {
                            elementIndex = 0;
                        }
                        weaponElement = elementArray[elementIndex];


                        Debug.Log("AI Element: "+weaponElement);
                        hit.transform.GetComponent<Health>().ChangeHealth(damage, transform.position, weaponElement);
                        //print("I hit an enemy");
                    }
                }
                /// newBullet.sourceAgent = this.gameObject;
                /// print("Firing.");
                currentMag--;
                //Debug.Log(currentMag);
            }
        }
        // If we run out of ammo, start reloading and stop shooting.
        else
        {
            StartCoroutine(ReloadTimed());
            //StopCoroutine("Shoot");
        }
    }

    void BulletTrail(Vector3 _target, float _dist)
    {
        GameObject bulletPath = Instantiate(lineRendPrefab, spawnPoint.position, spawnPoint.rotation);
        bulletPath.transform.SetParent(spawnPoint);
        BulletPath _bulletPath = bulletPath.GetComponent<BulletPath>();
        _bulletPath.target = _target;
        _bulletPath.distance = _dist;
    }

    void SpawnHitParticle(Vector3 hit)
    {
        GameObject _flash = Instantiate(hitParticle, hit, Quaternion.identity);
        Destroy(_flash, 3);
    }

    
    #endregion
}
