using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/* Task 1:  Draw.io projectile system
 *          Needs the following structure: 
 *              Projectile
 *              /   |   \
 *          Normal Fire Explosive
 *          Variables and functions for each class
 *          
 *############################################################################ 
 * 
 * Task 2:  Ensure the player can't shoot until the weapon is
 *          ready to be fired (fire rate)
 *          Refer to #game-systems-j211 for resources for this task
 *          
 */

namespace GameSystems
{
    // took off "abstract class" in order to add to player prefab so that the networked object 
    // was in the scene and attached to a network ID (necesary for certain functions online)
    public abstract class Weapon : NetworkBehaviour
    {
        #region OldCode
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
        #endregion
        public int ammo = 30;
        public int damage;
        public float accuracy = 1f;
        public float range = 10f;
        public float rateOfFire = 5f;
        public Transform spawnPoint;

        Quaternion hitRotation;

        protected int currentAmmo = 0;
     

        public abstract void Attack();
        

  
        // public Quaternion GetTargetNormal()
        // {
        //     RaycastHit hit;
        //     //Ray ray = Camera.main.ScreenPointToRay(Screen);
        //     Vector3 screenCentre = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        //     Ray ray = Camera.main.ScreenPointToRay(screenCentre);

        //     if (Physics.Raycast(ray, out hit))
        //     {
        //         hitPoint = hit.point;
        //         hitRotation = hit.transform.rotation;
        //     }
        //     return hitRotation;
        // }

        public virtual void Reload()
        {
            currentAmmo = ammo;
        }
    }
}
