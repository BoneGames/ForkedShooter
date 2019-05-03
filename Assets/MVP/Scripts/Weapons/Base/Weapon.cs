using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BT;

namespace GameSystems
{
    public abstract class Weapon : MonoBehaviour
    {
        public int damage = 100;
        public int maxAmmo = 30;

        public float accuracy = 1f;
        //public float range = 10f;
        public float scopeZoom = 75f;
        public float reloadSpeed;
        public float rateOfFire = 5f;
        public GameObject projectile;
        public Transform spawnPoint;

        public GameObject muzzle;
        //public Vector3 hitPoint;

        Quaternion hitRotation;
        public GameObject lineRendPrefab;

        public GradientAlphaKey[] startingAlphaKeys;
        [Tooltip("meters/second")]

        public int magSize;
        public int currentAmmo;

        public int currentMag;
        public int tempMag;

        public Text ammoDisplay;

        public virtual void Start()
        {
            currentAmmo = maxAmmo;
            currentMag = magSize;

            DefaultReload();
        }

        public abstract void Attack();


        public virtual void Aiming()
        {
            //if (RigidCharacterMovement.isAiming)
            // {
            Vector3 aimPoint = Vector3.zero;

            // creates a Camera ray that matches the scope needle
            Ray ray = Camera.main.ScreenPointToRay(BaneMath.screenCentre);

            // The below code calibrates the hitpoint to be on rocket sights crosshair (aiming), or on the onscreen UI cross hair  (not aiming)
            // This is obsolete once we fix our 3D models to have sights screen-centred when on aiming mode.
            // Ray ray = RigidCharacterMovement.isAiming ? Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height / 1.7f, 0)) : Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * .5f, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider)
                {
                    aimPoint = hit.point;
                }
            }
            //What's this for?
            spawnPoint.LookAt(aimPoint);
            //}
        }


        public Quaternion GetTargetNormal()
        {
            RaycastHit hit;
            //Ray ray = Camera.main.ScreenPointToRay(Screen);
            Vector3 screenCentre = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            Ray ray = Camera.main.ScreenPointToRay(screenCentre);

            if (Physics.Raycast(ray, out hit))
            {
                //hitPoint = hit.point;
                hitRotation = hit.transform.rotation;
            }
            return hitRotation;
        }

        public virtual void Reload()
        {
            DefaultReload();

            UpdateAmmoDisplay();
        }

        public virtual void SpawnMuzzleFlash()
        {
            if (muzzle)
            {
                GameObject _flash = Instantiate(muzzle, spawnPoint.transform);
                _flash.transform.SetParent(null);

                Destroy(_flash, 3);
            }
        }

        public virtual void UpdateAmmoDisplay()
        {
            if (ammoDisplay)
                ammoDisplay.text = string.Format("{0}/{1} // {2}/{3}", currentMag, magSize, currentAmmo, maxAmmo);
        }
        public IEnumerator ReloadTimed()
        {
            print(BaneTools.ColorString("Reloading", "green"));
            yield return new WaitForSeconds(reloadSpeed);
            DefaultReload();
        }

        void DefaultReload()
        {
            //print(BaneTools.ColorString(gameObject.name + " is reloading!", BaneTools.Color255(0, 255, 0)));
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

            UpdateAmmoDisplay();
        }
    }
}
