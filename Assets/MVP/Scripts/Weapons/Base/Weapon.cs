using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using BT;
using NaughtyAttributes;


public abstract class Weapon : MonoBehaviour
{
    [BoxGroup("Weapon Stats")]
    public int damage = 100, maxReserves = 30, currentReserves, magSize, currentMag;
    [BoxGroup("Weapon Stats")]
    [Slider(0, 10)] public float accuracy = 1f, loudness, bulletDetectionRadius;
    [BoxGroup("Weapon Stats")] public float scopeZoom = 75f, reloadSpeed, rateOfFire = 5f;
    [BoxGroup("Weapon Stats")]
    public Elements.Element weaponElement;
    //public float range = 10f
    [BoxGroup("References")]
    public GameObject projectile, muzzle, lineRendPrefab;
    [BoxGroup("References")]
    public Transform spawnPoint, aimShootPos, hipShootPos;
    public Vector3 aimOffset;
    [BoxGroup("References")]
    public Text ammoDisplay;
    public UIHandler UI;
    [BoxGroup("References")]
    public SfxPitchShifter pitchShifter;
    [BoxGroup("References")]
    public AudioSource audioWep;
    [BoxGroup("References")]
    public AudioClip[] sfx;

    [BoxGroup("Events")]
    public UnityEvent onFire;

    public bool elementColor = true;
    [BoxGroup("Element Colours")][ShowIf("elementColor")]
    public Color normal, fire, water, grass;

    public Vector3 hitPoint;
    float startingAccuracy;

    public LayerMask enemy;
    
    Quaternion hitRotation;

    public GradientAlphaKey[] startingAlphaKeys;

    public bool canShoot;
    public bool isEquipped;
    public WeaponStats stats;

    int tempMag;

    public virtual void Start()
    {
        currentReserves = maxReserves;
        currentMag = magSize;
        DefaultReload();
        startingAccuracy = accuracy;
    }

    public abstract void Attack();

    public virtual Quaternion AimAtCrosshair()
    {
        Ray crossHairRay = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;
        if (Physics.Raycast(crossHairRay, out hit, Mathf.Infinity))
        {
            Vector3 direction = hit.point - spawnPoint.position;
            spawnPoint.rotation = Quaternion.LookRotation(direction);
        }
        else
        {
            spawnPoint.localRotation = Quaternion.Euler(0, 90, 0);
        }
        return spawnPoint.rotation;
    }

    public virtual void OnAim(bool _aiming)
    {
        if(_aiming)
        {
            accuracy = startingAccuracy * 1.5f;
        }
        else
        {
            accuracy = startingAccuracy;
        }
    }

    public virtual Vector3 AccuracyOffset(float accuracy)
    {
        // increase in order to keep slider friendly: 0-10 range
        accuracy *= 10;
        // generate random values within accuracy range
        float x = Random.Range(-(1 / accuracy), 1 / accuracy);
        float y = Random.Range(-(1 / accuracy), 1 / accuracy);
        float z = Random.Range(-(1 / accuracy), 1 / accuracy);

        // return offset modifer
        return new Vector3(x, y, z);
    }
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
        DefaultReload();

        UpdateAmmoDisplay();
    }

    public void OnFire()
    {
        onFire.Invoke();
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
        if(UI)
        UI.UpdateAmmoDisplay(currentMag, magSize, currentReserves, maxReserves);
        //if (ammoDisplay)
        //    ammoDisplay.text = string.Format("{0}/{1} // {2}/{3}", currentMag, magSize, currentReserves, maxReserves);
    }
    public IEnumerator ReloadTimed()
    {
        yield return new WaitForSeconds(reloadSpeed);
        DefaultReload();
    }

    void DefaultReload()
    {
        //print(BaneTools.ColorString(gameObject.name + " is reloading!", BaneTools.Color255(0, 255, 0)));
        if (currentReserves > 0)
        {
            if (currentMag >= 0)
            {
                currentReserves += currentMag;
                currentMag = 0;

                if (currentReserves >= magSize)
                {
                    currentReserves -= magSize - currentMag;

                    currentMag = magSize;
                }
                else if (currentReserves < magSize)
                {
                    tempMag = currentReserves;
                    currentMag = tempMag;
                    currentReserves -= tempMag;
                }
            }
        }

        UpdateAmmoDisplay();
    }

    public void BulletAlert(Vector3 origin, Vector3 _hitPoint, float loudness)
    {
        // point 70% of the way from bit point to origin along trajectory
        Vector3 dir = (origin - _hitPoint) * 0.7f;
        Vector3 _newInspectionPoint = origin - dir;

        // Find enemies within radius of bullet impact
        Collider[] impactSearch = Physics.OverlapSphere(_hitPoint, bulletDetectionRadius, enemy);
        TellEnemies(impactSearch, _newInspectionPoint);

        // Find enemies withing radius of fire point
        Collider[] fireSearch = Physics.OverlapSphere(origin, loudness * 2, enemy);
        TellEnemies(fireSearch, origin);
    }
    void TellEnemies(Collider[] enemies, Vector3 inspectionPoint)
    {
        if (enemies.Length > 0)
        {
            foreach (Collider col in enemies)
            {
                if (col.GetComponent<BehaviourAI>())
                {
                    // send alert to enemy
                    col.GetComponent<BehaviourAI>().BulletAlert(inspectionPoint);
                }
            }
        }
    }

    public Color GetTrailColorBasedOn(Elements.Element ammoType)
    {
        // set shotDirArm color
        switch (ammoType)
        {
            case Elements.Element.Normal:
                Debug.Log("switch: " + ammoType);
                return normal;
            case Elements.Element.Fire:
                Debug.Log("switch: " + ammoType);
                return fire;
            case Elements.Element.Water:
                Debug.Log("switch: " + ammoType);
                return water;
            case Elements.Element.Grass:
                Debug.Log("switch: " + ammoType);
                return grass;
            default:
                Debug.Log("You Need to add a new Color/Element to Weapon to asign to bullet trail color");
                return normal;
        }

    }

}

