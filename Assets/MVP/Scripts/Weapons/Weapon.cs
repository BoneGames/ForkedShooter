using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

namespace GameSystems
{
    public abstract class Weapon : MonoBehaviour
    {
        public int damage = 100;
        public int maxAmmo = 30;
        public float accuracy = 1f;
        public float range = 10f;
        public float rateOfFire = 5f;
        public GameObject projectile;
        public Transform spawnPoint;

        public Vector3 hitPoint;
        Quaternion hitRotation;

        public int magSize;
        public int currentAmmo;

        public int currentMag;
        public int tempMag;

        public bool isOnline;

        private void Start()
        {
            currentAmmo = maxAmmo;
            currentMag = magSize;
        }

        public abstract void Attack();

        public Quaternion GetTargetNormal()
        {
            RaycastHit hit;
            //Ray ray = Camera.main.ScreenPointToRay(Screen);
            Vector3 screenCentre = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            Ray ray = Camera.main.ScreenPointToRay(screenCentre);

            if (Physics.Raycast(ray, out hit))
            {
                hitPoint = hit.point;
                hitRotation = hit.transform.rotation;
            }
            return hitRotation;
        }

        public virtual void Reload()
        {
            print(BaneTools.ColorString(gameObject.name + " is reloading!", BaneTools.Color255(0,255,0)));
            if (currentAmmo > 0)
            {
                if (currentAmmo >= magSize)
                {
                    currentAmmo -= magSize - currentMag;

                    currentMag = magSize;
                }
                if (currentAmmo < magSize)
                {
                    tempMag = currentAmmo;
                    currentMag = tempMag;
                    currentAmmo -= tempMag;
                }
            }
        }
    }
}
