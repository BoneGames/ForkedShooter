using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;
using BT;
using NaughtyAttributes;
using System;


public abstract class Weapon : MonoBehaviour
{
    [BoxGroup("Weapon Stats")]
    public int damage = 100, maxReserves = 30, currentReserves, magSize, currentMag;
    [BoxGroup("Weapon Stats")]
    [Slider(0, 10)] public float accuracy = 1f, loudness, bulletDetectionRadius;
    [BoxGroup("Weapon Stats")] public float scopeZoom = 75f, aimSpeed = 5f, reloadSpeed, rateOfFire = 5f;
    [BoxGroup("Weapon Stats"), HideInInspector] public float startScopeZoom = 75f;
    [BoxGroup("Weapon Stats")]
    public Elements.Element weaponElement;
    [BoxGroup("Weapon Stats")]
    public AmmoType.AmmoTypes ammoType;
    //public float range = 10f
    [BoxGroup("References")]
    public GameObject projectile, muzzle, lineRendPrefab;
    [BoxGroup("References")]
    public Transform shootPoint, aimShootPos, hipShootPos;
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
    [BoxGroup("Element Colours")]
    [ShowIf("elementColor")]
    public Color normal, fire, water, grass;

    public Vector3 hitPoint;
    float startingAccuracy;

    public LayerMask enemy;

    Quaternion hitRotation;

    public float attackTimer;

    public GradientAlphaKey[] startingAlphaKeys;

    public bool canShoot;
    public bool isEquipped;

    public bool spawnWithUniqueStats;
    public UniqueWeaponStats uniqueStats;

    [Slider(0.1f, 0.9f)]
    public float statVariation = 0.2f;

    int tempMag;

    public virtual void Awake()
    {
        if (spawnWithUniqueStats)
        {
            if (uniqueStats == null)
            {
                UniqueWeaponStats setup = ScriptableObject.CreateInstance<UniqueWeaponStats>();
                setup.Init(statVariation);
                uniqueStats = setup;
                ApplyUniqueWeaponStats(uniqueStats);
            }
        }

        currentReserves = maxReserves;
        currentMag = magSize;
        startScopeZoom = Camera.main.fieldOfView;
        DefaultReload();

        startingAccuracy = accuracy;
    }

    public void ApplyUniqueWeaponStats(UniqueWeaponStats uniqueStats)
    {
        // Get Array of all variable names in class
        var weaponVariableNames = this.GetType()
                     .GetFields()
                     .Select(field => field.Name);

        // Get Array of all stat multipliers in uniqueStats (paramater)
        var uniqueVariableNames = uniqueStats.GetType()
                     .GetFields()
                     .Select(field => field.Name);
        uniqueStats.baseStats.Clear();
        // Match variav\ble pairs with same name
        foreach (var multi in uniqueVariableNames)
        {
            if (weaponVariableNames.Contains(multi))
            {
                // Get field on weapon(as FieldInfo)
                var statField = this.GetType().GetField(multi);
                // Get matching field on UniqueWeaponStats class
                var statMultiplier = uniqueStats.GetType().GetField(multi);

                try
                {
                    // Collect weapon's base stats to reapply to it later (if weapon dropped)
                    uniqueStats.baseStats.Add(multi, Convert.ToSingle(statField.GetValue(this)));
                }
                catch
                {
                    Debug.Log("catch: " + multi);
                    uniqueStats.baseStats.Add(multi, 0);
                }


                // if statfield is an int - cast it as such when applying value
                if (statField.GetValue(this) is int)
                {
                    int newValue = Mathf.RoundToInt(((int)statField.GetValue(this) * (float)statMultiplier.GetValue(uniqueStats)));
                    statField.SetValue(this, newValue);
                }
                else if (statField.GetValue(this) is float)// if statfield is an float - cast it as such when applying value
                {
                    float newValue = (float)statField.GetValue(this) * (float)statMultiplier.GetValue(uniqueStats);
                    newValue = (float)Math.Round((double)newValue, 2);
                    statField.SetValue(this, newValue);
                }
                else
                {
                    Debug.Log("SETTING ENUM");
                    Elements.Element newValue = (Elements.Element)statMultiplier.GetValue(this);
                    statField.SetValue(this, newValue);
                }
            }
            else
            {
                //Debug.Log("UniqueWeaponStats variable: " + multi + ", does not have a counterpart to mutate in weapon script");
            }
        }
    }

    public void ResetBaseWeaponStats(Dictionary<string, float> baseValues)
    {
        // Get Array of all variable names in class
        var weaponVariableNames = this.GetType()
                     .GetFields()
                     .Select(field => field.Name);

        foreach (KeyValuePair<string, float> field in baseValues)
        {
            // check if all weapon variable names have a match for current reset Value name
            if (weaponVariableNames.Contains(field.Key))
            {
                // Get field on weapon(as FieldInfo)
                var statField = this.GetType().GetField(field.Key);

                try // apply base value as float
                {
                    statField.SetValue(this, field.Value);
                }
                catch // apply base value as int
                {
                    statField.SetValue(this, (int)field.Value);
                }
            }
        }
    }

    //public abstract void AiShoot(int _shots);
    public abstract void Attack();

    public virtual Quaternion AimAtCrosshair()
    {
        Ray crossHairRay = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;
        if (Physics.Raycast(crossHairRay, out hit, Mathf.Infinity))
        {
            Vector3 direction = hit.point - shootPoint.position;
            shootPoint.rotation = Quaternion.LookRotation(direction);
        }
        else
        {
            shootPoint.localRotation = Quaternion.Euler(0, 90, 0);
        }
        return shootPoint.rotation;
    }

    public virtual void OnAim(bool _aiming)
    {
        if (_aiming)
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
        float x = UnityEngine.Random.Range(-(1 / accuracy), 1 / accuracy);
        float y = UnityEngine.Random.Range(-(1 / accuracy), 1 / accuracy);
        float z = UnityEngine.Random.Range(-(1 / accuracy), 1 / accuracy);

        // return offset modifer
        return new Vector3(x, y, z);
    }

    //public bool canShoot;
    public void Update()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= 1f / rateOfFire)
        {
            // Can shoot!
            canShoot = true;
        }
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
            GameObject _flash = Instantiate(muzzle, shootPoint.transform);
            _flash.transform.SetParent(null);

            Destroy(_flash, 3);
        }
    }

    public virtual void UpdateAmmoDisplay()
    {
        if (UI)
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
        AlertEnemies(impactSearch, _newInspectionPoint);

        // Find enemies withing radius of fire point
        Collider[] fireSearch = Physics.OverlapSphere(origin, loudness * 2, enemy);
        AlertEnemies(fireSearch, origin);
    }

    void AlertEnemies(Collider[] enemies, Vector3 inspectionPoint)
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
                Debug.Log("Element: " + ammoType);
                return normal;
            case Elements.Element.Fire:
                Debug.Log("Element: " + ammoType);
                return fire;
            case Elements.Element.Water:
                Debug.Log("Element: " + ammoType);
                return water;
            case Elements.Element.Grass:
                Debug.Log("Element: " + ammoType);
                return grass;
            default:
                Debug.Log("You Need to add a new Color/Element to Weapon to asign to bullet trail color");
                return normal;
        }

    }

    public void BulletTrail(Vector3 target, float distance, Elements.Element bulletType)
    {
        GameObject bulletPath = Instantiate(lineRendPrefab, shootPoint.position, shootPoint.rotation);
        bulletPath.transform.SetParent(shootPoint);
        bulletPath.GetComponent<LineRenderer>().materials[0].SetColor("_TintColor", GetTrailColorBasedOn(bulletType));
        BulletPath script = bulletPath.GetComponent<BulletPath>();
        script.target = target;
        script.distance = distance;
    }
}

